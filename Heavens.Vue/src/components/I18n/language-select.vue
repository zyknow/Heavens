<template>
  <q-btn size="1.0rem" dense flat icon="r_public" class="text-gray-700" :label="title">
    <q-menu>
      <q-item
        v-for="(langTitle, lang, i) in supportLangs"
        :key="i"
        v-close-popup
        clickable
        @click="appState.lang = lang"
      >
        <q-item-section>{{ langTitle }}</q-item-section>
      </q-item>
    </q-menu>
  </q-btn>
</template>
<script lang="ts" setup>
import { supportLangs } from 'src/i18n/_index'
import { useI18n } from 'vue-i18n'
import { appState } from '@/store/app-state'
const t = useI18n().t
defineProps({
  title: String
})
</script>
