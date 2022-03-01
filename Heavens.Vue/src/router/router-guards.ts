import { userStore } from '../store/user-store'
import router from './index'
import { LoadingBar } from 'quasar'
import { intersectionWith } from 'lodash-es'
import { allowList, loginRoutePath } from './routes'

router.beforeEach(async (to, from) => {
  // 白名单路由直接不检查
  if (allowList.includes(to.name as string)) {
    return true
  }

  const userTokenInfo = userStore.tokenInfo
  if (!userTokenInfo) {
    if (to.fullPath !== loginRoutePath) {
      // 未登录，进入到登录页
      return {
        path: loginRoutePath,
        replace: true
      }
    }

    return to
  }
  // 无用户信息则获取用户信息
  if (!userStore.info && !(await userStore.getUserInfo())) {
    // 获取失败，去到登录页面
    return {
      path: loginRoutePath,
      replace: true
    }
  }

  let allowRouter = userStore.routers
  if (!allowRouter) {
    //生成路由
    allowRouter = userStore.generateRoutesDynamic()
    if (allowRouter) return { ...to, replace: true }
    return false
  }
  const authority = to.meta.authority as string[]

  const userRoles = userStore.info?.roles
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
  color: 'blue'
})

router.beforeEach((to, from, next) => {
  LoadingBar.start()
  next()
})
router.beforeEach(() => {
  LoadingBar.stop()
})

//#endregion
