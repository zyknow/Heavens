import { watch, nextTick, reactive, computed } from 'vue'
import router from 'src/router'
import { cloneDeep, last, take, takeRight, remove } from 'lodash-es'
import { IndexSign } from '@/typing'
import { RouteLocationNormalized } from 'vue-router'
import { sleepAsync } from 'src/utils'
import { notify } from 'src/utils/notify'

export type CacheKey = string
export interface CacheItem {
  name: string
  path: CacheKey
  key?: string | number
  tabTitle?: string
  tabPath?: string
  icon?: string
}

export interface MultiTabStore extends IndexSign {
  tagCaches: CacheItem[]
  current: CacheKey
  exclude: string[]
}
export interface IMultiTabAction {
  /**
   * 添加缓存
   */
  add: (item: CacheItem) => void
  /**
   * 关闭指定路径标签
   */
  close: (path: CacheKey) => void
  /**
   * 关闭指定路径左侧标签
   */
  closeLeft: (path: CacheKey) => void
  /**
   * 关闭指定路径右侧标签
   */
  closeRight: (path: CacheKey) => void
  /**
   * 关闭除指定路径之外的标签
   */
  closeOther: (path: CacheKey) => void
  /**
   * 获取缓存列表
   */
  getCaches: () => CacheItem[]
  /**
   * 刷新指定路径
   */
  refreshAsync: (path?: CacheKey | undefined) => void
}

export const MultiTabAction = (state: MultiTabStore): IMultiTabAction => {
  const removeItemsAsync = async (paths: string[]) => {
    if (state.tagCaches.length <= 1) {
      notify.warn('最后一个标签无法被关闭！')
      return
    }
    state.exclude = state.tagCaches.filter((c) => paths.includes(c.path)).map((p) => p.name)
    remove(state.tagCaches, (list) => paths.includes(list.path))
    new Promise<void>((resolve) => {
      setTimeout(() => {
        state.exclude = []
        resolve()
      })
    })
  }

  const addItem = (item: CacheItem) => {
    if (state.tagCaches.findIndex((t) => t.path == item.path) >= 0) {
      // 已存在相同标签
      return
    }
    state.tagCaches.push(cloneDeep(item))
    router.push(item.path)
  }

  const add = (item: CacheItem) => {
    addItem(item)
  }

  const close = (path: CacheKey) => {
    removeItemsAsync([path]).then(() => {
      // 移除当前tab则往后选中，否则往前选中
      if (path == state.current) {
        // state.current = last(state.tagCaches)?.path as CacheKey
        router.push(last(state.tagCaches)?.path as string)
      }
    })
  }

  const closeLeft = (path: CacheKey) => {
    const removeCount = state.tagCaches.findIndex((t) => t.path == path)
    if (removeCount > 0) {
      removeItemsAsync(take(state.tagCaches, removeCount).map((t) => t.path))
    }
  }

  const closeRight = (path: CacheKey) => {
    const removeIndex = state.tagCaches.findIndex((t) => t.path == path)
    const removeCount = state.tagCaches.length - removeIndex - 1
    if (removeCount > 0) {
      removeItemsAsync(takeRight(state.tagCaches, removeCount).map((t) => t.path))
    }
  }

  const closeOther = (path: CacheKey) => {
    removeItemsAsync(state.tagCaches.filter((t) => t.path != path).map((t) => t.path))
  }

  const getCaches = () => {
    return state.tagCaches
  }

  const refreshAsync = async (path?: CacheKey | undefined) => {
    router.push(path as string)
    state.exclude = [state.tagCaches.find((c) => c.path == path)?.name as string]
    // 刷新延时，可去除
    // await sleepAsync(500)

    // 下次页面更新时再刷新 exclude
    nextTick(() => (state.exclude = []))
  }

  const addToRouter = (to: RouteLocationNormalized) => {
    add({
      name: to.name as string,
      path: to.path || (to.redirectedFrom?.path as string),
      key: to.meta.id as number,
      tabTitle: to.meta.title as string,
      tabPath: to.fullPath,
      icon: to.meta.icon as string
    })
  }

  // 监听路由改变时，添加标签
  watch(router.currentRoute, (v, ov) => {
    addToRouter(v)
  })
  // 首次加载无法监听到，需要手动添加标签
  // addToRouter(router.currentRoute.value)

  return { add, close, closeLeft, closeRight, closeOther, getCaches, refreshAsync }
}

export const multiTabStore = reactive({
  tagCaches: [],
  current: computed(() => router.currentRoute.value.path),
  exclude: []
}) as MultiTabStore

export const multiTabAction = MultiTabAction(multiTabStore)

console.log(multiTabStore.tagCaches)
