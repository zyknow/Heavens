<template>
  <router-view />
</template>
<script lang="ts">
export default {
  name: 'App'
}
</script>
<script lang="ts" setup>
import { notifyI18n } from './utils/notify'
import { useI18n } from 'vue-i18n'
// 设置notify I18n
notifyI18n.t = useI18n().t
</script>
