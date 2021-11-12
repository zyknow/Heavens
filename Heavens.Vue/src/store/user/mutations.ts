import { User } from 'src/api/user'
import { ls } from 'src/utils'
import { MutationTree } from 'vuex'
import { initUserState, UserState } from './state'
import { TokenInfo } from './_typing'
export const SET_LANG = 'SET_LANG'
export const SET_MULTI_TAB_ENABLED = 'SET_MULTI_TAB_ENABLED'
export const SET_TOKEN_INFO = 'SET_TOKEN_INFO'
export const SET_USER_INFO = 'SET_USER_INFO'
export const RESET_USER = 'RESET_USER'
export const SET_ROUTERS = 'SET_ROUTERS'
export const SET_REFRESH_TOKEN = 'SET_REFRESH_TOKEN'

export const TOKEN_INFO = 'tokenInfo'

export const mutations: MutationTree<UserState> = {
  [SET_TOKEN_INFO]: (state, tokenInfo: TokenInfo) => {
    state.tokenInfo = tokenInfo
    ls.set(TOKEN_INFO, tokenInfo)
  },
  [SET_USER_INFO]: (state, info: User) => {
    state.info = info
  },
  [SET_ROUTERS]: (state, routers) => {
    state.routers = routers
  },
  [RESET_USER]: state => {
    Object.assign(state, initUserState)
  },
}
