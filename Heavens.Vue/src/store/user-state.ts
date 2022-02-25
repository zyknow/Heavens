import { computed, provide, watch, reactive } from 'vue'
import { copyByKeys, ls } from 'src/utils'
import { GetUserByToken, User } from '@/api/user'
import { TokenInfo } from './_typing'
import { GetAuthorizeJwtToken, LoginParams } from '@/api/authorize'
import { generatorDynamicRouter } from '@/router/generator-router'
import { dynamicRouter, loginRoutePath } from '@/router/routes'
import router from '@/router'
import { MenuDataItem } from '@/router/_typing'
import { RequestResult } from '@/api/_typing'

export const TOKEN_INFO = 'TOKEN_INFO'

// 角色
export const staticRoles = {
  admin: 'admin',
  user: 'user',
  test: 'test'
}
// 角色组
export const staticRoleGroups = {
  userGroup: [staticRoles.admin, staticRoles.user]
}

export interface UserState {
  /**
   * 用户信息
   */
  info: User | undefined
  /**
   * 用户路由
   */
  routers: MenuDataItem | undefined
  /**
   * token信息
   */
  tokenInfo: TokenInfo | undefined
  /**
   * 重置用户信息
   */
  resetInfo(): void

  login(info: LoginParams): Promise<RequestResult>
  logout(): void
  generateRoutesDynamic(): MenuDataItem
  getUserInfo(): Promise<boolean>
}

export const userState: UserState = reactive({
  info: undefined,
  routers: undefined,
  tokenInfo: undefined,
  resetInfo() {
    copyByKeys(userState, defaultUserState)
  },
  async login(info: LoginParams) {
    const res = await GetAuthorizeJwtToken(info)

    return res
  },
  async getUserInfo() {
    const res = await GetUserByToken()
    if (!res.succeeded) {
      userState.logout()
      return false
    }
    userState.info = res.data
    return true
  },
  generateRoutesDynamic() {
    const routers = generatorDynamicRouter(dynamicRouter, userState.info?.roles as []) as MenuDataItem
    const allowRoutes = routers || []
    // 添加到路由表
    router.addRoute(routers)
    userState.routers = allowRoutes
    return routers
  },
  logout() {
    userState.resetInfo()
    userState.tokenInfo = undefined
    router.push(loginRoutePath)
  }
})

const defaultUserState: UserState = Object.assign({}, userState)

//#region 初始化

// 初始化获取缓存token
userState.tokenInfo = ls.getItem(TOKEN_INFO) as TokenInfo

//#endregion

//#region 监听对象

// 监听tokenInfo保存缓存
watch(
  () => userState.tokenInfo,
  (v) => {
    ls.set(TOKEN_INFO, v)
  }
)
//#endregion
