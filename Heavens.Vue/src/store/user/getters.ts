import { GetterTree } from 'vuex'
import { UserState } from './state'
import { RootState } from 'src/store/root-state'

export const getters: GetterTree<UserState, RootState> = {
  info: (state) => state.info,
  tokenInfo: (state) => state.tokenInfo,
  routers: (state) => state.routers,
  roles: (state) => state.info?.roles
}
