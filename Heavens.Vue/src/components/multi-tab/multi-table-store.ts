import { watch, nextTick, reactive, computed } from 'vue'
import router from 'src/router'
import { cloneDeep, last, take, takeRight, remove } from 'lodash-es'
import { IndexSign } from '@/typing'
import { RouteLocationNormalized } from 'vue-router'
import { notify } from 'src/utils/notify'

export type CacheKey = string
export interface CacheItem {
  name: string
  tabTitle?: string
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
  close: (name: CacheKey) => void
  /**
   * 关闭指定路径左侧标签
   */
  closeLeft: (name: CacheKey) => void
  /**
   * 关闭指定路径右侧标签
   */
  closeRight: (name: CacheKey) => void
  /**
   * 关闭除指定路径之外的标签
   */
  closeOther: (name: CacheKey) => void
  /**
   * 获取缓存列表
   */
  getCaches: () => CacheItem[]
  /**
   * 刷新指定路径
   */
  refreshAsync: (name?: CacheKey | undefined) => void
}

export const MultiTabAction = (state: MultiTabStore): IMultiTabAction => {
  const removeItemsAsync = async (tabNames: string[]) => {
    if (state.tagCaches.length <= 1) {
      notify.warn('最后一个标签无法被关闭！')
      return
    }
    state.exclude = state.tagCaches.filter((c) => tabNames.includes(c.name)).map((p) => p.name)
    remove(state.tagCaches, (list) => tabNames.includes(list.name))
    new Promise<void>((resolve) => {
      setTimeout(() => {
        state.exclude = []
        resolve()
      })
    })
  }

  const addItem = (item: CacheItem) => {
    if (state.tagCaches.findIndex((t) => t.name == item.name) >= 0) {
      // 已存在相同标签
      return
    }
    state.tagCaches.push(cloneDeep(item))
    router.push(item.name)
  }

  const add = (item: CacheItem) => {
    addItem(item)
  }

  const close = (name: CacheKey) => {
    removeItemsAsync([name]).then(() => {
      // 移除当前tab则往后选中，否则往前选中
      if (name == state.current) {
        // state.current = last(state.tagCaches)?.name as CacheKey
        router.push(last(state.tagCaches)?.name as string)
      }
    })
  }

  const closeLeft = (name: CacheKey) => {
    const removeCount = state.tagCaches.findIndex((t) => t.name == name)
    if (removeCount > 0) {
      removeItemsAsync(take(state.tagCaches, removeCount).map((t) => t.name))
    }
  }

  const closeRight = (name: CacheKey) => {
    const removeIndex = state.tagCaches.findIndex((t) => t.name == name)
    const removeCount = state.tagCaches.length - removeIndex - 1
    if (removeCount > 0) {
      removeItemsAsync(takeRight(state.tagCaches, removeCount).map((t) => t.name))
    }
  }

  const closeOther = (name: CacheKey) => {
    removeItemsAsync(state.tagCaches.filter((t) => t.name != name).map((t) => t.name))
  }

  const getCaches = () => {
    return state.tagCaches
  }

  const refreshAsync = async (name?: CacheKey | undefined) => {
    router.push(name as string)
    state.exclude = [state.tagCaches.find((c) => c.name == name)?.name as string]
    // 刷新延时，可去除
    // await sleepAsync(500)

    // 下次页面更新时再刷新 exclude
    nextTick(() => (state.exclude = []))
  }

  const addToRouter = (to: RouteLocationNormalized) => {
    add({
      name: to.name as string,
      tabTitle: to.meta.title as string,
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

export const multiTabState = reactive({
  tagCaches: [],
  current: computed(() => router.currentRoute.value.name),
  exclude: []
}) as MultiTabStore

export const multiTabAction = MultiTabAction(multiTabState)
