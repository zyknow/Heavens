<template>
  <div class="h-full">
    <!-- Table -->
    <q-table
      v-model:selected.sync="pageQuery.selected"
      v-model:pagination="pageQuery.pagination"
      :rows="pageQuery.data"
      :columns="pageQuery.columns"
      selection="multiple"
      :loading="pageQuery.loading"
      :row-key="(v) => v.id"
      :visible-columns="pageQuery.visibleColumns"
      flat
      :rows-per-page-options="[10, 15, 50, 500, 1000, 10000]"
      table-header-class="bg-gray-100"
      class="h-full relative sticky-header-column-table sticky-right-column-table"
      :virtual-scroll="pageQuery.data.length >= 200"
      @request="tableHandler"
    >
      <!-- 加载动画 -->
      <template #loading>
        <q-inner-loading showing color="primary" />
      </template>
      <!-- table 顶部操作栏 -->
      <template #top>
        <query-filter
          easy-text-input-class="w-60"
          :base-query="(pageQuery as any)"
          :loading="pageQuery.loading"
          @on-search="getAudits()"
        />
        <q-select
          v-model="pageQuery.visibleColumns"
          multiple
          outlined
          dense
          options-dense
          :display-value="`显示${$q.lang.table.columns}`"
          emit-value
          map-options
          :options="pageQuery.columns"
          option-value="name"
          options-cover
          class="absolute right-4 top-3"
          menu-anchor="bottom middle"
          menu-self="bottom middle"
        />
      </template>

      <!-- 底部分页查询器 -->
      <template #pagination>
        <q-pagination
          v-model="pageQuery.pagination.page"
          color="primary"
          :max-pages="9"
          :max="pageQuery.pagination.totalPages"
          boundary-numbers
          @click="getAudits"
        />
      </template>

      <!-- #region Table 自定义显示 -->
      <template #body-cell-userRoles="props">
        <q-td :props="props">
          <q-chip v-for="role in props.row.userRoles" :key="role" dense outline square color="primary" :label="role" />
        </q-td>
      </template>
      <template #body-cell-hasQuery="props">
        <q-td :props="props">
          <q-chip
            v-if="props.row.hasQuery"
            clickable
            dense
            icon="info"
            outline
            square
            color="primary"
            @click="showParaDialog('Query', props.row.id)"
            >{{ props.row.hasQuery ? '详细' : '' }}</q-chip
          >
        </q-td>
      </template>

      <template #body-cell-hasBody="props">
        <q-td :props="props">
          <q-chip
            v-if="props.row.hasBody"
            clickable
            dense
            icon="info"
            outline
            square
            color="primary"
            @click="showParaDialog('Body', props.row.id)"
            >{{ props.row.hasBody ? '详细' : '' }}</q-chip
          >
        </q-td>
      </template>

      <template #body-cell-hasException="props">
        <q-td :props="props">
          <q-chip
            v-if="props.row.hasException"
            clickable
            dense
            outline
            icon="info"
            square
            color="primary"
            @click="showParaDialog('Exception', props.row.id)"
            >{{ props.row.hasException ? '详细' : '' }}</q-chip
          >
        </q-td>
      </template>

      <template #body-cell-hasReturnValue="props">
        <q-td :props="props">
          <q-chip
            v-if="props.row.hasReturnValue"
            icon="info"
            clickable
            dense
            outline
            square
            color="primary"
            @click="showParaDialog('ReturnValue', props.row.id)"
            >{{ props.row.hasReturnValue ? '详细' : '' }}</q-chip
          >
        </q-td>
      </template>

      <template #body-cell-actions="props">
        <q-td :props="props" class="space-x-1 w-1">
          <q-btn dense color="primary" :label="t('详情')" @click="showDetailDialog(props.row.id)" />
        </q-td>
      </template>
      <!-- #endregion -->
    </q-table>

    <!-- 详细  dialog-->
    <q-dialog v-model="state.paraDialogVisible" full-width>
      <q-card>
        <q-card-section>
          <div class="text-h6">{{ t(state.paraDialogTitle) }}</div>
        </q-card-section>
        <q-separator />
        <q-card-section>
          <JsonViewer
            :value="state.paraDetail"
            expanded
            :expandDepth="100"
            copyable
            boxed
            sort
            previewMode
            theme="dark"
          />
        </q-card-section>
      </q-card>
    </q-dialog>

    <!-- 详情  dialog-->
    <q-dialog v-model="state.detailDialogVisible" full-width>
      <q-card>
        <q-card-section class="flex flex-row justify-between">
          <div class="text-h6">{{ t('详情') }}</div>
          <q-btn icon="close" flat class="fixed right-12 z-10" @click="state.detailDialogVisible = false" />
        </q-card-section>
        <q-separator />
        <q-card-section>
          <div class="flex flex-col space-y-1">
            <div>
              <span>调用者角色：</span>
              <q-chip
                v-for="role in state.auditDetail.userRoles"
                :key="role"
                dense
                outline
                square
                color="primary"
                :label="role"
              />
            </div>
            <span>调用者Id：{{ state.auditDetail.createdId }}</span>
            <span>服务名Id：{{ state.auditDetail.serviceName }}</span>
            <span>执行方法：{{ state.auditDetail.methodName }}</span>
            <span>请求路径：{{ state.auditDetail.path }}</span>
            <span>Http请求方法：{{ state.auditDetail.httpMethod }}</span>
            <span>创建时间：{{ state.auditDetail.createdTime }}</span>
            <span>请求Ip：{{ state.auditDetail.clientIpAddress }}</span>
          </div>
        </q-card-section>
        <q-card-section class="json-view-section">
          <div v-if="state.auditDetail.bodyObj || state.auditDetail.body">
            <div class="text-xl">{{ t('body') }}</div>
            <JsonViewer
              :value="state.auditDetail.bodyObj || state.auditDetail.body"
              expanded
              :expandDepth="100"
              copyable
              boxed
              sort
              previewMode
              theme="dark"
            />
          </div>

          <div v-if="state.auditDetail.queryObj || state.auditDetail.query">
            <div class="text-xl">{{ t('query') }}</div>
            <JsonViewer
              :value="state.auditDetail.queryObj || state.auditDetail.query"
              expanded
              :expandDepth="100"
              copyable
              boxed
              sort
              previewMode
              theme="dark"
            />
          </div>

          <div v-if="state.auditDetail.exceptionObj || state.auditDetail.exception">
            <div class="text-xl">{{ t('异常') }}</div>
            <JsonViewer
              :value="state.auditDetail.exceptionObj || state.auditDetail.exception"
              expanded
              :expandDepth="100"
              copyable
              boxed
              sort
              previewMode
              theme="dark"
            />
          </div>

          <div v-if="state.auditDetail.returnValueObj || state.auditDetail.returnValue">
            <div class="text-xl">{{ t('返回值') }}</div>
            <JsonViewer
              :value="state.auditDetail.returnValueObj || state.auditDetail.returnValue"
              expanded
              :expandDepth="100"
              copyable
              boxed
              sort
              previewMode
              theme="dark"
            />
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>
  </div>
</template>
<script lang="ts">
// 声明额外的选项
export default {
  name: 'Audit'
}
</script>
<script lang="ts" setup>
import { AddAudit, DeleteAuditByIds, GetAuditById, GetAuditPage, UpdateAudit, Audit, HttpMethod } from '@/api/audit'
import { ref, defineComponent, toRefs, reactive, computed, watch, onBeforeUnmount } from 'vue'
import { useI18n } from 'vue-i18n'
import { copyByKeys } from '@/utils'
import { AuditPage } from '../../api/audit'
import 'vue3-json-viewer/dist/index.css'
import { camelCase } from 'lodash-es'
import QueryFilter from '@/components/query/query-filter.vue'
import { PageQuery } from '@/utils/page-request/query'
import { FieldType, Operate } from '@/utils/page-request/typing'
const t = useI18n().t

interface AuditDetail extends Audit {
  returnValueObj?: object | string
  bodyObj: object | string
  queryObj: object | string
  exceptionObj: object | string
}

const columns: any[] = [
  {
    label: t('调用者角色'),
    name: 'userRoles',
    field: 'userRoles',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('调用者Id'),
    name: 'createdId',
    field: 'createdId',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('调用者'),
    name: 'createdBy',
    field: 'createdBy',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('服务名'),
    name: 'serviceName',
    field: 'serviceName',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('执行方法'),
    name: 'methodName',
    field: 'methodName',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('请求路径'),
    name: 'path',
    field: 'path',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('Body'),
    name: 'hasBody',
    field: 'hasBody',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('Query'),
    name: 'hasQuery',
    field: 'hasQuery',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('Http请求方法'),
    name: 'httpMethod',
    field: 'httpMethod',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('执行时间'),
    name: 'executionMs',
    field: 'executionMs',
    sortable: true,
    format: (v: number) => `${v}ms`,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('异常'),
    name: 'hasException',
    field: 'hasException',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('创建时间'),
    name: 'createdTime',
    field: 'createdTime',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('请求IP'),
    name: 'clientIpAddress',
    field: 'clientIpAddress',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('返回数据'),
    name: 'hasReturnValue',
    field: 'hasReturnValue',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('操作'),
    name: 'actions',
    align: 'center',
    required: true
  }
]
// 卸载前保存缓存
onBeforeUnmount(() => {
  pageQuery.saveOption()
})
//#region PageQuery
const pageQuery = reactive(
  new PageQuery<AuditPage>([
    {
      field: 'userRoles',
      label: '调用者角色',
      easy: true,
      excludeQuery: true,
      operate: Operate.equal
    },
    {
      field: 'createdId',
      label: '调用者Id',
      type: FieldType.number,
      operate: Operate.equal,
      easy: true
    },
    {
      field: 'createdBy',
      label: '调用者',
      type: FieldType.text,
      operate: Operate.contains,
      easy: true
    },
    {
      field: 'serviceName',
      label: '服务名',
      type: FieldType.text,
      operate: Operate.contains,
      easy: true
    },
    {
      field: 'methodName',
      label: '执行方法',
      type: FieldType.text,
      operate: Operate.contains,
      easy: true
    },
    {
      field: 'path',
      label: '请求路径',
      type: FieldType.text,
      operate: Operate.contains,
      easy: true
    },
    {
      field: 'body',
      label: 'Body',
      type: FieldType.text,
      operate: Operate.contains
    },
    {
      field: 'query',
      label: 'Query',
      type: FieldType.text,
      operate: Operate.contains
    },
    {
      field: 'httpMethod',
      label: 'Http请求方法',
      type: FieldType.select,
      // selectOptions: enumToOption(HttpMethod),
      selectOptions: ['GET', 'POST', 'DELETE', 'PATH'],
      mapOptions: true,
      emitValue: true,
      operate: Operate.equal
    },
    {
      field: 'executionMs',
      label: '请求毫秒',
      type: FieldType.numberBetween,
      columns: { format: (v: number) => `${v}ms` },
      operate: Operate.equal,
      easy: true
    },
    {
      field: 'createdTime',
      label: '创建时间',
      type: FieldType.date,
      operate: Operate.equal
    },
    {
      field: 'hasException',
      label: '异常',
      type: FieldType.boolSelect,
      operate: Operate.equal
    },

    {
      field: 'hasBody',
      label: '包含Body',
      type: FieldType.boolSelect,
      operate: Operate.equal
    },
    {
      field: 'hasQuery',
      label: '包含query',
      type: FieldType.boolSelect,
      operate: Operate.equal
    },
    {
      field: 'hasQuery',
      label: '包含query',
      type: FieldType.boolSelect,
      operate: Operate.equal
    },
    {
      field: 'clientIpAddress',
      label: '请求IP',
      type: FieldType.text,
      operate: Operate.contains
    },
    {
      field: 'hasReturnValue',
      label: '返回数据',
      type: FieldType.text,
      operate: Operate.equal
    },
    {
      field: 'actions',
      label: '操作',
      type: FieldType.text,
      excludeQuery: true,
      operate: Operate.equal
    }
  ])
)

//#endregion

const state = reactive({
  // para
  paraDialogVisible: false,
  paraDialogTitle: 'Body',
  paraDetail: '',

  // detail
  auditDetail: {} as AuditDetail,
  detailDialogVisible: false
})

// 排序触发事件
const tableHandler = async ({ pagination }: any) => {
  pageQuery.pagination = pagination
  await getAudits()
}

// 获取Audit Page
const getAudits = async () => {
  pageQuery.loading = true
  const res = await GetAuditPage(pageQuery.toPageRequest())
  pageQuery.loading = false
  res.notifyOnErr()
  if (res.succeeded) {
    pageQuery.data = res.data?.items as AuditPage[]
    pageQuery.pagination.rowsNumber = res.data?.totalCount as number
    pageQuery.pagination.totalPages = res.data?.totalPages as number
  }
}

// 显示 详细 Dialog
const showParaDialog = async (type: string, id: number) => {
  pageQuery.loading = true
  const auditRes = await GetAuditById(id)
  pageQuery.loading = false

  auditRes.notifyOnErr()
  if (!auditRes.succeeded) return

  const audit = auditRes.data
  state.paraDialogTitle = type
  state.paraDialogVisible = true
  try {
    state.paraDetail = JSON.parse(audit[camelCase(type)])
  } catch (error) {
    state.paraDetail = audit[camelCase(type)]
  }
}

// 显示 详情 Dialog
const showDetailDialog = async (id: number) => {
  pageQuery.loading = true
  var auditRes = await GetAuditById(id)
  pageQuery.loading = false

  auditRes.notifyOnErr()
  if (!auditRes.succeeded) return

  const audit = auditRes.data
  copyByKeys(state.auditDetail, audit)

  try {
    state.auditDetail.bodyObj = JSON.parse(audit.body)
  } catch (e) {}
  try {
    state.auditDetail.queryObj = JSON.parse(audit.query)
  } catch (e) {}
  try {
    state.auditDetail.exceptionObj = JSON.parse(audit.exception)
  } catch (e) {}
  try {
    state.auditDetail.returnValueObj = JSON.parse(audit.returnValue)
  } catch (e) {}

  state.detailDialogVisible = true
}

getAudits()
</script>
<style lang="sass">
.json-view-section
  div
    @apply flex flex-col
</style>
