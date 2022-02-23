import { ActionTree } from 'vuex'
import { AppState } from './state'
import { SET_LANG } from './mutations'
import { RootState } from 'src/store/root-state'
import { i18n } from 'src/boot/i18n'
import { Quasar } from 'quasar'
import { notify } from 'src/utils/notify'
export const actions: ActionTree<AppState, RootState> = {
  async [SET_LANG]({ commit }, lang: string) {
    i18n.global.locale = lang
    try {
      await import(
        /* webpackInclude: /(zh-CN|en-US)\.js$/ */
        'quasar/lang/' + lang
      ).then((langObj) => {
        Quasar.lang.set(langObj.default)
        commit(SET_LANG, lang)
        location.reload()
      })
    } catch (err) {
      notify.error('切换语言失败，请重试')
      // Requested Quasar Language Pack does not exist,
      // let's not break the app, so catching error
    }
  }
}
