<template>
  <q-layout view="hhh lpR lFr" class="main-layout">
    <basic-header class="basic-header" />

    <q-drawer
      v-model="state.drawerOpen"
      :width="250"
      :mini="state.drawerMini"
      side="left"
      class="layout-rounded"
      style="-webkit-scroll"
      :class="state.drawerMini ? 'overflow-hidden' : ''"
    >
      <q-scroll-area class="h-full">
        <q-list class="rounded-borders text-primary">
          <nav-menu-item :routers="router.children" />
        </q-list>
      </q-scroll-area>
    </q-drawer>
    <q-page-container class="basic-page-container">
      <multi-tabs v-if="appStore.multiTabEnabled && !$q.platform.is.mobile" class="bg-white layout-rounded" />
      <div v-else class="w-full h-full flex bg-white layout-rounded">
        <router-view class="w-full h-full" />
      </div>
    </q-page-container>
  </q-layout>
</template>

<script lang="ts" setup>
import NavMenuItem from 'src/components/navigation/nav-menu-item.vue'
import BasicHeader from 'src/components/headers/basic-header.vue'
import MultiTabs from 'src/components/multi-tab/index.vue'
import { ref, defineComponent, provide, InjectionKey, readonly, reactive, toRefs, computed, watch } from 'vue'
import { useQuasar } from 'quasar'
import { CHANGE_LEFT_DRAWER_FUN, LEFT_DRAWER_CLOSED_KEY } from '.'
import { MenuDataItem } from '@/router/_typing'
import { userStore } from '@/store/user-store'
import { appStore } from '@/store/app-store'
const $q = useQuasar()
const state = reactive({
  drawerMini: false,
  drawerOpen: true
})
const changeDrawer = () => {
  if ($q.platform.is.mobile) state.drawerOpen = !state.drawerOpen
  else state.drawerMini = !state.drawerMini
}

provide(CHANGE_LEFT_DRAWER_FUN, changeDrawer)

provide(
  LEFT_DRAWER_CLOSED_KEY,
  computed(() => state.drawerMini)
)
const router = userStore.routers as MenuDataItem
</script>
<style lang="sass">

// 所有窗口类型统一圆角样式
.layout-rounded
  @apply rounded-md overflow-hidden

.main-layout
  @apply bg-gray-300 fixed h-screen w-screen
  .basic-header
    @apply  bg-white m-2 layout-rounded

  .basic-page-container
    padding-bottom: 28px
    padding-right: 24px
    @apply m-4 mr-2 max-h-full h-full w-full

  @media screen and (min-width: 500px)
    .q-drawer-container
      .q-drawer
        top: 66px !important
        left: 8px !important
        bottom: 12px !important
        @apply layout-rounded
</style>
