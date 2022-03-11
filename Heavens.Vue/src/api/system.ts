import { IndexSign } from '@/typing'
import request from 'src/utils/request'
import { BaseEntity, PagedList, RequestResult } from './_typing'

/**
 * 获取后台版本号
 */
export async function GetCoreVersion(): Promise<RequestResult<string>> {
  return request.get<any, RequestResult<string>>('/api/system/version')
}
