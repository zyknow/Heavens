import { cloneDeep } from 'lodash-es'
import { Pagination } from './../utils/page-request/index'
import { IndexSign } from '@/typing'
import { Filter, PageRequest } from '@/utils/page-request'
import { FilterCondition, FilterOperate } from '@/utils/page-request/enums'
import request from 'src/utils/request'
import { BaseEntity, PagedList, RequestResult } from './_typing'

/**
 * Audit
 */
export interface Audit extends IndexSign, BaseEntity {
  /**
   * 用户持有角色
   */
  userRoles?: string[]
  /**
   * 服务 (类/接口) 名
   */
  serviceName: string
  /**
   * 执行方法名称
   */
  methodName: string
  /**
   * 请求路径
   */
  path: string
  /**
   * Body参数
   */
  body: string
  /**
   * Query参数
   */
  query: string
  /**
   * Http请求方法
   */
  httpMethod: string
  /**
   * 返回值
   */
  returnValue: string
  /**
   * 方法调用的总持续时间（毫秒）
   */
  executionMs?: number
  /**
   * 客户端的IP地址
   */
  clientIpAddress: string
  /**
   * 方法执行期间发生异常
   */
  exception: string
}

export interface AuditPage extends Audit {
  hasBody?: boolean
  hasQuery?: boolean
  hasException?: boolean
  hasReturnValue?: boolean
}

export interface AuditPageRequestField {
  [propName: string]: any

  hasBody?: boolean
  hasQuery?: boolean
  hasException?: boolean
  hasReturnValue?: boolean
  userRoles?: string[]
  serviceName: string
  methodName: string
  path: string
  body: string
  query: string
  httpMethod: string
  returnValue: string
  clientIpAddress: string
  exception: string
  createdId?: number
  createdBy?: string
  'min.createdTime'?: string
  'max.createdTime'?: string
  'min.executionMs'?: string
  'max.executionMs'?: string
}

const filterRules: Filter[] = [
  {
    field: 'hasBody',
    value: undefined,
    operate: FilterOperate.equal,
    condition: FilterCondition.and
  },
  {
    field: 'hasQuery',
    value: undefined,
    operate: FilterOperate.equal,
    condition: FilterCondition.and
  },
  {
    field: 'hasException',
    value: undefined,
    operate: FilterOperate.equal,
    condition: FilterCondition.and
  },
  {
    field: 'hasReturnValue',
    value: undefined,
    operate: FilterOperate.equal,
    condition: FilterCondition.and
  },
  {
    field: 'userRoles',
    value: undefined,
    operate: FilterOperate.contains,
    condition: FilterCondition.and
  },
  {
    field: 'serviceName',
    value: '',
    operate: FilterOperate.contains,
    condition: FilterCondition.and
  },
  {
    field: 'methodName',
    value: '',
    operate: FilterOperate.contains,
    condition: FilterCondition.and
  },
  {
    field: 'path',
    value: '',
    operate: FilterOperate.contains,
    condition: FilterCondition.and
  },
  {
    field: 'body',
    value: '',
    operate: FilterOperate.contains,
    condition: FilterCondition.and
  },
  {
    field: 'query',
    value: '',
    operate: FilterOperate.contains,
    condition: FilterCondition.and
  },
  {
    field: 'httpMethod',
    value: '',
    operate: FilterOperate.equal,
    condition: FilterCondition.and
  },
  {
    field: 'returnValue',
    value: '',
    operate: FilterOperate.contains,
    condition: FilterCondition.and
  },
  {
    field: 'createdId',
    value: '',
    operate: FilterOperate.equal,
    condition: FilterCondition.and
  },
  {
    field: 'exception',
    value: '',
    operate: FilterOperate.contains,
    condition: FilterCondition.and
  },
  {
    field: 'createdBy',
    value: '',
    operate: FilterOperate.contains,
    condition: FilterCondition.and
  },
  {
    field: 'clientIpAddress',
    value: '',
    operate: FilterOperate.equal,
    condition: FilterCondition.and
  },
  {
    field: 'min.createdTime',
    value: '',
    operate: FilterOperate.greaterOrEqual,
    condition: FilterCondition.and
  },
  {
    field: 'max.createdTime',
    value: '',
    operate: FilterOperate.lessOrEqual,
    condition: FilterCondition.and
  },
  {
    field: 'min.executionMs',
    value: '',
    operate: FilterOperate.greaterOrEqual,
    condition: FilterCondition.and
  },
  {
    field: 'max.executionMs',
    value: '',
    operate: FilterOperate.lessOrEqual,
    condition: FilterCondition.and
  }
]

/**
 * 获取分页
 */
export async function GetAuditPage(
  pagination: Pagination,
  fieldObj: AuditPageRequestField
): Promise<RequestResult<PagedList<AuditPage>>> {
  const req = new PageRequest(cloneDeep(filterRules))
  req.setPagination(pagination)
  req.setRuleByField(fieldObj)
  return request.post<any, RequestResult<PagedList<AuditPage>>>('/api/audit/page', req)
}

/**
 * 添加用户
 */
export async function AddAudit(audit: Audit): Promise<RequestResult<Audit>> {
  return request.post<Audit, RequestResult<Audit>>('/api/audit', audit)
}

/**
 * 编辑用户
 */
export async function UpdateAudit(audit: Audit): Promise<RequestResult<Audit>> {
  return request.put<Audit, RequestResult<Audit>>('/api/audit', audit)
}

/**
 * 根据Id查询
 */
export async function GetAuditById(id: number): Promise<RequestResult<Audit>> {
  return request.get<number, RequestResult<Audit>>('/api/audit/by-id', { params: { id } })
}

/**
 * 获取所有用户
 */
export async function GetAllAudit(): Promise<RequestResult<Audit[]>> {
  return request.get<any, RequestResult<Audit[]>>('/api/Audit/all')
}

/**
 * 删除
 */
export async function DeleteAuditByIds(ids: number[]): Promise<RequestResult<number>> {
  return request.request<any, RequestResult<number>>({
    url: '/api/audit/by-ids',
    method: 'delete',
    data: ids
  })
}
