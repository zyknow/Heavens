<template>
  <router-view v-slot="{ Component }">
    <keep-alive v-if="appState.multiTabCacheEnabled && !multiTabState.refreshLoading" :exclude="multiTabState.exclude">
      <component :is="multiTabState.refreshLoading ? '' : Component" />
    </keep-alive>
    <component :is="multiTabState.refreshLoading ? '' : Component" v-else />
    <q-inner-loading :showing="multiTabState.refreshLoading" />
  </router-view>
</template>

<script lang="ts" setup>
import { appState } from '@/store/app-state'
import { multiTabState } from '@/components/multi-tab/multi-table-store'
</script>
