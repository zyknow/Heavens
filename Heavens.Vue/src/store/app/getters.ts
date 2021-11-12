import { GetterTree } from 'vuex'
import { AppState } from './state'
import { RootState } from '@/store/root-state'

export const getters: GetterTree<AppState, RootState> = {
  lang: state => state.lang,
  multiTabEnabled: state => state.multiTabEnabled,
}
