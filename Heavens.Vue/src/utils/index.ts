/* eslint-disable @typescript-eslint/no-unused-vars */
import { LocalStorage, SessionStorage } from 'quasar'

export const ls = LocalStorage
export const ss = SessionStorage

export const isDev = process.env.NODE_ENV == 'development'

/**
 * key值遍历赋值资源
 * @param resource 被赋值资源
 * @param byObj 赋值资源
 * @param useResourceKey 使用被赋值资源键做遍历
 */
export const copyByKeys = (resource: any, byObj: any, useResourceKey?: boolean): void => {
  if (resource && byObj) {
    const foreachObj = useResourceKey ? resource : byObj
    Object.keys(foreachObj).forEach((k) => (resource[k] = byObj[k]))
  }
}

/**
 * 延时 time 毫秒
 * @param time
 * @returns
 */
export const sleepAsync = (time: number): Promise<void> => {
  return new Promise<void>((resolve) => {
    // 模拟loading效果，加载太快，loading 不明显，主动加个延时 ，如不需要可删除延迟
    setTimeout(() => {
      resolve()
    }, time)
  })
}
export const isInTypeByFields = (fields: string[], type: unknown): boolean => {
  for (const field of fields) {
    if (!isInType(field, type)) return false
  }
  return true
}

export const isInType = (field: string, type: any): boolean => {
  return field in type
}
