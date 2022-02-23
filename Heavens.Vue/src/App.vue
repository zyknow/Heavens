<template>
  <router-view />
</template>
<script lang="ts">
export default {
  name: 'App'
}
</script>
<script lang="ts" setup>
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { defineComponent, computed, provide, watch } from 'vue'
import { notifyI18n } from './utils/notify'
import { useI18n } from 'vue-i18n'
import store from './store'
import { watchAppStateToSave } from './store/app/state'

// 设置notify I18n
notifyI18n.t = useI18n().t
watchAppStateToSave(store.state.app)
</script>

<style></style>
