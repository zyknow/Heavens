import axios from 'axios'
import { ls } from '.'

export const APP_SETTINGS = 'APP_SETTINGS'

export interface AppSettings {
  axios: {
    baseURL: string
    timeout: number
  }
}

export const getAppSettingsByLocalStorage = (): AppSettings => {
  return ls.getItem(APP_SETTINGS) as AppSettings
}

export const loadAppSettings = () => {
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
    .catch(() => {
      console.error('加载 appsettings.json 失败')
    })
}
