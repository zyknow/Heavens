import { intersectionWith } from 'lodash-es'
import { MenuDataItem } from './_typing'

// 根级菜单
const rootRouter: MenuDataItem = {
  name: '/home',
  path: '/',
  redirect: '/home',
  meta: {
    title: '首页',
  },
  component: () => import('@/layouts/basic-layout.vue'),
  children: [] as MenuDataItem[],
}

const generator = (router: MenuDataItem[], roles: string[]): MenuDataItem[] => {
  return router
    .filter(
      r =>
        !r.meta ||
        !r.meta.authority ||
        (r.meta?.authority && intersectionWith(r.meta?.authority, roles).length > 0),
    )
    .map(r => {
      if (r.children?.length) {
        r.children = generator(r.children, roles)
      } else {
        delete r.children
      }
      return r
    })
}

export const generatorDynamicRouter = (router: MenuDataItem[], roles: string[]) => {
  rootRouter.children = generator(router, roles)
  return rootRouter
}
