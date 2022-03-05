import { computed, provide, watch, reactive } from 'vue'
import { copyByKeys, ls } from 'src/utils'
import { GetUserByToken, User } from '@/api/user'
import { TokenInfo } from './_typing'
import { GetAuthorizeJwtToken, LoginParams } from '@/api/authorize'
import { generatorDynamicRouter } from '@/router/generator-router'
import { dynamicRouter, loginRoutePath } from '@/router/routes'
import router from '@/router'
import { MenuDataItem } from '@/router/_typing'
import multiTabState from '@/components/multi-tab/multi-tab-state'

export const TOKEN_INFO = 'TOKEN_INFO'

/**
 * 方法
 */
const actions = {
  /**
   * 重置用户信息
   */
  resetInfo() {
    copyByKeys(userStore, defaultUserStore)
  },
  /**
   * 登陆
   * @param info
   */
  async login(info: LoginParams) {
    const res = await GetAuthorizeJwtToken(info)

    return res
  },
  /**
   * 获取用户信息
   */
  async getUserInfo() {
    const res = await GetUserByToken()
    if (!res.succeeded) {
      this.logout()
      return false
    }
    userStore.info = res.data
    return true
  },
  /**
   * 生成动态路由
   */
  generateRoutesDynamic() {
    const routers = generatorDynamicRouter(dynamicRouter, userStore.info?.roles as []) as MenuDataItem
    const allowRoutes = routers || []
    // 添加到路由表
    router.addRoute(routers)
    userStore.routers = allowRoutes
    return routers
  },
  /**
   * 登出
   */
  logout() {
    this.resetInfo()
    userStore.tokenInfo = undefined
    router.push(loginRoutePath)
    multiTabState.clear()
  }
}

/**
 * 对象参数
 */
export const userStore = reactive({
  ...actions,
  /**
   * 用户信息
   */
  info: undefined as User | undefined,
  /**
   * 用户路由
   */
  routers: undefined as MenuDataItem | undefined,
  /**
   * 用户Token
   */
  tokenInfo: undefined as TokenInfo | undefined
})

const defaultUserStore = Object.assign({}, userStore)

// 初始化获取缓存token
userStore.tokenInfo = ls.getItem(TOKEN_INFO) as TokenInfo

//#region 监听对象

// 监听tokenInfo保存缓存
watch(
  () => userStore.tokenInfo,
  (v) => {
    ls.set(TOKEN_INFO, v)
  }
)
//#endregion
