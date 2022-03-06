import { cloneDeep } from 'lodash-es'
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
  /**
   * 筛选操作方式
   */
  operate?: FilterOperate
  /**
   * 筛选条件
   */
  condition?: FilterCondition
}

export interface SortBy {
  /**
   * 字段名
   */
  field?: string
  /**
   * 排序类型
   */
  sortType?: SortType
}

//#region Request | PageRequest
abstract class BaseRequest {
  /**
   * 排序集合
   */
  sort: SortBy = {}
  /**
   * 查询条件组
   */
  filters: Filter[] = []

  /**
   * 根据 entity 设置 filters中的值
   * @param entity
   */
  setFiltersByEntity = (entity: any) => {
    switch (typeof entity) {
      case 'string':
      case 'number':
        for (const filter of this.filters) {
          filter.value = entity
          filter.field = filter?.field?.split('-')[1] || filter.field
        }
        break

      default:
        for (const filter of this.filters) {
          if (filter.field && filter.field in entity) filter.value = entity[filter.field as string]
          filter.field = filter?.field?.split('-')[1] || filter.field
        }
        break
    }

    if (typeof entity == 'string') {
    } else if (typeof entity) {
    }
  }
}

export class Request extends BaseRequest {
  constructor(filters: Filter[] = [], sortBy: SortBy = {}, limit?: number) {
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
//#endregion

//#region Query | BaseQuery
class BaseQuery<T> {
  constructor(entity: T, filters: Filter[]) {
    this.entity = entity
    this.defaultEntity = cloneDeep(entity)
    this.filters = filters
  }
  /**
   * 过滤字段实体
   */
  entity: T
  /**
   * 过滤集合
   */
  filters: Filter[]
  /**
   * 初始过滤字段实体
   */
  private defaultEntity: T

  /**
   * 重置查询实体
   */
  resetEntity() {
    this.entity = cloneDeep(this.defaultEntity)
  }

  /**
   * 设置指定field的 filter 值,没有则创建该field
   * @param field
   * @param value
   */
  addOrUpdateFilter = (filter: Filter): void => {
    if (!this.filters?.length) {
      this.filters = []
    }
    const index = this.filters?.findIndex((r) => r.field == filter.field)
    if (index < 0) {
      this.filters.push(filter)
    } else {
      this.filters[index] = filter
    }
  }

  /**
   * 清除基于field的规则
   * @param field
   * @param value
   */
  removeFilterByField = (field: string): void => {
    if (!this.filters?.length) {
      return
    }
    const index = this.filters?.findIndex((r) => r.field == field)
    if (index >= 0) {
      this.filters.splice(index, 1)
    }
  }
}

export class PageQuery<T> extends BaseQuery<T> {
  constructor(entity: T, filters: Filter[], pagination: Pagination) {
    super(entity, filters)
    this.pagination = pagination
  }
  /**
   * 分页参数
   */
  pagination: Pagination

  /**
   * 获取 PageRequest
   * @returns
   */
  toPageRequest() {
    const req = new PageRequest()
    req.filters = cloneDeep(this.filters)
    req.setFiltersByEntity(this.entity)
    req.setPagination(this.pagination)
    return req
  }
}

export class Query<T> extends BaseQuery<T> {
  constructor(entity: T, filters: Filter[], limit = 0) {
    super(entity, filters)
    this.limit = limit
  }
  /**
   * 数据限制
   */
  limit: number

  /**
   * 获取 Request
   * @returns
   */
  toRequest() {
    const req = new Request()
    req.filters = cloneDeep(this.filters)
    req.setFiltersByEntity(this.entity)
    req.limit = this.limit
    return req
  }
}
//#endregion
