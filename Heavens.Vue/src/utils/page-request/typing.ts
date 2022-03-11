import { OptionType } from './../enum'
/**
 * 筛选条件
 */
export enum Condition {
  /**
   * 或者
   */
  or = 1,
  /**
   * 并且
   */
  and = 2
}

/**
 * 筛选操作方式
 */
export enum Operate {
  /**
   * 等于
   */
  equal = 3,
  /**
   * 不等于
   */
  notEqual = 4,
  /**
   * 小于
   */
  less = 5,
  /**
   * 小于等于
   */
  lessOrEqual = 6,
  /**
   * 大于
   */
  greater = 7,
  /**
   * 大于等于
   */
  greaterOrEqual = 8,
  /**
   * 开始于
   */
  startsWith = 9,
  /**
   * 结束于
   */
  endsWith = 10,
  /**
   * 包含
   */
  contains = 11,
  /**
   * 不包含
   */
  notContains = 12,
  /**
   * 包括在
   */
  in = 13
}

/**
 * 排序类型
 */

export enum SortType {
  /**
   * 倒序
   */
  desc = 0,
  /**
   * 顺序
   */
  asc = 1
}

/**
 * 字段类型
 */
export enum FieldType {
  number = 'number',
  text = 'text',
  date = 'date',
  boolCheckBox = 'boolCheckBox',
  boolSelect = 'bool-select',
  select = 'select'
}

/**
 * 搜索过滤模式
 */
export enum QueryModel {
  /**
   * 简单过滤
   */
  easy,
  /**
   * 高级过滤
   */
  advanced,
  /**
   * 自定义搜索过滤
   */
  custom
}

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
   * 字段名
   */
  field?: string
  /**
   * 值
   */
  value?: any
  /**
   * 筛选操作方式
   */
  operate?: Operate
  /**
   * 筛选条件
   */
  condition?: Condition
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

export type FieldOption = Filter & {
  /**
   * label
   */
  label?: string
  /**
   * 字段类型
   */
  type?: FieldType
  /**
   * easy模式
   */
  easy?: boolean
  /**
   * 当type为select时，需要传入该options
   */
  selectOptions?: OptionType<any>[] | any[]
  /**
   * 对应 QSelect 中的参数，仅当字段类型为select或bool时有效
   */
  emitValue?: boolean
  /**
   * 对应 QSelect 中的参数，仅当字段类型为select或bool时有效
   */
  mapOptions?: boolean
  /**
   * 对应 QSelect 中的参数，仅当字段类型为select或bool时有效
   */
  multiple?: boolean
  /**
   * 对应 QSelect 中的参数，仅当字段类型为select或bool时有效
   */
  useChips?: boolean
}
