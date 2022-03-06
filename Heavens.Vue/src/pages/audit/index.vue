<template>
  <div class="h-full">
    <!-- Table -->
    <q-table
      v-model:selected.sync="state.selected"
      v-model:pagination="pageQuery.pagination"
      :rows="state.audits"
      :columns="state.columns"
      selection="multiple"
      :loading="state.loading"
      :row-key="(v) => v.id"
      :visible-columns="state.visibleColumns"
      flat
      :rows-per-page-options="[10, 15, 50, 500, 1000, 10000]"
      table-header-class="bg-gray-100"
      class="h-full relative sticky-header-column-table sticky-right-column-table"
      :virtual-scroll="state.audits.length >= 200"
      @request="tableHandler"
    >
      <!-- 加载动画 -->
      <template #loading>
        <q-inner-loading showing color="primary" />
      </template>
      <!-- table 顶部操作栏 -->
      <template #top>
        <div class="w-full flex flex-row">
          <div class="flex flex-row flex-wrap mr-20 items-center space-x-0.5 space-y-0.5">
            <q-input
              v-model="pageQuery.entity.createdId"
              outlined
              :label="t('调用者Id')"
              dense
              @keyup.enter="getAudits"
            />
            <q-input
              v-model="pageQuery.entity.createdBy"
              outlined
              :label="t('调用者')"
              dense
              @keyup.enter="getAudits"
            />
            <q-input
              v-model="pageQuery.entity.serviceName"
              outlined
              :label="t('服务名')"
              dense
              @keyup.enter="getAudits"
            />
            <q-input
              v-model="pageQuery.entity.methodName"
              outlined
              :label="t('执行方法')"
              dense
              @keyup.enter="getAudits"
            />
            <q-input v-model="pageQuery.entity.path" outlined :label="t('请求路径')" dense @keyup.enter="getAudits" />
            <q-select
              v-model="pageQuery.entity.httpMethod"
              class="w-40"
              :options="['GET', 'POST', 'DELETE', 'PATH']"
              :label="t('Http请求方法')"
              outlined
              clearable
              dense
            />
            <q-input
              v-model="pageQuery.entity.clientIpAddress"
              outlined
              :label="t('请求IP')"
              dense
              @keyup.enter="getAudits"
            />
            <q-checkbox v-model="pageQuery.entity.hasBody" toggle-indeterminate :label="t('Body')" />
            <q-input
              v-if="pageQuery.entity.hasBody"
              v-model="pageQuery.entity.body"
              outlined
              :label="t('Body')"
              dense
              @keyup.enter="getAudits"
            />
            <q-checkbox v-model="pageQuery.entity.hasQuery" toggle-indeterminate :label="t('Query')" />
            <q-input
              v-if="pageQuery.entity.hasQuery"
              v-model="pageQuery.entity.query"
              outlined
              :label="t('Query')"
              dense
              @keyup.enter="getAudits"
            />
            <q-checkbox v-model="pageQuery.entity.hasException" toggle-indeterminate :label="t('异常')" />
            <q-input
              v-if="pageQuery.entity.hasException"
              v-model="pageQuery.entity.exception"
              outlined
              :label="t('异常')"
              dense
              @keyup.enter="getAudits"
            />
            <q-input
              v-model="pageQuery.entity['min-executionMs']"
              type="number"
              outlined
              :label="t('最小执行毫秒')"
              dense
              @keyup.enter="getAudits"
            />
            —
            <q-input
              v-model="pageQuery.entity['max-executionMs']"
              type="number"
              outlined
              :label="t('最大执行毫秒')"
              dense
              @keyup.enter="getAudits"
            />
            <q-date-time v-model="pageQuery.entity['min-createdTime']" outlined dense :label="t('最小创建时间')" />
            —
            <q-date-time v-model="pageQuery.entity['max-createdTime']" outlined dense :label="t('最大创建时间')" />
            <div class="flex flex-row space-x-1">
              <q-btn icon="search" color="primary" @click="getAudits" />
              <q-btn icon="r_restart_alt" color="primary" :label="t('重置过滤')" @click="pageQuery.resetEntity()" />
            </div>
          </div>
          <q-select
            v-model="state.visibleColumns"
            multiple
            outlined
            dense
            options-dense
            :display-value="$q.lang.table.columns"
            emit-value
            map-options
            :options="state.columns"
            option-value="name"
            options-cover
            class="absolute right-4 top-3"
            menu-anchor="bottom middle"
            menu-self="bottom middle"
          />
        </div>
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
import { AddAudit, DeleteAuditByIds, GetAuditById, GetAuditPage, UpdateAudit, Audit } from '@/api/audit'
import { ref, defineComponent, toRefs, reactive, computed, watch, onBeforeUnmount } from 'vue'
import { useI18n } from 'vue-i18n'
import { dateFormat } from '@/utils/date-util'
import { useQuasar } from 'quasar'
import { copyByKeys, ls } from '@/utils'
import { PageQuery, PageRequest, Pagination } from '@/utils/page-request'
import { FilterCondition, FilterOperate } from '@/utils/page-request/enums'
import { AuditPage } from '../../api/audit'
import 'vue3-json-viewer/dist/index.css'
import { camelCase } from 'lodash-es'
import { staticRoles } from '@/router/routes'
import QDateTime from '@/components/framework-Components/q-date-time.vue'

const t = useI18n().t

interface AuditDetail extends Audit {
  returnValueObj?: object | string
  bodyObj: object | string
  queryObj: object | string
  exceptionObj: object | string
}

// 显示列
const AUDIT_VISIBLE_COLUMNS = `AUDIT_VISIBLE_COLUMNS`

// 分页大小
const AUDIT_ROWS_PER_PAGE = 'AUDIT_ROWS_PER_PAGE'

// 默认显示的Table列
const defaultVisibleColumns = [
  'userRoles',
  'serviceName',
  'methodName',
  'path',
  'hasBody',
  'hasQuery',
  'httpMethod',
  'hasReturnValue',
  'executionMs',
  'clientIpAddress',
  'hasException',
  'id',
  'createdId',
  'createdBy',
  'createdTime',
  'updatedId',
  'updatedBy',
  'updatedTime',
  'actions'
]

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
  ls.set(AUDIT_VISIBLE_COLUMNS, state.visibleColumns)
  ls.set(AUDIT_ROWS_PER_PAGE, pageQuery.pagination.rowsPerPage)
})

//#region PageQuery
const pageQuery = reactive(
  new PageQuery<{
    [propName: string]: any
    hasBody?: boolean
    hasQuery?: boolean
    hasException?: boolean
    hasReturnValue?: boolean
    userRoles?: string[]
    serviceName?: string
    methodName?: string
    path?: string
    body?: string
    query?: string
    httpMethod?: string
    returnValue?: string
    clientIpAddress?: string
    exception?: string
    createdId?: number
    createdBy?: string
    'min-createdTime'?: string
    'max-createdTime'?: string
    'min-executionMs'?: string
    'max-executionMs'?: string
  }>(
    {},
    [
      {
        field: 'hasBody',
        value: undefined,
        operate: FilterOperate.equal,
        condition: FilterCondition.and
      },
      {
        field: 'hasQuery',
        value: undefined,
        operate: FilterOperate.equal,
        condition: FilterCondition.and
      },
      {
        field: 'hasException',
        value: undefined,
        operate: FilterOperate.equal,
        condition: FilterCondition.and
      },
      {
        field: 'hasReturnValue',
        value: undefined,
        operate: FilterOperate.equal,
        condition: FilterCondition.and
      },
      {
        field: 'userRoles',
        value: undefined,
        operate: FilterOperate.contains,
        condition: FilterCondition.and
      },
      {
        field: 'serviceName',
        value: '',
        operate: FilterOperate.contains,
        condition: FilterCondition.and
      },
      {
        field: 'methodName',
        value: '',
        operate: FilterOperate.contains,
        condition: FilterCondition.and
      },
      {
        field: 'path',
        value: '',
        operate: FilterOperate.contains,
        condition: FilterCondition.and
      },
      {
        field: 'body',
        value: '',
        operate: FilterOperate.contains,
        condition: FilterCondition.and
      },
      {
        field: 'query',
        value: '',
        operate: FilterOperate.contains,
        condition: FilterCondition.and
      },
      {
        field: 'httpMethod',
        value: '',
        operate: FilterOperate.equal,
        condition: FilterCondition.and
      },
      {
        field: 'returnValue',
        value: '',
        operate: FilterOperate.contains,
        condition: FilterCondition.and
      },
      {
        field: 'createdId',
        value: '',
        operate: FilterOperate.equal,
        condition: FilterCondition.and
      },
      {
        field: 'exception',
        value: '',
        operate: FilterOperate.contains,
        condition: FilterCondition.and
      },
      {
        field: 'createdBy',
        value: '',
        operate: FilterOperate.contains,
        condition: FilterCondition.and
      },
      {
        field: 'clientIpAddress',
        value: '',
        operate: FilterOperate.equal,
        condition: FilterCondition.and
      },
      {
        field: 'min-createdTime',
        value: '',
        operate: FilterOperate.greaterOrEqual,
        condition: FilterCondition.and
      },
      {
        field: 'max-createdTime',
        value: '',
        operate: FilterOperate.lessOrEqual,
        condition: FilterCondition.and
      },
      {
        field: 'min-executionMs',
        value: '',
        operate: FilterOperate.greaterOrEqual,
        condition: FilterCondition.and
      },
      {
        field: 'max-executionMs',
        value: '',
        operate: FilterOperate.lessOrEqual,
        condition: FilterCondition.and
      }
    ],
    {
      sortBy: 'id',
      descending: false,
      page: 1,
      rowsPerPage: ls.getItem<number>(AUDIT_ROWS_PER_PAGE) || 10,
      rowsNumber: 1,
      totalPages: 1
    }
  )
)
//#endregion

const state = reactive({
  columns,
  visibleColumns: (ls.getItem(AUDIT_VISIBLE_COLUMNS) || defaultVisibleColumns) as string[],
  // table选中项
  selected: [] as AuditPage[],
  // table 数据
  audits: [] as AuditPage[],
  loading: false,
  // 搜索过滤
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
  state.loading = true
  const res = await GetAuditPage(pageQuery.toPageRequest())
  state.loading = false
  res.notifyOnErr()
  if (res.succeeded) {
    state.audits = res.data?.items as AuditPage[]
    pageQuery.pagination.rowsNumber = res.data?.totalCount as number
    pageQuery.pagination.totalPages = res.data?.totalPages as number
  }
}

// 显示 详细 Dialog
const showParaDialog = async (type: string, id: number) => {
  state.loading = true
  const auditRes = await GetAuditById(id)
  state.loading = false

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
  state.loading = true
  var auditRes = await GetAuditById(id)
  state.loading = false

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
