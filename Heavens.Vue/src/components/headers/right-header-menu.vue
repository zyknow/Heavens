<template>
  <div class="space-x-1 flex flex-row items-center">
    <q-btn class="text-gray-700" size="1.0rem" dense flat icon="r_chat">
      <q-tooltip>{{ t('消息') }}</q-tooltip>
    </q-btn>
    <q-btn class="text-gray-700" size="1.0rem" dense flat icon="r_assignment">
      <q-tooltip>{{ t('任务') }}</q-tooltip>
    </q-btn>
    <q-btn class="text-gray-700" size="1.0rem" dense flat icon="r_settings" @click="toSettings">
      <q-tooltip>{{ t('设置') }}</q-tooltip>
    </q-btn>
    <q-btn
      class="text-gray-700"
      size="1.0rem"
      dense
      flat
      icon="r_account_circle"
      :label="$store.getters['user/info']?.name"
    >
      <q-tooltip>{{ t('账户') }}</q-tooltip>
      <q-menu>
        <q-item v-close-popup clickable>
          <q-item-section icon="public">个人中心</q-item-section>
        </q-item>
        <q-item v-close-popup clickable>
          <q-item-section>个人设置</q-item-section>
        </q-item>
        <q-separator />
        <q-item v-close-popup clickable>
          <q-item-section @click="$store.dispatch(`user/${LOGOUT}`)">退出登录</q-item-section>
        </q-item>
      </q-menu>
    </q-btn>
    <LanguageSelect />
  </div>
</template>
<script lang="ts">
export default {
  name: ''
}
</script>
<script lang="ts" setup>
import { TO_SETTINGS_FUN } from '@/layouts'
import { LOGOUT } from '@/store/user/actions'
import { inject } from 'vue'
import { useI18n } from 'vue-i18n'
import LanguageSelect from '../I18n/language-select.vue'
const t = useI18n().t

const toSettings = inject(TO_SETTINGS_FUN) as () => void
</script>
<style lang="sass" scoped></style>
