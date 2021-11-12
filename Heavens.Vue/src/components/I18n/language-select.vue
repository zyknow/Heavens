<template>
  <q-btn
    :loading="state.loading"
    size="1.0rem"
    dense
    flat
    icon="r_public"
    class="text-gray-700"
    :label="title"
  >
    <q-menu>
      <q-item
        clickable
        v-close-popup
        v-for="(langTitle, lang, i) in supportLangs"
        :key="i"
        @click="setLang(lang)"
      >
        <q-item-section>{{ langTitle }}</q-item-section>
      </q-item>
    </q-menu>
  </q-btn>
</template>
<script lang="ts" setup>
import { supportLangs } from 'src/i18n/_index'
import { ref, defineComponent, toRefs, reactive, defineProps } from 'vue'
import { SET_LANG } from 'src/store/user/mutations'
import store from '@/store'
import { sleepAsync } from '@/utils'

const props = defineProps({
  title: String,
})

const state = reactive({
  loading: false,
})

const setLang = async (lang: string) => {
  // state.loading = true
  await store.dispatch(`app/${SET_LANG}`, lang)
  // 延时，可去除
  await sleepAsync(200)
  state.loading = false
}
</script>
<style lang="sass"></style>
