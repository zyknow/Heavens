import { computed, provide, watch, reactive } from 'vue'
import { copyByKeys, ls } from 'src/utils'
import { ActionTree } from 'vuex'
import { Quasar } from 'quasar'
import { notify } from 'src/utils/notify'
import { i18n } from '@/boot/i18n'
export const APP = 'APP'

export interface AppState {
  /**
   * 语言
   */
  lang: string
  /**
   * 多标签栏
   */
  multiTabEnabled: boolean
}

export const appState: AppState = reactive({
  lang: 'zh-CN',
  multiTabEnabled: true
})

//#region 初始化(必须放在 watch 前面)

// 根据缓存获取配置
const app = ls.getItem(APP) as AppState
if (app) {
  copyByKeys(appState, app)
}

//#endregion

//#region 监听对象

// 监听 lang 变化
watch(
  () => appState.lang,
  async (lang) => {
    i18n.global.locale = lang as any
    try {
      await import(
        /* webpackInclude: /(zh-CN|en-US)\.js$/ */
        'quasar/lang/' + lang
      ).then((langObj) => {
        Quasar.lang.set(langObj.default)
        location.reload()
      })
    } catch (err) {
      notify.error('切换语言失败，请重试')
      // Requested Quasar Language Pack does not exist,
      // let's not break the app, so catching error
    }
  }
)

// 监听app state 变化进行缓存
watch(
  appState,
  (app) => {
    ls.set(APP, app)
  },
  { deep: true }
)

//#endregion
