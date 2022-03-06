<template>
  <div class="fullscreen bg-blue text-white text-center q-pa-md flex flex-center">
    <div>
      <div style="font-size: 30vh">404</div>

      <div class="text-h2" style="opacity: 0.4">{{ t('导航错误，不存在该路由') }}</div>

      <q-btn class="q-mt-xl" color="white" text-color="blue" unelevated to="/" :label="t('返回主页')" no-caps />
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useI18n } from 'vue-i18n'

const t = useI18n().t
</script>
