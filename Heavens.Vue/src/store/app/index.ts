import { Module } from 'vuex'
import { state, AppState } from './state'
import { mutations } from './mutations'
import { actions } from './actions'
import { getters } from './getters'
import { RootState } from '@/store/root-state'

export const app: Module<AppState, RootState> = {
  namespaced: true,
  state,
  mutations,
  actions,
  getters
}
