import { find } from 'lodash'
import { FilterCondition, FilterOperate, SortType } from './enums'

export type FilterValue = any
export type Pagination = {
  sortBy: string
  descending: boolean
  page: number
  rowsPerPage: number
  rowsNumber: number
  totalPages: number
}
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
  value?: FilterValue
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
abstract class BaseRequest {
  /**
   * 排序集合
   */
  sort: SortBy = {}
  /**
   * 查询条件组
   */
  filters: Array<Filter> = []
  /**
   * 清空所有规则
   */
  clearRules = (): void => {
    this.filters = []
  }
  /**
   * 统一设置查询条件value值
   * @param value
   * @param exclude 排除的字段
   */
  setAllRulesValue = (value: string | number | [], exclude?: string[]): void => {
    this.filters?.filter((f) => !exclude?.includes(f.field as never)).forEach((r) => (r.value = value))
  }
  /**
   * 设置指定field的 Rule value,没有则创建该field
   * @param field
   * @param value
   */
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
  /**
   * 清除基于field的规则
   * @param field
   * @param value
   */
  setRuleValue = (field: string, value: FilterValue): void => {
    const rule = find(this.filters, (f) => f.field == field)
    if (!rule) return
    rule.value = value
  }
  /**
   * 获取字段
   * @param field 字段名
   */
  setRuleOperate = (field: string, operate: FilterOperate): void => {
    const rule = find(this.filters, (f) => f.field == field)
    if (!rule) return
    rule.operate = operate
  }
  /**
   * 获取 Filter 根据字段名
   * @param field
   * @returns
   */
  getRuleByField = (field: string): Filter | undefined => {
    if (this.filters?.length) return undefined
    return find(this.filters, (f) => f.field == field) as Filter
  }
  /**
   * 根据 QueryForm 设置 查询 value
   * @param queryForm
   */
  setRuleByField = (queryForm: any) => {
    for (const rule of this.filters) {
      if (rule.field && rule.field in queryForm) rule.value = queryForm[rule.field as string]
      rule.field = rule?.field?.split('.')[1] || rule.field
    }
  }
  /**
   * 清除基于field的规则
   * @param field
   * @param value
   */
  removeRules = (field: string): void => {
    if (!this.filters?.length) {
      return
    }
    const index = this.filters?.findIndex((r) => r.field == field)
    if (index >= 0) {
      this.filters.splice(index, 1)
    }
  }
}

export class Request extends BaseRequest {
  constructor(filters: Filter[], sortBy: SortBy = {}, limit?: number) {
    super()
    this.filters = filters
    if (sortBy?.field) this.sort = sortBy
    else this.sort = { field: 'id', sortType: SortType.desc }
  }

  /**
   * 数据限制
   */
  limit?: number
}

export class PageRequest extends BaseRequest {
  constructor(filters: Filter[] = [], sortBy: SortBy = {}) {
    super()
    this.filters = filters

    if (sortBy?.field) this.sort = sortBy
    else this.sort = { field: 'id', sortType: SortType.desc }
  }
  /**
   * 页码
   */
  page: number = 1
  /**
   * 每页大小
   */
  pageSize: number = 50
  /**
   * 设置分页参数
   * @param pagination 分页数据
   */
  setPagination = (pagination: Pagination): void => {
    this.page = pagination.page
    this.pageSize = pagination.rowsPerPage
    if (this.sort?.field) {
      this.sort.field = pagination.sortBy || 'id'
      this.sort.sortType = pagination.descending ? SortType.desc : SortType.asc
    }
  }
}
