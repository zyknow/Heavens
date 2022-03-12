import { watch } from 'vue'
import { Filter } from './typing'
import { IndexSign } from '@/typing'
import { cloneDeep, last } from 'lodash-es'
import { dateFormatFull } from '../date-util'
import { enumToOption } from '../enum'
import { PageRequest, DataRequest } from './request'
import { FieldOption, QueryModel, Condition, FieldType, Pagination, Operate } from './typing'
import { ls } from '..'
import router from '@/router'

export class BaseQuery {
  constructor(fieldOptions: FieldOption[]) {
    this.fieldOptions = fieldOptions
  }
  /**
   * 过滤集合
   */
  filters: FieldOption[] = []

  fieldOptions: FieldOption[] = []

  mode: QueryModel = QueryModel.easy

  setAllCondition(condition: Condition) {
    for (const filter of this.filters) {
      filter.condition = condition
    }
  }
  /**
   * 设置指定field的 filter 值,没有则创建该field
   * @param field
   * @param value
   */
  addOrUpdateFilter = (filter: FieldOption): void => {
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

  setValueByField = (filter: FieldOption): void => {
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

  setAllFilterValue = (value: any): void => {
    for (const filter of this.filters) {
      filter.value = value
    }
  }

  reset = (): void => {
    if (this.mode == QueryModel.custom) {
      this.filters = [{ ...this.fieldOptions[0] }]
      this.filters.splice(0, this.filters.length)
      // this.filters = [this.fieldOptions[0]]
    } else if (this.mode == QueryModel.advanced) {
      const { fieldOptions } = this
      this.filters = []
      for (const item of fieldOptions) {
        if (item.type == FieldType.date || item.type == FieldType.numberBetween) {
          this.filters.push({
            ...item,
            field: `start-${item.field}`,
            label: `起始${item.label}`,
            value: undefined,
            operate: Operate.greaterOrEqual
          })
          this.filters.push({
            ...item,
            field: `end-${item.field}`,
            label: `结束${item.label}`,
            operate: Operate.lessOrEqual,
            value: item.type == FieldType.date ? dateFormatFull(Date.now()) : undefined
          })
        } else {
          this.filters.push({ ...item })
        }
      }
      this.setAllCondition(Condition.and)
    } else if (this.mode == QueryModel.easy) {
      this.filters = cloneDeep(this.fieldOptions.filter((f) => f.easy))
      this.setAllCondition(Condition.or)
    }
  }
}

const PAGE_QUERY_TABLE_OPTIONS = 'PAGE_QUERY_TABLE_OPTIONS'
const defaultPerPage = 10
const tableOptions: { routerName: string; rowsPerPage: number; defaultVisibleColumns: any[] }[] =
  ls.getItem(PAGE_QUERY_TABLE_OPTIONS) || []
const saveTableOption = () => {
  ls.set(PAGE_QUERY_TABLE_OPTIONS, tableOptions)
}
export class PageQuery<T> extends BaseQuery {
  //#region 构造函数
  /**
   *
   * @param fieldOptions
   * @param pagination
   */
  constructor(fieldOptions: FieldOption[], pagination?: Pagination) {
    super(fieldOptions)

    this.routerName = router.currentRoute.value.name as string
    let tableOption = tableOptions.find((p) => p.routerName == this.routerName)
    this.pagination = pagination || {
      sortBy: '',
      descending: false,
      page: 1,
      rowsPerPage: tableOption?.rowsPerPage || defaultPerPage,
      rowsNumber: 1,
      totalPages: 1
    }

    if (!tableOption) {
      tableOption = {
        routerName: this.routerName,
        rowsPerPage: this.pagination.rowsPerPage,
        defaultVisibleColumns: fieldOptions.filter((p) => p.defaultVisibleColumn != false).map((p) => p.field)
      }
      tableOptions.push(tableOption)
    }

    this.visibleColumns = tableOption.defaultVisibleColumns

    // 设置 table 中的 columns 参数
    this.columns = fieldOptions.map((p) => {
      return {
        ...p,
        ...{ sortable: true, align: 'center' },
        ...p.columns,
        name: p.field
      }
    })

    this.fieldOptions = this.fieldOptions.filter((p) => !p.excludeQuery)
  }
  //#endregion

  /**
   * 分页参数
   */
  pagination: Pagination

  /**
   * 显示的列
   */
  visibleColumns: string[]

  /**
   * loading
   */
  loading: boolean = false

  /**
   * 数据源
   */
  data: T[] = []

  /**
   * 选中的数据
   */
  selected: T[] = []

  /**
   * ? 仅用于table中的columns参数
   */
  columns: any[]

  /**
   * 当前路由名
   */
  private routerName: string

  /**
   * 上一次请求的字符串，用于对比不同时设置成第一页
   */
  private beforeRequestFilterStr?: string

  /**
   * 获取 PageRequest
   * @param inputFilters
   * @returns
   */
  toPageRequest(inputFilters?: Filter[]) {
    const req = new PageRequest()

    const filters =
      inputFilters ||
      this.filters.filter((f) => {
        if (f.value?.toString()?.length) {
          if (f.field?.includes('-')) f.field = last(f.field.split('-'))
          return true
        }
        return false
      })

    const filterStr = filters.toString()
    // 请求字符串发现变动，直接返回到第一页
    if (this.beforeRequestFilterStr != filterStr) this.pagination.page = 1
    this.beforeRequestFilterStr = filterStr

    req.filters = filters as Filter[]
    req.setPagination(this.pagination)
    return req
  }

  toDataQuery(limit = 0) {
    return new DataQuery(this.fieldOptions, limit)
  }
  /**
   * 保存每页行数
   */
  saveOption() {
    const tableOption = tableOptions.find((p) => p.routerName == this.routerName)
    if (tableOption) {
      tableOption.rowsPerPage = this.pagination.rowsPerPage
      tableOption.defaultVisibleColumns = this.visibleColumns
    }
    saveTableOption()
  }
}

export class DataQuery extends BaseQuery {
  constructor(fieldOptions: FieldOption[], limit = 0) {
    super(fieldOptions)
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
    const req = new DataRequest()
    req.filters = this.filters as Filter[]
    req.limit = this.limit
    return req
  }
}

const operateOption = enumToOption(Operate)

// 缓存，用于优化 getOperatesByFieldType 返回
const operateCache: IndexSign = {}
/**
 * 根据 type 获取 可以使用的 Operate
 * @param type
 * @returns
 */
export const getOperatesByFieldType = (type?: FieldType): { label: string; value: Operate }[] => {
  if (type && type in operateCache) return operateCache[type]
  let operates
  switch (type) {
    case FieldType.date:
    case FieldType.number:
    case FieldType.numberBetween:
      operates = [
        Operate.equal,
        Operate.notEqual,
        Operate.less,
        Operate.lessOrEqual,
        Operate.greater,
        Operate.greaterOrEqual
      ]
      break
    case FieldType.boolCheckBox:
    case FieldType.boolSelect:
      operates = [Operate.equal, Operate.notEqual]
      break
    case FieldType.text:
      operates = [
        Operate.equal,
        Operate.notEqual,
        Operate.startsWith,
        Operate.endsWith,
        Operate.contains,
        Operate.notContains,
        Operate.in
      ]
      break
    case FieldType.select:
      operates = [Operate.in]
      break
    default:
      return operateOption
  }

  const res = operates.map((v) => {
    return {
      ...operateOption.find((p) => p.value == v)
    }
  })

  if (type && !(type in operateCache)) operateCache[type] = res

  return operateCache[type]
}
