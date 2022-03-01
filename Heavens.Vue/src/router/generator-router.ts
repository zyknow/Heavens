import { intersectionWith } from 'lodash-es'
import { dynamicRootRouter } from './routes'
import { MenuDataItem } from './_typing'

const generator = (router: MenuDataItem[], roles: string[]): MenuDataItem[] => {
  return router
    .filter(
      (r) =>
        !r.meta || !r.meta.authority || (r.meta?.authority && intersectionWith(r.meta?.authority, roles).length > 0)
    )
    .map((r) => {
      if (r.children?.length) {
        r.children = generator(r.children, roles)
      } else {
        delete r.children
      }
      return r
    })
}

export const generatorDynamicRouter = (router: MenuDataItem[], roles: string[]): MenuDataItem => {
  dynamicRootRouter.children = generator(router, roles)
  return dynamicRootRouter
}
