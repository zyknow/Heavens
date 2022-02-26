import { isDev } from './index'
import { RequestResult, ResponseBody } from 'src/api/_typing'
import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios'
import { notify } from './notify'
import router from '@/router'
import { getAppSettingsByLocalStorage } from './app-settings'
import { TokenInfo } from '@/store/_typing'
import { userState } from '@/store/user-state'
import { loginRoutePath } from '@/router/routes'
export const REQUEST_TOKEN_KEY = 'Authorization'
export const REQUEST_REFRESH_TOKEN_KEY = 'X-Authorization'

const settings = getAppSettingsByLocalStorage()?.axios
const request = axios.create({
  baseURL: settings?.baseURL,
  timeout: settings?.timeout
})

// 异常拦截处理器
const errorHandler = async (error: AxiosError): Promise<any> => {
  if (error.response) {
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const { data = {}, status, statusText } = error.response

    // 403 无权限
    if (status === 403) {
      notify.error('Forbidden', { caption: '无权限', position: 'top-right' })
    }
    // 401 未登录/未授权
    if (status === 401) {
      notify.error('Unauthorized', { caption: '未授权', position: 'top-right' })
      // notify.error('未授权或登录过期，请重新登录')
      router.push(loginRoutePath)
    }
  }
  return new RequestResult({
    succeeded: false,
    errors: error.message,
    excption: error
  } as RequestResult)
}

// 请求拦截器
const requestHandler = (config: AxiosRequestConfig): AxiosRequestConfig | Promise<AxiosRequestConfig> => {
  const tokenInfo = userState.tokenInfo

  if (tokenInfo?.token) {
    config.headers[REQUEST_TOKEN_KEY] = `Bearer ${tokenInfo.token}`
  }
  // 过期时携带refreshToken,后台时间戳精度只有10位，需要除1000
  if (tokenInfo?.refreshToken && tokenInfo?.expirationTime <= new Date().getTime() / 1000) {
    config.headers[REQUEST_REFRESH_TOKEN_KEY] = `Bearer ${tokenInfo.refreshToken}`
  }
  return config
}

// Add a request interceptor
request.interceptors.request.use(requestHandler, errorHandler)

// 响应拦截器
const responseHandler = (response: AxiosResponse): ResponseBody<any> | AxiosResponse<any> | Promise<any> | any => {
  const tokenInfo = userState.tokenInfo

  const newToken = response.headers['access-token']
  const newRefreshToken = response.headers['x-access-token']

  if (newToken && newToken != tokenInfo?.token) {
    // 新token中的过期时间戳
    const expirationTime = JSON.parse(
      decodeURIComponent(escape(window.atob(newToken.split('.')[1].replace(/-/g, '+').replace(/_/g, '/'))))
    ).exp

    const newTokenInfo = {
      token: newToken,
      refreshToken: newRefreshToken,
      expirationTime
    } as TokenInfo

    userState.tokenInfo = newTokenInfo
    // TODO: 拿到新token重新获取用户信息
  }

  if (isDev) {
    // 开发环境下包含扩展信息
    if (response?.data?.extras) {
      console.log(response.data.extras)
    }
  }

  return new RequestResult(response.data)
}

// Add a response interceptor
request.interceptors.response.use(responseHandler, errorHandler)

export default request
