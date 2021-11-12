<template>
  <div v-for="(router, index) in routers" :key="index">
    <q-item
      v-if="!router.children"
      clickable
      v-ripple
      :to="router.path"
      class="text-gray-700"
      exact-active-class="bg-blue-100"
      active-class=" text-primary"
    >
      <q-item-section avatar>
        <q-icon size="1.5rem" :name="router.meta?.icon" />
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
      class="text-gray-700"
    >
      <nav-right-menu-item :routers="router.children"></nav-right-menu-item>
    </q-expansion-item>
  </div>
</template>
<script lang="ts" setup>
import { MenuDataItem } from '@/router/_typing'
import { ref, defineComponent, toRefs, reactive, PropType, defineProps } from 'vue'
import { useI18n } from 'vue-i18n'
const props = defineProps({
  routers: Object as PropType<MenuDataItem[]>,
})

const t = useI18n().t
</script>
<style lang="sass"></style>
