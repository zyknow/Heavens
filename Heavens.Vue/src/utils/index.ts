import { LocalStorage, SessionStorage } from 'quasar'
/**
 * 延时 time 毫秒
 * @param time
 * @returns
 */
export const sleepAsync = (time: number): Promise<void> => {
  return new Promise<void>(resolve => {
    // 模拟loading效果，加载太快，loading 不明显，主动加个延时 ，如不需要可删除延迟
    setTimeout(() => {
      resolve()
    }, time)
  })
}

export const ls = LocalStorage
export const ss = SessionStorage

export const isDev = process.env.NODE_ENV == 'development'
