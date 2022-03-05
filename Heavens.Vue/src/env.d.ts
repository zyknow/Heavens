declare module 'particles.vue3'
declare module 'vue3-json-viewer' {
  import { AllowedComponentProps, App, Component, ComponentCustomProps, VNodeProps } from 'vue'
  interface JsonViewerProps {
    // eslint-disable-next-line @typescript-eslint/ban-types
    value: Object | Array<any> | string | number | boolean //对象
    expanded: boolean //是否自动展开
    expandDepth: number //展开层级
    copyable: boolean | object //是否可复制
    sort: boolean //是否排序
    boxed: boolean //是否boxed
    theme: string //主题 jv-dark | jv-light
    previewMode: boolean //是否可复制
    timeformat: (value: any) => string
  }
  type JsonViewerType = JsonViewerProps & VNodeProps & AllowedComponentProps & ComponentCustomProps
  const JsonViewer: Component<JsonViewerType>
  export { JsonViewer }
  const def: { install: (app: App) => void }
  export default def
}
declare namespace NodeJS {
  interface ProcessEnv {
    NODE_ENV: string
    VUE_ROUTER_MODE: 'hash' | 'history' | 'abstract' | undefined
    VUE_ROUTER_BASE: string | undefined
  }
}
