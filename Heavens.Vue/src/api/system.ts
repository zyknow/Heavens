import { IndexSign } from '@/typing'
import { PageRequest } from '@/utils/page-request'
import request from 'src/utils/request'
import { BaseEntity,PagedList, RequestResult } from './_typing'


// /**
//  * 获取后台版本号
//  */
export async function GetCoreVersion(req: PageRequest): Promise<RequestResult<string>> {
  return request.post<any, RequestResult<string>>('/api/system/version')
}