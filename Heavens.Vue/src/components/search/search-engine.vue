<template>
  <div>
    <q-input
      v-model="state.searchText"
      :dark="state.searchDarkStyle"
      dense
      standout
      clearable
      type="search"
      debounce="500"
      :label="t('全局搜索')"
      :style="getWidth"
      @focus="state.searchDarkStyle = true"
      @blur="state.searchDarkStyle = false"
    >
      <template #prepend>
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
              v-if="state.options[searchIndex + ''] && state.options[searchIndex + ''].length"
              dense
              class="flex flex-row justify-center"
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
              </q-btn>-->
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
                <q-icon name="account_circle" />
              </q-item-section>
              <q-item-section>{{ user.account }}</q-item-section>
              <q-item-section>{{ user.name }}</q-item-section>
              <q-item-section>{{ user.description }}</q-item-section>
            </q-item>
          </div>
        </q-list>
      </q-menu>
    </q-input>
  </div>
</template>
<script lang="ts" setup>
import { IndexSign } from '@/typing'
import { useQuasar } from 'quasar'
import { reactive, watch, computed, PropType } from 'vue'
import { useI18n } from 'vue-i18n'
defineEmits(['onClick'])
const props = defineProps({
  debounce: {
    type: String,
    default: '500'
  },
  icon: {
    type: String,
    default: 'travel_explore'
  },
  searchIndexs: {
    type: Array as PropType<string[]>,
    default: () => [] as string[]
  },
  width: {
    type: String,
    default: '400px'
  },
  phoneWidth: {
    type: String,
    default: '144px'
  }
})
const $q = useQuasar()
const t = useI18n().t
const state = reactive({
  searchText: '',
  loading: false,
  searchDarkStyle: false,
  options: {} as IndexSign,
  searchContextVisible: false
})
const getWidth = computed(() => {
  if ($q.platform.is.mobile) {
    return `width:${props.phoneWidth}`
  } else {
    return `width:${props.width}`
  }
})
</script>
<style lang="sass" scoped>

.my-card
  width: 100%
  max-width: 250px
</style>
