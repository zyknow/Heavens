import { Module } from 'vuex'
import { state, UserState } from './state'
import { mutations } from './mutations'
import { getters } from './getters'
import { RootState } from 'src/store/root-state'
import { actions } from './actions'
export const user: Module<UserState, RootState> = {
  namespaced: true,
  state,
  mutations,
  getters,
  actions,
}
