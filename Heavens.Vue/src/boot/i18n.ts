import { APP, appStore } from './../store/app-store'
import { ls } from './../utils/index'
import { boot } from 'quasar/wrappers'
import { createI18n } from 'vue-i18n'

import messages from 'src/i18n/_index'
import { Quasar } from 'quasar'

const lang = appStore?.lang
export const i18n = createI18n({
  locale: lang ?? 'zh-CN',
  messages
})

export default boot(async ({ app }) => {
  // Set i18n instance on app
  app.use(i18n)

  const langIso = lang || 'zh-CN' // ... some logic to determine it (use Cookies Plugin?)

  try {
    await import(
      /* webpackInclude: /(de|en-US)\.js$/ */
      'quasar/lang/' + langIso
    ).then((lang) => {
      Quasar.lang.set(lang.default)
    })
  } catch (err) {
    // Requested Quasar Language Pack does not exist,
    // let's not break the app, so catching error
  }
})
