import { boot } from 'quasar/wrappers'
import { Quasar } from 'quasar'
import enUS from 'quasar/lang/en-US'
import zhCN from 'quasar/lang/zh-CN'
import { ls } from 'src/utils'
import { AppState } from 'src/store/app/state'

// "async" is optional;
// more info on params: https://v2.quasar.dev/quasar-cli/boot-files
export default boot(async (/* { app, router, ... } */) => {
  // something to do
  const langIso = (ls.getItem('app') as AppState)?.lang || 'zh-CN' // ... some logic to determine it (use Cookies Plugin?)

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
