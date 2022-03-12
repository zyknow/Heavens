<template>
  <div class="flex flex-row items-center space-x-1">
    <!-- #region custom 模式 字段 和 判断条件选项-->
    <q-select
      v-if="mode == QueryModel.custom"
      :model-value="option"
      :options="fieldOptions"
      outlined
      dense
      value
      map-options
      emit-value
      options-dense
      selected
      @update:model-value="(v) => $emit('onFieldChange', option, v)"
    />
    <q-select
      v-if="mode == QueryModel.custom"
      v-model="option.operate"
      :options="getOperatesByFieldType(option.type)"
      outlined
      dense
      map-options
      emit-value
      options-dense
      :option-label="(v) => t(`enum.operate.${v.label}`)"
    />
    <!-- #endregion -->

    <!-- advanced 模式下 Label 显示-->
    <span v-if="mode == QueryModel.advanced">{{ option.label }}：</span>

    <!-- #region 字段 -->
    <q-input
      v-if="option.type == FieldType.text || option.type == FieldType.number || option.type == FieldType.numberBetween"
      v-model="option.value"
      :type="(option.type.replace('Between','') as any)"
      outlined
      dense
    />

    <q-checkbox v-if="option.type == FieldType.boolCheckBox" v-model="option.value" toggle-indeterminate />
    <q-date-time v-if="option.type == FieldType.date" v-model="option.value" outlined dense />

    <q-select
      v-if="option.type == FieldType.select || option.type == FieldType.boolSelect"
      v-model="option.value"
      outlined
      dense
      :options="
        option.type != FieldType.boolSelect ? option.selectOptions : option.selectOptions || defaultBoolSelectOption
      "
      :map-options="option.mapOptions != undefined ? option.mapOptions : true"
      :emit-value="option.type != FieldType.boolSelect ? option.emitValue : true"
      :multiple="option.type != FieldType.boolSelect ? option.multiple : false"
      :use-chips="option.useChips"
      clearable
    />
    <!-- #endregion -->
  </div>
</template>
<script lang="ts">
export default {
  name: ''
}
</script>
<script lang="ts" setup>
import { OptionType } from '@/utils/enum'
import { getOperatesByFieldType } from '@/utils/page-request/query'
import { FieldOption, FieldType, QueryModel } from '@/utils/page-request/typing'
import { ref, toRefs, reactive, PropType, getCurrentInstance } from 'vue'
import { useI18n } from 'vue-i18n'
import qDateTime from '../framework-components/q-date-time.vue'
defineEmits(['onFieldChange'])

const props = defineProps({
  fieldOption: Object as PropType<FieldOption>,
  fieldOptions: Object as PropType<{ label: string; value: any }[]>,
  mode: Object as PropType<QueryModel>
})

const defaultBoolSelectOption: OptionType<any>[] = [
  {
    label: '是',
    value: 'true'
  },
  {
    label: '不是',
    value: 'false'
  }
]

const option = props.fieldOption as FieldOption

const t = useI18n().t
</script>
<style lang="sass" scoped></style>
