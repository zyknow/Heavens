<template>
  <q-header class="bg-white text-white shadow-sm">
    <q-toolbar class="text-black w-full">
      <!-- left header -->
      <div class="flex-row flex-a-center space-x-4">
        <q-btn dense flat round icon="menu" @click="$emit('changeDrawer')" />
        <!-- 这才是标题栏 -->
        <span class="header-title-text text-xl" v-if="!$q.platform.is.mobile">Heavens</span>
        <q-input
          :dark="state.searchDarkStyle"
          dense
          @focus="state.searchDarkStyle = true"
          @blur="state.searchDarkStyle = false"
          standout
          v-model="state.search"
          type="search"
          debounce="500"
          :label="t('全局搜索')"
          input-class="text-right"
          :class="$q.platform.is.mobile ? 'w-44' : ''"
        >
          <template v-slot:append>
            <q-icon name="r_travel_explore" />
          </template>
        </q-input>
      </div>
      <q-toolbar-title></q-toolbar-title>
      <!--  right header -->
      <div class="space-x-3">
        <q-btn
          class="text-gray-700"
          size="1.0rem"
          dense
          flat
          icon="r_account_circle"
          :label="!$q.platform.is.mobile ? $store.getters['user/info']?.name || '用户' : ''"
        >
          <q-menu>
            <q-item clickable v-close-popup>
              <q-item-section icon="public">个人中心</q-item-section>
            </q-item>
            <q-item clickable v-close-popup>
              <q-item-section>个人设置</q-item-section>
            </q-item>
            <q-separator></q-separator>
            <q-item clickable v-close-popup>
              <q-item-section @click="$store.dispatch(`user/${LOGOUT}`)">退出登录</q-item-section>
            </q-item>
          </q-menu>
        </q-btn>
        <language-select></language-select>
      </div>
    </q-toolbar>
  </q-header>
</template>
<script lang="ts" setup>
import { defineComponent, reactive, toRefs } from 'vue'
import { useI18n } from 'vue-i18n'
import { LOGOUT } from 'src/store/user/actions'
import languageSelect from '../I18n/language-select.vue'

const t = useI18n().t
const state = reactive({
  search: '',
  searchDarkStyle: false,
})
</script>
<style lang="sass">
// .sticky-header-column-table
//   tr th
//     font-size: 1.1rem
//     position: sticky
//     z-index: 2
//   thead tr:first-child th
//     top: 0
//     z-index: 1
</style>
