import router from '@/router'
import { computed, provide, watch, reactive } from 'vue'
import { copyByKeys, ls } from 'src/utils'
import { Quasar } from 'quasar'
import { notify } from 'src/utils/notify'
import { i18n } from '@/boot/i18n'
import { multiTabState } from '@/components/multi-tab/multi-tab-state'
import { GetCoreVersion } from '@/api/system'
export const APP = 'APP'

const actions = {}

export const appStore = reactive({
  ...actions,
  /**
   * 语言
   */
  lang: 'zh-CN',
  /**
   * 多标签栏显示
   */
  multiTabVisible: true,
  /**
   * 多标签栏缓存启用
   */
  multiTabCacheEnabled: false,

  version: ''
})

const init = async () => {
  // 初始化 根据缓存获取配置
  const app = ls.getItem(APP)
  if (app) {
    copyByKeys(appStore, app)
  }

  // 初始化 version
  const versionRes = await GetCoreVersion()
  versionRes.notifyOnErr()
  if (versionRes.succeeded) {
    appStore.version = versionRes.data
  }
}

init()

//#region 监听对象

// 监听 lang 变化
watch(
  () => appStore.lang,
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

watch(
  () => appStore.multiTabVisible,
  (v) => {
    if (!v) {
      appStore.multiTabCacheEnabled = v
      multiTabState.clear()
    } else {
      multiTabState.addByRouter(router.currentRoute.value)
    }
  }
)

// 监听app state 变化进行缓存
watch(
  appStore,
  (app) => {
    ls.set(APP, app)
  },
  { deep: true }
)

//#endregion
