import { boot } from 'quasar/wrappers'
import Particles from 'particles.vue3'
export default boot(({ app }) => {
  app.use(Particles)
})
