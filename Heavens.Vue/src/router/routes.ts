import { staticRoles } from '@/store/user-state'
import { cloneDeep } from 'lodash-es'
import userLayout from 'src/layouts/user-layout.vue'
import { MenuDataItem } from './_typing'

// 白名单页名
export const allowList = ['login']
// 登录路由
export const loginRoutePath = '/login'
// 默认访问路由
export const defaultRoutePath = '/home'

// 动态路由，根据用户信息中的角色
export const dynamicRouter: MenuDataItem[] = [
  {
    path: '/home',
    name: 'home',
    meta: { title: '主页', icon: 'r_home', authority: [staticRoles.admin, staticRoles.user] },
    component: import('pages/home/index.vue')
  },
  {
    path: '/user',
    name: 'user',
    meta: { title: '用户管理', icon: 'r_manage_accounts', authority: [staticRoles.admin] },
    component: import('pages/user/index.vue')
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
