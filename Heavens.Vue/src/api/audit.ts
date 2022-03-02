import { IndexSign } from '@/typing'
import { PageRequest } from '@/utils/page-request'
import request from 'src/utils/request'
import { BaseEntity,PagedList, RequestResult } from './_typing'

/**
 * Audit 
 */
export interface Audit extends IndexSign, BaseEntity {
  
  /**
  * 用户持有角色
  */
  userRoles: string
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
  executionMs: number
  /**
  * 客户端的IP地址
  */
  clientIpAddress: string
  /**
  * 方法执行期间发生异常
  */
  exception: string
}

// /**
//  * 获取分页
//  */
export async function GetAuditPage(req: PageRequest): Promise<RequestResult<PagedList<Audit>>> {
  return request.post<any, RequestResult<PagedList<Audit>>>('/api/audit/page', req)
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
    data: ids,
  })
}