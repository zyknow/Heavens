import { MutationTree } from 'vuex'
import { AppState } from './state'
export const SET_LANG = 'SET_LANG'
export const SET_MULTI_TAB_ENABLED = 'SET_MULTI_TAB_ENABLED'

export const mutations: MutationTree<AppState> = {
  [SET_LANG](state: AppState, lang: string): void {
    state.lang = lang
  },
  [SET_MULTI_TAB_ENABLED](state: AppState, enabled: boolean) {
    state.multiTabEnabled = enabled
  }
}
