import { RouteRecordRaw, RouteMeta, RouteComponent, RouteRecordRedirectOption } from 'vue-router'
import { VNodeChild } from 'vue'

type Lazy<T> = () => Promise<T>

export type MenuRouteMeta = {
  title: string
  hidden?: boolean
  // icon?: string | VNodeChild | JSX.Element
  icon?: string
  authority?: string[]
  keepAlive?: boolean
}

export type MenuDataItem = {
  path: string
  name?: string
  children?: MenuDataItem[]
  meta?: MenuRouteMeta & RouteMeta
  redirect?: string
  component?: RouteComponent | Lazy<RouteComponent>
} & RouteRecordRaw
