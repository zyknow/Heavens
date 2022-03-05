<template>
  <router-view v-slot="{ Component }">
    <keep-alive v-if="appStore.multiTabCacheEnabled && !multiTabState.loading" :exclude="multiTabState.exclude">
      <component :is="multiTabState.loading ? '' : Component" />
    </keep-alive>
    <component :is="multiTabState.loading ? '' : Component" v-else />

    <q-inner-loading :showing="multiTabState.loading" />
  </router-view>
</template>

<script lang="ts" setup>
import { multiTabState } from '@/components/multi-tab/multi-tab-state'
import { appStore } from '@/store/app-store'
</script>
