import { SortBy, SortType, Pagination, Filter } from './typing'

export abstract class BaseRequest {
  /**
   * 排序集合
   */
  sort: SortBy = {}
  /**
   * 查询条件组
   */
  filters: Filter[] = []
}

export class DataRequest extends BaseRequest {
  constructor(filters: Filter[] = [], sortBy?: SortBy, limit?: number) {
    super()
    this.filters = filters
    if (sortBy?.field) this.sort = sortBy
  }

  /**
   * 数据限制
   */
  limit?: number
}

export class PageRequest extends BaseRequest {
  constructor(filters: Filter[] = []) {
    super()
    this.filters = filters
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
    this.sort.field = pagination.sortBy
    this.sort.sortType = pagination.descending ? SortType.desc : SortType.asc
  }
}
