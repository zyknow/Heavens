/**
 * <br />&nbsp;并且 And = 1<br />&nbsp;或者 Or = 2<br />
 */
export type FilterCondition = 1 | 2

export const FilterCondition = {
  and: 1 as FilterCondition,
  or: 2 as FilterCondition
}

/**
 * 筛选操作方式<br />&nbsp;等于 Equal = 3<br />&nbsp;不等于 NotEqual = 4<br />&nbsp;小于 Less = 5<br />&nbsp;小于等于 LessOrEqual = 6<br />&nbsp;大于 Greater = 7<br />&nbsp;大于等于 GreaterOrEqual = 8<br />&nbsp;开始于 StartsWith = 9<br />&nbsp;结束于 EndsWith = 10<br />&nbsp;包含 Contains = 11<br />&nbsp;不包含 NotContains = 12<br />&nbsp;包括在 In = 13<br />
 */
export type FilterOperate = 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13

export const FilterOperate = {
  equal: 3 as FilterOperate,
  notEqual: 4 as FilterOperate,
  less: 5 as FilterOperate,
  lessOrEqual: 6 as FilterOperate,
  greater: 7 as FilterOperate,
  greaterOrEqual: 8 as FilterOperate,
  startsWith: 9 as FilterOperate,
  endsWith: 10 as FilterOperate,
  contains: 11 as FilterOperate,
  notContains: 12 as FilterOperate,
  in: 13 as FilterOperate
}

/**
 * 排序类型<br />&nbsp; Desc = 0<br />&nbsp; Asc = 1<br />
 */
export type SortType = 0 | 1

export const SortType = {
  desc: 0 as SortType,
  asc: 1 as SortType
}
