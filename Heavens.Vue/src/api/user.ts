import { IndexSign } from '@/typing'
import { PageRequest } from '@/utils/page-request'
import request from 'src/utils/request'
import { BaseEntity, PagedList, RequestResult } from './_typing'

// *---------------------------------------------------------------interface-------------------------------------------------------------------

/**
 * 用户信息表 类型
 * @interface User extends IndexSign,BaseEntity
 */
export interface User extends IndexSign, BaseEntity {
  /**
   * 名字
   */
  name: string
  /**
   * 登录账号
   */
  account: string
  /**
   * 登录密码
   */
  passwd: string
  /**
   * 是否启用
   */
  enabled: boolean
  /**
   * 持有的角色 |号分割
   */
  roles: string[]
  /**
   * 邮箱
   */
  email: string
  /**
   * 手机号
   */
  phone: string
  /**
   * 性别
   */
  sex: boolean
  /**
   * 备注
   */
  description: string
  /**
   * 最后登录时间
   */
  lastLoginTime: Date
}

// *---------------------------------------------------------------method---------------------------------------------------------------------

// /**
//  * 获取分页
//  */
export async function GetUserPage(req: PageRequest): Promise<RequestResult<PagedList<User>>> {
  return request.post<any, RequestResult<PagedList<User>>>('/api/user/page', req)
}

/**
 * 用户修改密码
 */
export async function ChangeUserPasswd(
  oldPasswd: string,
  newPasswd: string,
): Promise<RequestResult<boolean>> {
  return request.put<any, RequestResult<boolean>>('/api/user/passwd', { oldPasswd, newPasswd })
}

/**
 * 添加用户
 */
export async function AddUser(userInput: User): Promise<RequestResult<User>> {
  return request.post<User, RequestResult<User>>('/api/user', userInput)
}

/**
 * 编辑用户
 */
export async function UpdateUser(userInput: User): Promise<RequestResult<User>> {
  return request.put<User, RequestResult<User>>('/api/user', userInput)
}

/**
 * 根据Id查询
 */
export async function GetUserById(id: number): Promise<RequestResult<User>> {
  return request.get<number, RequestResult<User>>('/api/user/by-id', { params: { id } })
}

/**
 * 获取所有用户
 */
export async function GetAllUser(): Promise<RequestResult<User[]>> {
  return request.get<any, RequestResult<User[]>>('/api/User/all')
}

/**
 * 根据 token 获取用户信息
 */
export async function GetUserByToken(): Promise<RequestResult<User>> {
  return request.get<any, RequestResult<User>>('/api/user/by-token')
}

/**
 * 删除
 */
export async function DeleteUserByIds(ids: number[]): Promise<RequestResult<number>> {
  return request.request<any, RequestResult<number>>({
    url: '/api/user/by-ids',
    method: 'delete',
    data: ids,
  })
}
