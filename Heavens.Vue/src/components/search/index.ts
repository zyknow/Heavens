import { getAppSettingsByLocalStorage } from '@/utils/app-settings'
import { MeiliSearch } from 'meilisearch'

export const supportIndexs = {
  book: 'book',
  reader: 'reader'
}

export interface MeiliResult {
  hits: [
    {
      id: string | number
      title: string
      genres: string[]
    }
  ]
  offset: number
  limit: number
  nbHits: number
  processingTimeMs: number
  query: string
}

export class MeiliEngine {
  private client: MeiliSearch
  constructor() {
    const settings = getAppSettingsByLocalStorage().searchEngine
    this.client = new MeiliSearch({ host: settings.host, apiKey: settings.masterKey })
  }
  search = async (indexStr: string, searchStr: string) => {
    const index = await this.client.getIndex(indexStr)
    const res = await index.search(searchStr)
    return res
  }
}

export const searchEngine = new MeiliEngine()
