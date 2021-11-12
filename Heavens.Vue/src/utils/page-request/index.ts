import { identity } from 'lodash'
import { FilterCondition, FilterOperate, ListSortType } from './enums'

/**
 * 筛选规则
 */
export interface FilterRule {
  /**
   * 获取或设置 属性名称
   */
  field?: string
  /**
   * 获取或设置 属性值
   */
  value?: any
  operate?: FilterOperate
  condition?: FilterCondition
}

export interface ListSortDirection {
  /**
   * 字段名
   */
  fieldName?: string
  sortType?: ListSortType
}

export interface PageRequest {
  /**
   * 页码
   */
  page?: number
  /**
   * 每页大小
   */
  pageSize?: number
  /**
   * 排序集合
   */
  orderConditions?: Array<ListSortDirection>
  /**
   * 查询条件组
   */
  filterRules?: Array<FilterRule>

  /**
   * 统一设置查询条件value值，用于单搜索框简单搜索
   * @param value
   */
  setAllRulesValue(value: string | number): void

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
  constructor(page: number, pageSize: number, filterGroups: Array<FilterRule> = []) {
    this.page = page
    this.pageSize = pageSize
    this.filterRules = filterGroups
    this.orderConditions = [{ fieldName: 'id', sortType: ListSortType.desc }]
  }

  page?: number = 1

  pageSize?: number = 50

  orderConditions?: Array<ListSortDirection>

  filterRules?: Array<FilterRule>

  setAllRulesValue = (value: string | number) => {
    this.filterRules?.forEach(r => (r.value = value))
  }

  setOrder = (pagination: {
    sortBy: string
    descending: boolean
    page: number
    rowsPerPage: number
    rowsNumber: number
  }) => {
    this.page = pagination.page
    this.pageSize = pagination.rowsPerPage
    if (this.orderConditions && this.orderConditions.length > 0) {
      this.orderConditions[0].fieldName = pagination.sortBy || 'id'
      this.orderConditions[0].sortType = pagination.descending
        ? ListSortType.desc
        : ListSortType.asc
    }
  }
}
