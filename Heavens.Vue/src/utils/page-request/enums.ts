/**
 * 筛选条件
 */
export type FilterCondition = 1 | 2

export const FilterCondition = {
  /**
   * 或者
   */
  or: 1 as FilterCondition,
  /**
   * 并且
   */
  and: 2 as FilterCondition
}

/**
 * 筛选操作方式
 */
export type FilterOperate = 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13

export const FilterOperate = {
  /**
   * 等于
   */
  equal: 3 as FilterOperate,
  /**
   * 不等于
   */
  notEqual: 4 as FilterOperate,
  /**
   * 小于
   */
  less: 5 as FilterOperate,
  /**
   * 小于等于
   */
  lessOrEqual: 6 as FilterOperate,
  /**
   * 大于
   */
  greater: 7 as FilterOperate,
  /**
   * 大于等于
   */
  greaterOrEqual: 8 as FilterOperate,
  /**
   * 开始于
   */
  startsWith: 9 as FilterOperate,
  /**
   * 结束于
   */
  endsWith: 10 as FilterOperate,
  /**
   * 包含
   */
  contains: 11 as FilterOperate,
  /**
   * 不包含
   */
  notContains: 12 as FilterOperate,
  /**
   * 包括在
   */
  in: 13 as FilterOperate
}

/**
 * 排序类型
 */
export type SortType = 0 | 1

export const SortType = {
  /**
   * 倒序
   */
  desc: 0 as SortType,
  /**
   * 顺序
   */
  asc: 1 as SortType
}
