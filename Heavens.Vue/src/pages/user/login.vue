<template>
  <div style="width: 400px" class="p-10">
    <q-form class="space-y-4" @submit="submit">
      <q-input v-model="loginForm.account" outlined :label="t('账号')" lazy-rules>
        <template #prepend>
          <q-icon name="r_person" />
        </template>
      </q-input>
      <q-input
        v-model="loginForm.passwd"
        lazy-rules
        :type="state.isPwd ? 'password' : 'text'"
        outlined
        :label="t('密码')"
      >
        <template #prepend>
          <q-icon name="r_lock" />
        </template>
        <template #append>
          <q-icon
            :name="state.isPwd ? 'visibility_off' : 'visibility'"
            class="cursor-pointer"
            @click="state.isPwd = !state.isPwd"
          />
        </template>
      </q-input>
      <div class="flex flex-row justify-between">
        <q-checkbox v-model="loginForm.keepAlive" dense keep-color :label="t('保持登录')" color="blue" />
        <a class="cursor-pointer text-primary">{{ t('忘记密码') }}?</a>
      </div>

      <q-btn :loading="state.loading" type="submit" class="w-full" color="primary" :label="t('登录')" />
      <div class="flex flex-row justify-between">
        <div class>
          <language-select :title="t('语言')" />
        </div>
      </div>
    </q-form>
  </div>
</template>
<script lang="ts" setup>
import { isDev } from 'src/utils'
import { LoginParams } from '@/api/authorize'
import { reactive } from 'vue'
import { useI18n } from 'vue-i18n'
import { RequestResult } from 'src/api/_typing'
import { notify } from 'src/utils/notify'
import router from 'src/router'
import LanguageSelect from '@/components/I18n/language-select.vue'
import { userStore } from '@/store/user-store'
import { defaultRoutePath } from '@/router/routes'

const t = useI18n().t
const state = reactive({
  loading: false,
  isPwd: true
})

const loginForm = reactive<LoginParams>({
  account: isDev ? 'admin' : '',
  passwd: isDev ? '123456' : '',
  keepAlive: false
} as LoginParams)

const submit = async () => {
  state.loading = true
  const res = await userStore.login(loginForm)
  state.loading = false
  notify.responseOnErr(res)
  if (res?.succeeded) {
    router.push(defaultRoutePath)
  }
}
</script>
<style lang="sass"></style>
