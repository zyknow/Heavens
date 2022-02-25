import { appState } from './../store/app-state'
import { boot } from 'quasar/wrappers'
import { createI18n } from 'vue-i18n'

import messages from 'src/i18n/_index'
export const i18n = createI18n({
  locale: appState.lang ?? 'zh-CN',
  messages
})

export default boot(({ app }) => {
  // Set i18n instance on app
  app.use(i18n)
})
