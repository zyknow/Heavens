<template>
  <div class="h-full flex flex-row flex-wrap w-full">
    <!-- 多标签父div -->
    <div class="flex flex-row pr-2 whitespace-nowrap w-full">
      <q-tabs
        align="left"
        active-color="primary"
        dense
        inline-label
        indicator-color="transparent"
        :breakpoint="0"
        outside-arrows
        style="width: calc(100% - 30px)"
        class="flex flex-row flex-nowrap items-center h-8"
      >
        <!-- 多标签 -->
        <div
          v-for="(tab, index) in multiTabStore.tagCaches"
          :key="index"
          class="flex page-tab h-full cursor-pointer flex-row flex-nowrap items-center justify-center mr-0.5 pl-2 pr-2 space-x-0.5"
          :class="tab.path == $router.currentRoute.value.path ? 'page-tab-active' : ''"
          @click="$router.push(tab.path)"
        >
          <q-tab class="hidden" />
          <div class="flex flex-row flex-nowrap items-center justify-center">
            <q-icon v-if="tab.icon" class="page-tab-title-icon" size="1.3rem" :name="tab.icon" />
            <span class="page-tab-title text-gray-500">{{ t(tab.tabTitle || '') }}</span>
          </div>

          <!-- 刷新按钮 -->
          <q-icon
            v-if="tab.path == multiTabStore.current"
            class="page-tab-icon"
            :class="state.refreshLoading && tab.path == multiTabStore.current ? 'animate-spin' : ''"
            name="r_autorenew"
            @click.stop="refresh(tab.path)"
          />
          <!-- X按钮 -->
          <q-icon class="page-tab-icon" name="close" @click.stop="actions.close(tab.path)" />
          <!-- 右键菜单 -->
          <q-menu touch-position context-menu>
            <q-list dense>
              <q-item v-close-popup clickable>
                <q-item-section @click="refresh(tab.path)">{{ $t('刷新') }}</q-item-section>
              </q-item>
              <q-item v-close-popup clickable>
                <q-item-section @click="actions.closeOther(tab.path)">
                  {{ t('关闭其他') }}
                </q-item-section>
              </q-item>
              <q-item v-close-popup clickable>
                <q-item-section @click="actions.closeLeft(tab.path)">
                  {{ t('关闭左侧所有') }}
                </q-item-section>
              </q-item>
              <q-item v-close-popup clickable>
                <q-item-section @click="actions.closeRight(tab.path)">
                  {{ t('关闭右侧所有') }}
                </q-item-section>
              </q-item>
            </q-list>
          </q-menu>
        </div>
      </q-tabs>

      <!-- 侧缓存按钮 -->
      <div class="flex items-center justify-center">
        <q-icon
          class="cursor-pointer"
          :class="
            state.cachingEnabledLoading
              ? 'animate-spin'
              : 'transition transform ease-in-out duration-500 hover:scale-125'
          "
          size="1.5rem"
          :color="state.cachingEnabled ? 'green' : 'gray'"
          name="data_usage"
          @click="setCachingEnabled(!state.cachingEnabled)"
        >
          <q-tooltip
            class="text-white flex-col"
            :class="state.cachingEnabled ? 'bg-green-400 ' : ' bg-gray-400'"
            :offset="[10, 10]"
          >
            <span>{{ state.cachingEnabled ? t('已开启标签页缓存') : t('未开启标签页缓存') }}</span>
          </q-tooltip>
        </q-icon>
      </div>
    </div>
    <!-- 内容页 -->
    <div style="height: calc(100% - 35px)" class="w-full h-full">
      <router-view v-slot="{ Component }">
        <keep-alive v-if="state.cachingEnabled" :exclude="multiTabStore.exclude">
          <component :is="!state.refreshLoading ? Component : ''" />
        </keep-alive>
        <component :is="!state.refreshLoading ? Component : ''" v-else />
        <q-inner-loading :showing="state.refreshLoading" />
      </router-view>
    </div>
  </div>
</template>
<script lang="ts" setup>
import { defineComponent, reactive, toRefs, computed } from 'vue'
import { CacheItem, MultiTabAction, MultiTabStore } from './multi-table-store'
import router from 'src/router'
import { isDev, ls, sleepAsync } from 'src/utils'
import { useI18n } from 'vue-i18n'

const MULTI_TAB_CACHING_ENABLED = 'multi-tab-caching-enabled'

const t = useI18n().t
const state = reactive({
  tab: 'multi-table',
  refreshLoading: false,
  cachingEnabled: ls.getItem(MULTI_TAB_CACHING_ENABLED),
  cachingEnabledLoading: false
})

const setCachingEnabled = async (enabled: boolean) => {
  state.cachingEnabledLoading = true
  await sleepAsync(1000)
  state.cachingEnabledLoading = false
  state.cachingEnabled = enabled
  ls.set(MULTI_TAB_CACHING_ENABLED, enabled)
}

const multiTabStore = reactive({
  tagCaches: [] as CacheItem[],
  current: computed(() => router.currentRoute.value.path),
  exclude: [] as string[],
  include: [] as string[]
}) as MultiTabStore
const actions = MultiTabAction(multiTabStore)

const refresh = async (path: string) => {
  state.refreshLoading = true
  await actions.refreshAsync(path)
  state.refreshLoading = false
}
</script>

<style lang="sass" scoped>
.page-tab
  .page-tab-title,
  .page-tab-title-icon
    @apply text-gray-500


.page-tab:hover
  .page-tab-title,
  .page-tab-title-icon
    @apply text-light-primary

.page-tab-active
  .page-tab-title,
  .page-tab-title-icon
    @apply text-primary

  // .page-tab-title
  //   @apply font-bold

  border-bottom-color: white


.page-tab-icon
  font-size: 1.2rem
  border-radius: 0.2rem
  opacity: 0.58
  transition: all 0.3s


.page-tab-icon:hover
  @apply text-light-primary opacity-100 cursor-pointer font-bold
</style>
