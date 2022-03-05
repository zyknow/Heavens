import { watch, nextTick, reactive, computed } from 'vue'
import router from 'src/router'
import { cloneDeep, last, take, takeRight, remove } from 'lodash-es'
import { IndexSign } from '@/typing'
import { RouteLocationNormalized } from 'vue-router'
import { notify } from 'src/utils/notify'
import { sleepAsync } from '@/utils'

export type CacheKey = string
export interface CacheItem {
  name: string
  tabTitle?: string
  icon?: string
}

const privateActions = {
  async removeItemsAsync(tabNames: string[]) {
    if (multiTabState.tagCaches.length <= 1) {
      notify.warn('最后一个标签无法被关闭！')
      return
    }
    multiTabState.exclude = multiTabState.tagCaches.filter((c) => tabNames.includes(c.name)).map((p) => p.name)
    remove(multiTabState.tagCaches, (list) => tabNames.includes(list.name))
    new Promise<void>((resolve) => {
      setTimeout(() => {
        multiTabState.exclude = []
        resolve()
      })
    })
  },

  addItem(item: CacheItem) {
    if (multiTabState.tagCaches.findIndex((t) => t.name == item.name) >= 0) {
      // 已存在相同标签
      return
    }
    multiTabState.tagCaches.push(cloneDeep(item))
    router.push(item.name)
  },
  /**
   * 添加缓存
   */
  add(item: CacheItem) {
    privateActions.addItem(item)
  }
}

const actions = {
  /**
   * 关闭指定路径标签
   */
  close(name: CacheKey) {
    privateActions.removeItemsAsync([name]).then(() => {
      // 移除当前tab则往后选中，否则往前选中
      if (name == multiTabState.current) {
        // multiTabState.current = last(multiTabState.tagCaches)?.name as CacheKey
        router.push(last(multiTabState.tagCaches)?.name as string)
      }
    })
  },
  /**
   * 关闭指定路径左侧标签
   */
  closeLeft(name: CacheKey) {
    const removeCount = multiTabState.tagCaches.findIndex((t) => t.name == name)
    if (removeCount > 0) {
      privateActions.removeItemsAsync(take(multiTabState.tagCaches, removeCount).map((t) => t.name))
    }
  },
  /**
   * 关闭指定路径右侧标签
   */
  closeRight(name: CacheKey) {
    const removeIndex = multiTabState.tagCaches.findIndex((t) => t.name == name)
    const removeCount = multiTabState.tagCaches.length - removeIndex - 1
    if (removeCount > 0) {
      privateActions.removeItemsAsync(takeRight(multiTabState.tagCaches, removeCount).map((t) => t.name))
    }
  },
  /**
   * 关闭除指定路径之外的标签
   */
  closeOther(name: CacheKey) {
    privateActions.removeItemsAsync(multiTabState.tagCaches.filter((t) => t.name != name).map((t) => t.name))
  },
  /**
   * 刷新指定路径
   */
  async refreshAsync(name: CacheKey) {
    multiTabState.exclude = [multiTabState.tagCaches.find((c) => c.name == name)?.name as string]

    // 刷新延时，可去除
    // await sleepAsync(500)

    // 下次页面更新时再刷新 exclude
    nextTick(() => (multiTabState.exclude = []))
  },
  /**
   * 添加
   * @param to
   */
  addByRouter(to: RouteLocationNormalized) {
    if (to.meta?.keepAlive)
      privateActions.add({
        name: to.name as string,
        tabTitle: to.meta.title as string,
        icon: to.meta.icon as string
      })
  },
  /**
   * 清空所有缓存
   */
  clear() {
    multiTabState.loading = true
    multiTabState.exclude = cloneDeep(multiTabState.tagCaches.map((t) => t.name))
    multiTabState.tagCaches = []
    // 下次页面更新时再刷新 exclude
    nextTick(() => (multiTabState.exclude = []))
    multiTabState.loading = false
  }
}

export const multiTabState = reactive({
  ...actions,
  tagCaches: [] as CacheItem[],
  current: computed(() => router.currentRoute.value.name),
  exclude: [] as string[],
  loading: false
})

// 监听路由改变时，添加标签
watch(router.currentRoute, (v, ov) => {
  actions.addByRouter(v)
})
