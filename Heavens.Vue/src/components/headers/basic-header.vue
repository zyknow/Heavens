<template>
  <q-header class="text-white">
    <q-toolbar class="text-black">
      <!-- left header -->
      <div class="flex flex-row items-center space-x-4">
        <q-btn dense flat round icon="menu" @click="changeLeftDrawer()" />
        <!-- 这才是标题栏 -->
        <div v-if="!$q.platform.is.mobile" class="flex flex-row items-center">
          <q-spinner-box color="orange" size="2.5em" />
          <span
            class="bg-clip-text text-transparent bg-gradient-to-r from-pink-500 to-blue-500 header-title-text text-xl font-semibold"
            >HeavensFeel</span
          >
        </div>
        <!-- 完成搜索功能 -->
        <q-input
          v-model="state.search"
          :dark="state.searchDarkStyle"
          dense
          standout
          type="search"
          debounce="500"
          :label="t('全局搜索')"
          input-class="text-right"
          @focus="state.searchDarkStyle = true"
          @blur="state.searchDarkStyle = false"
        >
          <template #append>
            <q-icon name="r_travel_explore" />
          </template>
        </q-input>
      </div>
      <q-toolbar-title />
      <!-- right header -->
      <right-header-menu class="sm:hidden" />
    </q-toolbar>
  </q-header>
</template>
<script lang="ts" setup>
import { reactive, inject } from 'vue'
import { useI18n } from 'vue-i18n'
import { CHANGE_LEFT_DRAWER_FUN } from '@/layouts'
import RightHeaderMenu from './right-header-menu.vue'

const changeLeftDrawer = inject(CHANGE_LEFT_DRAWER_FUN) as () => void

const t = useI18n().t
const state = reactive({
  search: '',
  searchDarkStyle: false
})
</script>
<style lang="sass" scoped>
// .sticky-header-column-table
//   tr th
//     font-size: 1.1rem
//     position: sticky
//     z-index: 2
//   thead tr:first-child th
//     top: 0
//     z-index: 1
</style>
