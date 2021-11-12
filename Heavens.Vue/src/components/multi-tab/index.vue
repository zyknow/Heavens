<template>
  <div class="h-full">
    <div class="flex-row flex-j-bet h-8 mt-0.5 w-full pl-2 pr-2 multi-tab-bar">
      <q-tabs
        align="left"
        active-color="primary"
        class="col-12 h-8"
        style="width: 97%"
        dense
        swipeable
        inline-label
        indicator-color="transparent"
        :breakpoint="0"
        outside-arrows
      >
        <div
          v-for="(tab, index) in multiTabStore.tagCaches"
          :key="index"
          class="
            page-tab
            h-full
            cursor-pointer
            flex-row flex-all-center
            ml-0.5
            mr-0.5
            pl-2
            pr-2
            space-x-1
          "
          :class="tab.path == $router.currentRoute.value.path ? 'page-tab-active' : ''"
          @click="$router.push(tab.path)"
        >
          <div class="flex-row flex-all-center">
            <q-icon class="page-tab-title-icon" size="1.3rem" v-if="tab.icon" :name="tab.icon" />
            <span class="page-tab-title text-gray-500">{{ t(tab.tabTitle || '') }}</span>
          </div>

          <transition></transition>
          <q-icon
            class="page-tab-icon"
            :class="state.refreshLoading && tab.path == multiTabStore.current ? 'animate-spin' : ''"
            name="autorenew"
            @click.stop="refresh(tab.path)"
          />
          <q-icon class="page-tab-icon" name="close" @click.stop="actions.close(tab.path)" />

          <q-menu touch-position context-menu>
            <q-list dense>
              <q-item clickable v-close-popup>
                <q-item-section @click="refresh(tab.path)">{{ $t('刷新') }}</q-item-section>
              </q-item>
              <q-item clickable v-close-popup>
                <q-item-section @click="actions.closeOther(tab.path)">
                  {{ $t('关闭其他') }}
                </q-item-section>
              </q-item>
              <q-item clickable v-close-popup>
                <q-item-section @click="actions.closeLeft(tab.path)">
                  {{ $t('关闭左侧所有') }}
                </q-item-section>
              </q-item>
              <q-item clickable v-close-popup>
                <q-item-section @click="actions.closeRight(tab.path)">
                  {{ $t('关闭右侧所有') }}
                </q-item-section>
              </q-item>
            </q-list>
          </q-menu>
        </div>
      </q-tabs>
      <div class="flex-all-center">
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
    <div style="height: calc(100% - 32px)">
      <router-view v-slot="{ Component }">
        <keep-alive :exclude="multiTabStore.exclude" v-if="state.cachingEnabled">
          <component :is="!state.refreshLoading ? Component : ''"></component>
        </keep-alive>
        <component v-else :is="!state.refreshLoading ? Component : ''"></component>
        <q-inner-loading :showing="state.refreshLoading"></q-inner-loading>
      </router-view>
      <!-- <router-view v-slot="{ Component }" v-else>
        <component :is="!refreshLoading ? Component : ''"></component>
        <q-inner-loading :showing="refreshLoading"></q-inner-loading>
      </router-view> -->
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
  cachingEnabledLoading: false,
})

const setCachingEnabled = async enabled => {
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
  include: [] as string[],
}) as MultiTabStore
const actions = MultiTabAction(multiTabStore)

const refresh = async path => {
  state.refreshLoading = true
  await actions.refreshAsync(path)
  state.refreshLoading = false
}
</script>

<style lang="sass" scoped>
.multi-tab-bar
  box-sizing: border-box
  border-bottom: 1px solid rgb(209, 209, 209)

.page-tab
  border-style: solid
  border-width: 1px
  border-top-width: '0px'
  border-top-color: white
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
  font-size: 1rem
  border-radius: 0.2rem
  opacity: 0.58
  transition: all 0.3s


.page-tab-icon:hover
  @apply text-light-primary opacity-100 cursor-pointer
</style>
