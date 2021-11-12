import { AxiosRequestConfig } from 'axios'
import request from 'src/utils/request'
import { RequestResult } from './_typing'

// *---------------------------------------------------------------interface-------------------------------------------------------------------

export type LoginType = 'AccountPassword' | 'PhonePasswd'
export type LoginClientType = 'Browser' | 'Desktop' | 'Mobile'
export interface LoginParams {
  loginType?: LoginType
  account: string
  passwd: string
  keepAlive: boolean
  loginClientType: string
}

// *---------------------------------------------------------------method---------------------------------------------------------------------

/**
 * 登录
客户端每次请求需将 accessToken 和 refreshToken 放到请求报文头中传送到服务端，格式为：
Authorization: Bearer 你的token
X-Authorization: Bearer 你的刷新token
*/
export async function GetAuthorizeJwtToken(loginInfo: LoginParams): Promise<RequestResult<void>> {
  return request.request<any, RequestResult<void>>({
    url: '/api/authorize/token',
    data: loginInfo,
    method: 'post',
  })
}
