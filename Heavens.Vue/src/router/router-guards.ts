import router from './index'
import { LoadingBar } from 'quasar'
import { intersectionWith } from 'lodash-es'
import { GENERATE_ROUTES_DYNAMIC, GET_USER_INFO } from 'src/store/user/actions'
import store from '@/store'
// 白名单页名
export const allowList = ['login']
// 登录路由
export const loginRoutePath = '/login'
// 默认访问路由
export const defaultRoutePath = '/home'

router.beforeEach(async (to, from) => {
  // 白名单路由直接不检查
  if (allowList.includes(to.name as string)) {
    return true
  }

  const userTokenInfo = store.getters['user/tokenInfo']
  if (!userTokenInfo) {
    if (to.fullPath !== loginRoutePath) {
      // 未登录，进入到登录页
      return {
        path: loginRoutePath,
        replace: true,
      }
    }

    return to
  }
  // 无用户信息则获取用户信息
  if (!store.getters['user/info'] && !(await store.dispatch(`user/${GET_USER_INFO}`))) {
    // 获取失败，去到登录页面
    return {
      path: loginRoutePath,
      replace: true,
    }
  }

  let allowRouter = store.getters['user/routers']
  if (!allowRouter) {
    //生成路由
    allowRouter = store.dispatch(`user/${GENERATE_ROUTES_DYNAMIC}`)
    if (allowRouter) return { ...to, replace: true }
    return false
  }
  const authority = to.meta.authority as string[]

  const userRoles = store.getters['user/roles']
  // 未设置页面权限，直接访问
  if (!authority || authority.length == 0) {
    return true
  }
  // 该用户不包含该权限
  if (userRoles && intersectionWith(authority, userRoles).length > 0) {
    return true
  }

  return false
})

// #region //*进度条

// 配置
LoadingBar.setDefaults({
  color: 'blue',
})

router.beforeEach((to, from, next) => {
  LoadingBar.start()
  next()
})
router.beforeEach(() => {
  LoadingBar.stop()
})

//#endregion
