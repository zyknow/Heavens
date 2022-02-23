import { ls } from 'src/utils'
import { copyByKeys } from './../../utils/index'
import { watch, computed } from 'vue'
export interface AppState {
  lang: string
  multiTabEnabled: boolean
}

export const state: AppState = {
  lang: 'zh-CN',
  multiTabEnabled: true
}

// 根据缓存获取配置
const app = ls.getItem('app') as AppState
if (app) {
  copyByKeys(state, app)
}

/**
 * 监听app state 变化进行缓存
 * @param appState
 */
export const watchAppStateToSave = (appState: AppState) => {
  watch(
    appState,
    (v, ov) => {
      ls.set('app', v)
    },
    { deep: true }
  )
}
