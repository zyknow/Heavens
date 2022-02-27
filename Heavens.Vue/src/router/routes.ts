import { staticRoles } from '@/store/user-state'
import { MenuDataItem } from './_typing'

const rotueView = import('@/layouts/route-view.vue')
const userLayout = import('@/layouts/user-layout.vue')
const mainLayout = import('@/layouts/main-layout.vue')

// 白名单页名
export const allowList = ['login']
// 登录路由
export const loginRoutePath = '/login'
// 默认访问路由
export const defaultRoutePath = '/'

// 动态路由根级菜单
export const dynamicRootRouter: MenuDataItem = {
  name: '/home',
  path: '/',
  redirect: '/home',
  meta: {
    title: '首页'
  },
  component: mainLayout,
  children: [] as MenuDataItem[]
}

// 动态路由，根据用户信息中的角色
export const dynamicRouter: MenuDataItem[] = [
  {
    path: '/home',
    name: 'Home',
    meta: { title: '主页', icon: 'r_home', authority: [staticRoles.admin, staticRoles.user] },
    component: import('pages/home/index.vue')
  },
  {
    path: '/user',
    name: 'User',
    meta: { title: '用户管理', icon: 'r_manage_accounts', authority: [staticRoles.admin] },
    component: import('pages/user/index.vue')
  },
  // 演示多级标签
  {
    path: '/user1',
    meta: { title: '用户管理下拉', icon: 'r_manage_accounts', authority: [staticRoles.admin] },
    component: rotueView,
    redirect: 'User2',
    children: [
      {
        path: '/user2',
        name: 'User2',
        meta: { title: '用户管理2', icon: 'r_manage_accounts', authority: [staticRoles.admin] },
        component: import('pages/user/index.vue')
      }
    ]
  }
]
// 静态路由
export const staticRoutes: MenuDataItem[] = [
  {
    path: '/login',
    name: 'Login',
    redirect: '/login',
    component: userLayout,
    children: [
      {
        path: '/login',
        name: 'login',
        meta: { title: '登录' },
        component: import('pages/user/login.vue')
      },
      {
        path: '/register',
        name: 'register',
        meta: { title: '注册' },
        component: import('pages/user/register.vue')
      }
    ]
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: import('pages/Error404.vue')
  }
]
