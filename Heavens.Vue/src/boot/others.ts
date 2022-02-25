import { boot } from 'quasar/wrappers'
import Particles from 'particles.vue3'
export default boot(({ app }) => {
  // Set i18n instance on app
  app.use(Particles)
})
