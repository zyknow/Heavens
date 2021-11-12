import { ls } from 'src/utils'
import { watch, computed } from 'vue'
export interface AppState {
  lang: string
  multiTabEnabled: boolean
}

export const state: AppState = {
  lang: 'zh-CN',
  multiTabEnabled: true,
}

// 根据缓存获取配置
const app = ls.getItem('app')
if (app) {
  Object.keys(app).forEach(key => (state[key] = app[key]))
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
    { deep: true },
  )
}
