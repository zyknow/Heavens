import { FilterCondition, FilterOperate, SortType } from './enums'

/**
 * 筛选规则
 */
export interface Filter {
  /**
   * 获取或设置 属性名称
   */
  field?: string
  /**
   * 获取或设置 属性值
   */
  value?: number | string | number[] | string[] | Date
  operate?: FilterOperate
  condition?: FilterCondition
}

export interface SortBy {
  /**
   * 字段名
   */
  field?: string
  sortType?: SortType
}

export interface PageRequest {
  /**
   * 页码
   */
  page: number
  /**
   * 每页大小
   */
  pageSize: number
  /**
   * 排序集合
   */
  sort: SortBy
  /**
   * 查询条件组
   */
  filters: Array<Filter>

  /**
   * 统一设置查询条件value值
   * @param value
   * @param exclude 排除的字段
   */
  setAllRulesValue(value: string | number | [], exclude?: string[]): void

  /**
   * 设置指定field的 Rule value,没有则创建该field
   * @param field
   * @param value
   */
  setRule(rule: Filter): void

  /**
   * 清除基于field的规则
   * @param field
   * @param value
   */
  removeRules(field: string): void

  /**
   * 清空所有规则
   */
  clearRules(): void
  /**
   * 设置分页参数
   * @param pagination 分页数据
   */
  setOrder(pagination: {
    sortBy: string
    descending: boolean
    page: number
    rowsPerPage: number
    rowsNumber: number
  }): void
}

export class PageRequest implements PageRequest {
  constructor(page: number, pageSize: number, filters: Filter[], sortBy: SortBy = {}) {
    this.page = page
    this.pageSize = pageSize
    this.filters = filters

    if (sortBy?.field) this.sort = sortBy
    else this.sort = { field: 'id', sortType: SortType.desc }
  }

  page = 1

  pageSize = 50

  sort: SortBy

  filters: Array<Filter>

  clearRules = (): void => {
    this.filters = []
  }

  setAllRulesValue = (value: string | number | [], exclude?: string[]): void => {
    this.filters?.filter((f) => !exclude?.includes(f.field as never)).forEach((r) => (r.value = value))
  }

  setRule = (rule: Filter): void => {
    if (!this.filters?.length) {
      this.filters = []
    }
    const index = this.filters?.findIndex((r) => r.field == rule.field)
    if (index < 0) {
      this.filters.push(rule)
    } else {
      this.filters[index] = rule
    }
  }

  removeRules = (field: string): void => {
    if (!this.filters?.length) {
      return
    }
    const index = this.filters?.findIndex((r) => r.field == field)
    if (index >= 0) {
      this.filters.splice(index, 1)
    }
  }

  setOrder = (pagination: {
    sortBy: string
    descending: boolean
    page: number
    rowsPerPage: number
    rowsNumber: number
  }): void => {
    this.page = pagination.page
    this.pageSize = pagination.rowsPerPage
    if (this.sort?.field) {
      this.sort.field = pagination.sortBy || 'id'
      this.sort.sortType = pagination.descending ? SortType.desc : SortType.asc
    }
  }
}
