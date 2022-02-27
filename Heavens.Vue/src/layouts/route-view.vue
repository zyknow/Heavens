<template>
  <router-view v-slot="{ Component }">
    <keep-alive v-if="appState.multiTabCacheEnabled" :exclude="multiTabState.exclude">
      <component :is="state.refreshLoading ? '' : Component" />
    </keep-alive>
    <component :is="state.refreshLoading ? '' : Component" v-else />
    <q-inner-loading :showing="state.refreshLoading" />
  </router-view>
</template>

<script lang="ts" setup>
import { ref, defineComponent, provide, InjectionKey, readonly, reactive, toRefs, computed, watch } from 'vue'
import { useQuasar } from 'quasar'
import { appState } from '@/store/app-state'
import { multiTabState } from '@/components/multi-tab/multi-table-store'
const $q = useQuasar()
const state = reactive({
  refreshLoading: false,
  cachingEnabledLoading: false
})
</script>
