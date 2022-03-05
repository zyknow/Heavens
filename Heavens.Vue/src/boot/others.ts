import { boot } from 'quasar/wrappers'
import Particles from 'particles.vue3'
import JsonViewer from 'vue3-json-viewer'
export default boot(({ app }) => {
  app.use(Particles)
  app.use(JsonViewer)
})
