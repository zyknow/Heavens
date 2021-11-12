<template>
  <div>
    <q-input
      :dark="state.searchDarkStyle"
      dense
      @focus="state.searchDarkStyle = true"
      @blur="state.searchDarkStyle = false"
      standout
      clearable
      v-model="state.searchText"
      type="search"
      debounce="500"
      :label="t('全局搜索')"
      :style="getWidth"
    >
      <template v-slot:prepend>
        <q-icon v-if="!state.loading" :name="icon" />
        <!-- <q-spinner v-else color="primary" size="1.2em" :thickness="5" /> -->
        <q-spinner-tail v-else color="primary" />
      </template>
      <q-menu
        v-model="state.searchContextVisible"
        no-focus
        transition-show="jump-down"
        transition-hide="jump-up"
        no-refocus
      >
        <q-list v-for="searchIndex in searchIndexs" :key="searchIndex">
          <div v-if="searchIndex == 'users'">
            <q-item
              dense
              v-if="state.options[searchIndex + ''] && state.options[searchIndex + ''].length"
              class="flex-row flex-j-bet"
            >
              <q-item-section side>
                <q-chip square dense outline color="grey-7">{{ t('用户管理') }}</q-chip>
              </q-item-section>
              <!-- <q-btn
                flat
                dense
                color="primary"
                @click="$emit('onClick', { searchIndex,inputValue })"
              >
                搜索全部
              </q-btn> -->
              <!-- <a>搜索全部</a> -->
            </q-item>
            <q-item
              v-for="user in state.options[searchIndex + '']"
              :key="user.id"
              clickable
              :style="getWidth"
              @click="$emit('onClick', { searchIndex, entity: user })"
            >
              <q-item-section avatar>
                <q-icon name="account_circle"></q-icon>
              </q-item-section>
              <q-item-section>
                {{ user.account }}
              </q-item-section>
              <q-item-section>{{ user.name }}</q-item-section>
              <q-item-section>
                {{ user.description }}
              </q-item-section>
            </q-item>
          </div>
        </q-list>
      </q-menu>
    </q-input>
  </div>
</template>
<script lang="ts" setup>
import { useQuasar } from 'quasar'
import { ref, defineComponent, toRefs, reactive, watch, computed, defineProps, PropType } from 'vue'
import { useI18n } from 'vue-i18n'
import { searchEngine } from './index'
const props = defineProps({
  debounce: {
    type: String,
    default: '500',
  },
  icon: {
    type: String,
    default: 'travel_explore',
  },
  searchIndexs: {
    type: Array as PropType<string[]>,
    default: () => [] as string[],
  },
  width: {
    type: String,
    default: '400px',
  },
  phoneWidth: {
    type: String,
    default: '144px',
  },
})
const $q = useQuasar()
const t = useI18n().t
const state = reactive({
  searchText: '',
  loading: false,
  searchDarkStyle: false,
  options: {},
  searchContextVisible: false,
})

watch(
  () => state.searchText,
  (v, ov) => {
    onSearch(v)
  },
)

const onSearch = async inputValue => {
  state.loading = true
  if (!inputValue) {
    state.loading = false
    state.options = {}
  } else {
    state.options = {}
    const opt = state.options
    state.searchContextVisible = true
    for (let index = 0; index < props.searchIndexs.length; index++) {
      const indexName = props.searchIndexs[index]
      const res = await searchEngine.search(indexName as string, inputValue)
      if (!opt[indexName + '']) opt[indexName + ''] = []
      opt[indexName + ''].push(...res.hits)
    }
    state.loading = false
  }
}

const getWidth = computed(() => {
  if ($q.platform.is.mobile) {
    return `width:${props.phoneWidth}`
  } else {
    return `width:${props.width}`
  }
})
</script>
<style lang="sass">
.my-card
  width: 100%
  max-width: 250px
</style>
