import { userState } from './../store/user-state'
import { appState } from './../store/app-state'
import { boot } from 'quasar/wrappers'
import axios, { AxiosInstance } from 'axios'
import request from 'src/utils/request'
import { loadAppSettings } from '@/utils/app-settings'

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $axios: AxiosInstance
  }
}
// * -------------------------------- boot --------------------------------------------------------
export default boot(({ app }) => {
  // for use inside Vue files (Options API) through this.$axios and this.$api

  app.config.globalProperties.$axios = axios
  // ^ ^ ^ this will allow you to use this.$axios (for Vue Options API form)
  //       so you won't necessarily have to import axios in each vue file

  app.config.globalProperties.$api = request
  // ^ ^ ^ this will allow you to use this.$api (for Vue Options API form)
  //       so you can easily perform requests against your app's API

  // 加载appsettings
  loadAppSettings()
})
