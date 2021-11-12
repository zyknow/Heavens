import axios from 'axios'
import { ls } from '.'

export const APP_SETTINGS = 'appsettings'

export interface AppSettings {
  axios: {
    baseURL: string
    timeout: number
  }
  searchEngine: {
    host: string
    masterKey: string
  }
}

export const getAppSettingsByLocalStorage = (): AppSettings => {
  return ls.getItem(APP_SETTINGS) as AppSettings
}

export const loadAppsetings = () => {
  // 获取appsettings
  axios
    .get('appsettings.json')
    .then(({ data }) => {
      const appsettings = ls.getItem(APP_SETTINGS) as any
      if (!appsettings || JSON.stringify(data) != JSON.stringify(appsettings)) {
        ls.set(APP_SETTINGS, data)
        location.reload()
      }
    })
    .catch(e => {
      console.error('加载 appsettings.json 失败')
    })
}
