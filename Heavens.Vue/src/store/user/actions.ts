import { ActionTree } from 'vuex'
import { UserState } from './state'
import { RootState } from 'src/store/root-state'
import { GetAuthorizeJwtToken, LoginParams } from '@/api/authorize'
import router from 'src/router'
import { loginRoutePath } from 'src/router/router-guards'
import { GetUserByToken } from 'src/api/user'
import { dynamicRouter } from 'src/router/routes'
import { generatorDynamicRouter } from '@/router/generator-router'
import type { RouteRecordRaw } from 'vue-router'
import { RESET_USER, SET_USER_INFO, SET_ROUTERS, SET_TOKEN_INFO } from './mutations'

export const LOGOUT = 'LOGOUT'
export const LOGIN = 'LOGIN'
export const GET_USER_INFO = 'GET_USER_INFO'
export const GENERATE_ROUTES_DYNAMIC = 'GENERATE_ROUTES_DYNAMIC'

export const actions: ActionTree<UserState, RootState> = {
  async [LOGIN]({}, info: LoginParams) {
    const res = await GetAuthorizeJwtToken(info)
    return res
  },
  async [GET_USER_INFO]({ commit, dispatch }) {
    const res = await GetUserByToken()
    if (!res.succeeded) {
      dispatch(LOGOUT)
      return false
    }
    commit(SET_USER_INFO, res.data)
    return true
  },
  [GENERATE_ROUTES_DYNAMIC]({ state, commit }) {
    const routers = generatorDynamicRouter(dynamicRouter, state.info.roles) as RouteRecordRaw
    const allowRoutes = routers || []
    // 添加到路由表
    router.addRoute(routers)
    commit(SET_ROUTERS, allowRoutes)
    return routers
  },
  [LOGOUT]({ commit }) {
    commit(RESET_USER)
    commit(SET_TOKEN_INFO, undefined)
    router.push(loginRoutePath)
  }
}
