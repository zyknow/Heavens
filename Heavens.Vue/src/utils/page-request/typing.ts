import { OptionType } from './../enum'
/**
 * 筛选条件
 */
export enum Condition {
  /**
   * 或者
   */
  or = 'Or',
  /**
   * 并且
   */
  and = 'And'
}

/**
 * 筛选操作方式
 */
export enum Operate {
  /**
   * 等于
   */
  equal = 'Equal',
  /**
   * 不等于
   */
  notEqual = 'NotEqual',
  /**
   * 小于
   */
  less = 'Less',
  /**
   * 小于等于
   */
  lessOrEqual = 'LessOrEqual',
  /**
   * 大于
   */
  greater = 'Greater',
  /**
   * 大于等于
   */
  greaterOrEqual = 'GreaterOrEqual',
  /**
   * 开始于
   */
  startsWith = 'StartsWith',
  /**
   * 结束于
   */
  endsWith = 'EndsWith',
  /**
   * 包含
   */
  contains = 'Contains',
  /**
   * 不包含
   */
  notContains = 'NotContains',
  /**
   * 包括在
   */
  in = 'In'
}

/**
 * 排序类型
 */

export enum SortType {
  /**
   * 倒序
   */
  desc = 'Desc',
  /**
   * 顺序
   */
  asc = 'Asc'
}

/**
 * 字段类型
 */
export enum FieldType {
  numberBetween = 'numberBetween',
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
  operate: Operate
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
   * ? 仅支持 text 和 number
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

  /**
   * 列配置
   */
  columns?: {
    /**
     * 如果我们使用可见列，这个col将始终是可见的
     */
    required?: boolean
    /**
     * 此列中单元格的水平对齐
     * 默认值: right
     */
    align?: 'left' | 'right' | 'center'
    /**
     * 是否可排序
     */
    sortable?: boolean
    /**
     * 比较函数，如果您有一些自定义数据或想要一个特定的方式来比较两行
     * @param a 第一个比较项的值
     * @param b 第二个比较项的值
     * @param rowA 对象，其中包含第一项
     * @param rowB 对象，其中包含第二项
     * @returns 项a与项b的比较结果。 当'a'应该在前面时小于0; 如果'b'在前面，则该值大于0; 如果它们的位置不能相互改变，则为0
     */
    sort?: (a: any, b: any, rowA: any, rowB: any) => number
    /**
     * 设置列排序顺序:'ad'(升序-降序)或'da'(降序-升序); 重写'column-sort-order'支柱
     * 默认值: ad
     */
    sortOrder?: 'ad' | 'da'
    /**
     * 格式化数据显示
     * @param val 单元格的值
     * @param row 列的值
     * @returns 生成的格式化值
     */
    format?: (val: any, row: any) => any
    /**
     * 样式应用于列的正常单元格
     * @param row 列的值
     */
    style?: string | ((row: any) => string)
    /**
     * 添加class 到单元格上
     * @param row 列的值
     */
    classes?: string | ((row: any) => string)
    /**
     * 标题单元格样式
     */
    headerStyle?: string
    /**
     * 标题单元格 Class
     */
    headerClasses?: string
  }

  /**
   * 默认显示在table中的列 默认值为true
   */
  defaultVisibleColumn?: boolean

  /**
   * 在查询中排除，仅用于配置table Column
   */
  excludeQuery?: boolean

  /**
   * ? 仅在开发人员需要自定义多个参数and 或 or 中间的隔层，一般不要使用
   * ? 具体使用方式请参考文档
   */
  notRemoveItemByQuery?: boolean
}
