<template>
  <div class="flex flex-col space-y-1 w-full">
    <div class="flex flex-row items-center justify-between">
      <div class="flex flex-row space-x-1">
        <!-- TODO: 字符串类型分一组，数字类型分一组 -->
        <q-input
          v-if="query.mode == QueryModel.easy && easyOption.number.label"
          v-model="easyOption.number.searchKey"
          :class="easyNumberInputClass"
          :label="easyOption.number.label"
          type="number"
          outlined
          dense
          @keydown.enter="onSearch"
        />
        <q-input
          v-if="query.mode == QueryModel.easy && easyOption.text.label"
          v-model="easyOption.text.searchKey"
          :class="easyTextInputClass"
          :label="easyOption.text.label"
          outlined
          dense
          @keydown.enter="onSearch"
        />
        <q-btn dense icon="search" color="primary" :loading="loading" @click="onSearch">搜索</q-btn>
        <slot name="btn"></slot>
      </div>
      <div class="flex flex-row mr-32">
        <q-select
          v-model="query.mode"
          outlined
          dense
          class="w-40"
          label="搜索模式"
          map-options
          emit-value
          :options="enumToOption(QueryModel)"
          :option-label="(v) => t(`enum.query_mode.${v.label}`)"
        />
        <q-btn
          flat
          style="margin-left: 10px"
          color="danger"
          @click="
            // eslint-disable-next-line prettier/prettier
                query.reset(); easyOption.reset();$forceUpdate()
          "
          >重置过滤</q-btn
        >
      </div>
    </div>

    <div v-if="query.mode == QueryModel.advanced" class="flex flex-row">
      <div v-for="(item, index) in query.filters" :key="index" class="flex flex-row mr-2 mt-2">
        <query-field-item
          :mode="query.mode"
          :fieldOption="item"
          :field-options="fieldOptions"
          @on-field-change="onFieldChange"
        />
      </div>
    </div>

    <div v-if="query.mode == QueryModel.custom" class="flex flex-col space-y-0.5">
      <div class="flex flex-row space-x-1 items-center">
        <q-select
          v-model="state.condition"
          outlined
          dense
          map-options
          emit-value
          :options="enumToOption(Condition)"
          :display-value="state.condition == Condition.or ? '符合以下任何条件' : '符合以下所有条件'"
          :option-label="(v) => (v.value == Condition.or ? '符合以下任何条件' : '符合以下所有条件')"
          @update:model-value="(v) => query.setAllCondition(v)"
        />
        <q-btn color="primary" icon="add" dense flat @click="add" />
      </div>
      <div class="flex flex-row space-y-0.5 space-x-5">
        <div v-for="(item, index) in query.filters" :key="index" class="flex flex-row space-x-1 ml-5">
          <query-field-item
            :mode="query.mode"
            :fieldOption="item"
            :field-options="fieldOptions"
            @on-field-change="onFieldChange"
          />
          <q-btn icon="remove" dense color="danger" @click="query.filters.splice(index, 1)" />

          <!-- <q-separator vertical /> -->
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
export default {
  name: ''
}
</script>
<script lang="ts" setup>
import { enumToOption, fromEnum, OptionType } from '@/utils/enum'
import { ref, toRefs, reactive, PropType, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import qDateTime from '../framework-components/q-date-time.vue'
import { cloneDeep, first, join } from 'lodash-es'

import { copyByKeys } from '@/utils'
import { BaseQuery, getOperatesByFieldType } from '@/utils/page-request/query'
import { Condition, FieldOption, FieldType, QueryModel } from '@/utils/page-request/typing'
import QueryFieldItem from './query-field-item.vue'
const t = useI18n().t

const emits = defineEmits(['onSearch'])

const props = defineProps({
  baseQuery: Object as PropType<BaseQuery>,
  loading: Boolean,
  easyTextInputClass: String,
  easyNumberInputClass: String
})

const query = props.baseQuery as BaseQuery

watch(
  () => query.mode,
  (mode) => {
    query.reset()
  }
)

// custom模式
const fieldOptions: OptionType<string>[] = query.fieldOptions?.map((p: FieldOption) => {
  return { label: p.label as string, value: `${p.field}-${p.label}` }
})

// custom模式 默认添加的Option
const defaultAddFieldOption = first(query.fieldOptions)

const easyOption = reactive({
  text: {
    label: join(
      query.fieldOptions.filter((f) => f.easy == true && f.type == FieldType.text).map((f) => f.label),
      '/'
    ),
    searchKey: ''
  },
  number: {
    label: join(
      query.fieldOptions.filter((f) => f.easy == true && f.type == FieldType.number).map((f) => f.label),
      '/'
    ),
    searchKey: undefined
  },
  reset: () => {
    easyOption.number.searchKey = undefined
    easyOption.text.searchKey = ''
  }
})

const state = reactive({
  condition: Condition.or
})

const add = () => {
  const item: FieldOption = cloneDeep({
    ...defaultAddFieldOption,
    condition: state.condition,
    operate: first(getOperatesByFieldType(defaultAddFieldOption?.type))?.value
  })

  query.filters.push(item)
}

onMounted(() => {
  query.reset()
})

const onFieldChange = (item: FieldOption, v: string) => {
  const fieldOption = query.fieldOptions?.find((p) => `${p.field}-${p.label}` == v)
  copyByKeys(item, fieldOption)
  item.operate = first(getOperatesByFieldType(item.type))?.value
  if (fieldOption) item.value = fieldOption.value
}

const onSearch = () => {
  if (query.mode == QueryModel.easy) {
    query.filters.filter((f) => {
      if (f.type == FieldType.text) {
        f.value = easyOption.text.searchKey
      } else if (f.type == FieldType.number) {
        f.value = easyOption.number.searchKey
      }
    })
  }

  emits('onSearch')
}
</script>
<style lang="sass" scoped></style>
