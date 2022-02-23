<template>
  <q-layout view="hhh lpR lFr" class="basic-layout">
    <basic-header class="basic-header" />

    <q-drawer v-model="state.drawerOpen" :width="250" :mini="state.drawerMini" side="left" class="rounded-lg">
      <q-list class="rounded-borders text-primary">
        <nav-menu-item :routers="router.children" />
      </q-list>
    </q-drawer>
    <q-page-container class="basic-page-container">
      <multi-tabs v-if="$store.getters['app/multiTabEnabled'] && !$q.platform.is.mobile" />
      <router-view v-else />
    </q-page-container>
  </q-layout>
</template>

<script lang="ts" setup>
import NavMenuItem from 'src/components/navigation/nav-menu-item.vue'
import BasicHeader from 'src/components/headers/basic-header.vue'
import MultiTabs from 'src/components/multi-tab/index.vue'
import { ref, defineComponent, provide, InjectionKey, readonly, reactive, toRefs, computed, watch } from 'vue'
import { useQuasar } from 'quasar'
import store from '@/store'
import { CHANGE_LEFT_DRAWER_FUN, LEFT_DRAWER_CLOSED_KEY } from '.'

// import { useMultiTabStateProvider } from 'src/components/multi-tab-bar/multi-table-store'

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
const router = store.getters['user/routers']
</script>
<style lang="sass">

.basic-layout
  @apply bg-gray-300 fixed h-screen w-screen
  .basic-header
    @apply  bg-white rounded-lg m-2

  .basic-page-container
    padding-bottom: 29px
    @apply m-4 mr-2 max-h-full h-full

  @media screen and (min-width: 500px)
    .q-drawer-container
      .q-drawer
        top: 66px !important
        left: 8px !important
        bottom: 12px !important
        @apply rounded-lg
</style>
