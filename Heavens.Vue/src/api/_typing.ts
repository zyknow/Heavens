import { notify, NotifyOption } from '@/utils/notify'
import { AxiosError } from 'axios'

export interface ResponseBody<T = any> {
  message: string
  code: number
  data?: T | T[]
}

/** 统一返回结构体 */

export interface PagedList<T = any> {
  pageIndex?: number
  pageSize?: number
  totalCount?: number
  totalPages?: number
  items?: Array<T>
  hasPrevPages?: boolean
  hasNextPages?: boolean
}

export interface RequestResult<T = any> {
  statusCode?: number
  data: T
  succeeded?: boolean
  errors?: Array<string | any> | string
  extras?: Array<any>
  timestamp?: number
  excption?: AxiosError // 请求异常时会包含此字段
}
export class RequestResult<T = any> {
  constructor(req: RequestResult) {
    this.statusCode = req.statusCode
    this.data = req.data
    this.succeeded = req.succeeded
    this.errors = req.errors
    this.extras = req.extras
    this.timestamp = req.timestamp
    this.excption = req.excption
  }
  /**
   * axios执行结果提示
   * @param opt ?: NotifyOption
   */
  notify = (opt?: NotifyOption) => {
    notify.response(this, opt)
  }
  /**
   * axios执行消息提示,仅当执行失败时
   * @param opt ?: NotifyOption
   */
  notifyOnErr = (opt?: NotifyOption) => {
    notify.responseOnErr(this, opt)
  }
}

export interface BaseEntity {
  /**
   * id主键
   */
  id: number
  /**
   * 创建者id
   */
  createdId: number
  /**
   * 创建者
   */
  createdBy?: string
  /**
   * 创建时间
   */
  createdTime: string
  /**
   * 更新者id
   */
  updatedId: number
  /**
   * 更新者
   */
  updatedBy?: string
  /**
   * 更新时间
   */
  updatedTime: string
}
