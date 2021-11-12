import { boot } from 'quasar/wrappers'
import { createI18n } from 'vue-i18n'

import messages from 'src/i18n/_index'
import { ls } from 'src/utils'
import { AppState } from 'src/store/app/state'

const i18n = createI18n({
  locale: (ls.getItem('app') as AppState)?.lang || 'zh-CN',
  messages,
})

export default boot(({ app }) => {
  // Set i18n instance on app
  app.use(i18n)
})
export { i18n }
