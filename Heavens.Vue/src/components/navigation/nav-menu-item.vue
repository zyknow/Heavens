<template>
  <div v-for="(router, index) in routers" :key="index">
    <q-item
      v-if="!router.children"
      clickable
      v-ripple
      :inset-level="childLevel == 0 ? undefined : childLevel"
      :to="router.path"
      class="text-gray-700"
      exact-active-class="bg-blue-100"
      active-class="text-primary"
      @mouseenter="state.rightMenuVisible = false"
    >
      <!-- :style="state.drawerMini ? 'padding-left: 0px' : ''" -->

      <q-tooltip
        v-if="state.drawerMini"
        transition-show="flip-right"
        transition-hide="flip-left"
        anchor="center right"
        self="center left"
        :offset="[10, 10]"
      >
        {{ t(router.meta?.title || '') }}
      </q-tooltip>
      <q-item-section avatar>
        <q-icon :name="router.meta?.icon || ''" />
      </q-item-section>
      <q-item-section>
        {{ t(router.meta?.title || '') }}
      </q-item-section>
    </q-item>
    <q-expansion-item
      v-else
      :duration="10"
      :icon="router.meta?.icon"
      :label="t(router.meta?.title || '')"
      @mouseenter="showRightMenu()"
      class="text-gray-700"
    >
      <template #header>
        <q-item-section avatar>
          <q-icon :name="router.meta?.icon">
            <!-- 注释该 q-menu 以取消mini状态下的 右侧menu -->
            <q-menu
              transition-show="flip-right"
              transition-hide="flip-left"
              anchor="top right"
              self="top left"
              v-model="state.rightMenuVisible"
              v-if="state.drawerMini && router.children && router.children.length > 0"
              :offset="[17, 10]"
            >
              <nav-right-menu-item :routers="router.children"></nav-right-menu-item>
            </q-menu>
          </q-icon>
        </q-item-section>
        <q-item-section>
          {{ t(router.meta?.title || '') }}
        </q-item-section>
      </template>
      <template #default>
        <div>
          <nav-menu-item
            v-if="router.children && router.children.length > 0"
            :routers="router.children"
            :childLevel="childLevel + 1"
          ></nav-menu-item>
        </div>
      </template>
    </q-expansion-item>
  </div>
</template>
<script lang="ts" setup>
import {
  defineComponent,
  PropType,
  ref,
  inject,
  watch,
  reactive,
  toRefs,
  computed,
  defineProps,
} from 'vue'
import { MenuDataItem } from '@/router/_typing'
import { useI18n } from 'vue-i18n'
import navRightMenuItem from './nav-right-menu-item.vue'
import { leftDrawerClosedKey } from '@/layouts'

const props = defineProps({
  routers: {
    type: Object as PropType<MenuDataItem[]>,
    default: () => [] as MenuDataItem[],
  },
  isMenu: {
    type: Boolean,
    default: false,
  },
  childLevel: {
    type: Number,
    default: 0,
  },
})
const t = useI18n().t
const libDialog = ref()
const state = reactive({
  drawerMini: inject(leftDrawerClosedKey, ref(true)),
  rightMenuVisible: false,
  addLibDialogVisible: false,
})
const showRightMenu = () => {
  if (state.drawerMini) state.rightMenuVisible = !state.rightMenuVisible
}

const show = () => {
  libDialog.value.show()
}
</script>
<style lang="sass"></style>
