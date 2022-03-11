import { watch } from 'vue'
import { Filter } from './typing'
import { IndexSign } from '@/typing'
import { cloneDeep, last } from 'lodash-es'
import { dateFormatFull } from '../date-util'
import { enumToOption } from '../enum'
import { PageRequest, DataRequest } from './request'
import { FieldOption, QueryModel, Condition, FieldType, Pagination, Operate } from './typing'
import { ls } from '..'

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
      this.filters = [this.fieldOptions[0]]
    } else if (this.mode == QueryModel.advanced) {
      const { fieldOptions } = this
      const filters: FieldOption[] = []
      for (const item of fieldOptions) {
        if (item.type == FieldType.date) {
          filters.push({
            ...item,
            field: `start-${item.field}`,
            label: `起始${item.label}`,
            value: undefined,
            operate: Operate.greaterOrEqual
          })
          filters.push({
            ...item,
            field: `end-${item.field}`,
            label: `结束${item.label}`,
            operate: Operate.lessOrEqual,
            value: dateFormatFull(Date.now())
          })
        } else {
          filters.push({ ...item })
        }
      }
      this.filters = [
        ...filters
        // ...filters.filter((f) => this.filters.findIndex((qf) => qf.field == f.field && qf.label == f.label) == -1)
      ]
      this.setAllCondition(Condition.and)
    } else if (this.mode == QueryModel.easy) {
      this.filters = cloneDeep(this.fieldOptions.filter((f) => f.easy))
      this.setAllCondition(Condition.or)
    }
  }
}

export class PageQuery extends BaseQuery {
  /**
   *
   * @param fieldOptions
   * @param pagination
   */
  constructor(fieldOptions: FieldOption[], pagination: Pagination | string = '') {
    super(fieldOptions)

    if (typeof pagination == 'string') {
      this.rowsPerPageStr = pagination
      this.pagination = {
        sortBy: 'id',
        descending: false,
        page: 1,
        rowsPerPage: ls.getItem<number>(pagination) || 10,
        rowsNumber: 1,
        totalPages: 1
      }
    } else {
      this.pagination = pagination
    }
  }

  /**
   * 分页参数
   */
  pagination: Pagination

  private rowsPerPageStr?: string

  /**
   * 获取 PageRequest
   * @returns
   */
  toPageRequest() {
    const req = new PageRequest()
    const filters = this.filters.filter((f) => {
      if (f.value?.toString()?.length) {
        if (f.field?.includes('-')) f.field = last(f.field.split('-'))
        return true
      }
      return false
    })
    req.filters = filters as any
    req.setPagination(this.pagination)
    return req
  }

  saveRowsPerPage() {
    if (this.rowsPerPageStr) ls.set(this.rowsPerPageStr, this.pagination.rowsPerPage)
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
