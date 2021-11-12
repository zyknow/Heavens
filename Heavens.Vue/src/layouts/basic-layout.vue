<template>
  <q-layout view="hhh lpR lFr" class="h-screen">
    <div class="w-full fixed z-20">
      <basic-header @changeDrawer="changeDrawer"></basic-header>
    </div>

    <div class="h-full fixed z-20">
      <q-drawer
        v-model="state.drawerOpen"
        :width="250"
        :mini="state.drawerMini"
        show-if-above
        side="left"
        elevated
      >
        <q-list class="rounded-borders text-primary">
          <nav-menu-item :routers="router.children"></nav-menu-item>
        </q-list>
      </q-drawer>
    </div>
    <!-- 99.5% 修复出现滚动条 -->
    <q-page-container style="height: 99.5%">
      <multi-tabs
        v-if="$store.getters['app/multiTabEnabled'] && !$q.platform.is.mobile"
      ></multi-tabs>

      <router-view v-else></router-view>
    </q-page-container>
  </q-layout>
</template>

<script lang="ts" setup>
import NavMenuItem from 'src/components/navigation/nav-menu-item.vue'
import BasicHeader from 'src/components/headers/basic-header.vue'
import MultiTabs from 'src/components/multi-tab/index.vue'
import {
  ref,
  defineComponent,
  provide,
  InjectionKey,
  readonly,
  reactive,
  toRefs,
  computed,
  watch,
} from 'vue'
import { useQuasar } from 'quasar'
import store from '@/store'
import { leftDrawerClosedKey } from '.'

// import { useMultiTabStateProvider } from 'src/components/multi-tab-bar/multi-table-store'

const $q = useQuasar()
const state = reactive({
  drawerMini: false,
  drawerOpen: true,
})
const changeDrawer = () => {
  if ($q.platform.is.mobile) state.drawerOpen = !state.drawerOpen
  else state.drawerMini = !state.drawerMini
}

provide(
  leftDrawerClosedKey,
  computed(() => state.drawerMini),
)
const router = store.getters['user/routers']
</script>
