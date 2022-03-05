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
          v-for="(tab, index) in multiTabState.tagCaches"
          :key="index"
          class="flex page-tab h-full cursor-pointer flex-row flex-nowrap items-center justify-center mr-0.5 pl-2 pr-2 space-x-0.5"
          :class="tab.name == $router.currentRoute.value.name ? 'page-tab-active' : ''"
          @click="$router.push(tab.name)"
        >
          <q-tab class="hidden" />
          <div class="flex flex-row flex-nowrap items-center justify-center">
            <q-icon v-if="tab.icon" class="page-tab-title-icon" size="1.3rem" :name="tab.icon" />
            <span class="page-tab-title text-gray-500">{{ t(tab.tabTitle || '') }}</span>
          </div>

          <!-- 刷新按钮 -->
          <q-icon
            v-if="tab.name == multiTabState.current"
            class="page-tab-icon"
            :class="multiTabState.loading && tab.name == multiTabState.current ? 'animate-spin' : ''"
            name="r_autorenew"
            @click.stop="refresh(tab.name)"
          />
          <!-- X按钮 -->
          <q-icon class="page-tab-icon" name="close" @click.stop="multiTabState.close(tab.name)" />
          <!-- 右键菜单 -->
          <q-menu touch-position context-menu>
            <q-list dense>
              <q-item v-close-popup clickable>
                <q-item-section @click="refresh(tab.name)">{{ $t('刷新') }}</q-item-section>
              </q-item>
              <q-item v-close-popup clickable>
                <q-item-section @click="multiTabState.closeOther(tab.name)">
                  {{ t('关闭其他') }}
                </q-item-section>
              </q-item>
              <q-item v-close-popup clickable>
                <q-item-section @click="multiTabState.closeLeft(tab.name)">
                  {{ t('关闭左侧所有') }}
                </q-item-section>
              </q-item>
              <q-item v-close-popup clickable>
                <q-item-section @click="multiTabState.closeRight(tab.name)">
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
          :color="appStore.multiTabCacheEnabled ? 'green' : 'gray'"
          name="data_usage"
          @click="setCachingEnabled(!appStore.multiTabCacheEnabled)"
        >
          <q-tooltip
            class="text-white flex-col"
            :class="appStore.multiTabCacheEnabled ? 'bg-green-400 ' : ' bg-gray-400'"
            :offset="[10, 10]"
          >
            <span>{{ appStore.multiTabCacheEnabled ? t('已开启标签页缓存') : t('未开启标签页缓存') }}</span>
          </q-tooltip>
        </q-icon>
      </div>
    </div>
    <!-- 内容页 -->
    <div style="height: calc(100% - 35px)" class="w-full h-full">
      <route-view />
    </div>
  </div>
</template>
<script lang="ts" setup>
import { defineComponent, reactive, toRefs, computed } from 'vue'
import { isDev, ls, sleepAsync } from 'src/utils'
import { useI18n } from 'vue-i18n'
import RouteView from '@/layouts/route-view.vue'
import multiTabState from './multi-tab-state'
import { appStore } from '@/store/app-store'
const t = useI18n().t
const state = reactive({
  cachingEnabledLoading: false
})

const setCachingEnabled = async (enabled: boolean) => {
  state.cachingEnabledLoading = true
  // 延迟动画 可以移除
  await sleepAsync(500)
  state.cachingEnabledLoading = false
  appStore.multiTabCacheEnabled = enabled
}

const refresh = async (name: string) => {
  multiTabState.loading = true
  await multiTabState.refreshAsync(name)
  multiTabState.loading = false
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
