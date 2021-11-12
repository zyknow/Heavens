import { ls } from 'src/utils'
import { User } from 'src/api/user'
import { TokenInfo } from './_typing'
import { TOKEN_INFO } from './mutations'
export type UserState = {
  info: User
  routers: []
  tokenInfo: TokenInfo
}

export const state: UserState = {} as UserState

// 初始化获取缓存token
state.tokenInfo = ls.getItem(TOKEN_INFO) as TokenInfo

export const initUserState: UserState = Object.assign({}, state)

// 角色
export const staticRoles = {
  admin: 'admin',
  user: 'user',
  test: 'test',
}

// 角色组
export const staticRoleGroups = {
  userGroup: [staticRoles.admin, staticRoles.user],
}
