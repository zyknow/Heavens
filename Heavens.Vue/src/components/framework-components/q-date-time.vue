<template>
  <q-input :model-value="modelValue" @update:model-value="(v) => $emit('update:modelValue', v)">
    <template #prepend>
      <q-icon name="event" class="cursor-pointer">
        <q-popup-proxy cover transition-show="scale" transition-hide="scale">
          <q-date
            :model-value="modelValue"
            mask="YYYY-MM-DD HH:mm:ss"
            @update:model-value="(v) => $emit('update:modelValue', v)"
          >
            <div class="row items-center justify-end">
              <q-btn v-close-popup label="Close" color="primary" flat />
            </div>
          </q-date>
        </q-popup-proxy>
      </q-icon>
    </template>

    <template #append>
      <q-icon name="access_time" class="cursor-pointer">
        <q-popup-proxy cover transition-show="scale" transition-hide="scale">
          <q-time
            :model-value="modelValue"
            mask="YYYY-MM-DD HH:mm:ss"
            format24h
            @update:model-value="(v) => $emit('update:modelValue', v)"
          >
            <div class="row items-center justify-end">
              <q-btn v-close-popup label="Close" color="primary" flat />
            </div>
          </q-time>
        </q-popup-proxy>
      </q-icon>
    </template>
  </q-input>
</template>
<script lang="ts">
export default {
  name: ''
}
</script>
<script lang="ts" setup>
import { ref, toRefs, reactive, PropType } from 'vue'
import { useI18n } from 'vue-i18n'

const emit = defineEmits(['update:modelValue'])

defineProps({
  modelValue: String
})

const t = useI18n().t
const state = reactive({})
</script>
<style lang="sass" scoped></style>
