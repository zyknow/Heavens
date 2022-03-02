
<template>
  <div class="p-2 h-full">
    <!-- Table -->
    <q-table
      :rows="state.audits"
      :columns="state.columns"
      selection="multiple"
      :loading="state.loading"
      :row-key="v => v.id"
      :visible-columns="state.visibleColumns"
      v-model:selected.sync="state.selected"
      flat
      @request="tableHandler"
      v-model:pagination="state.pagination"
      :rows-per-page-options="[10, 15, 50, 500, 1000, 10000]"
      table-header-class="bg-gray-100"
      class="h-full relative sticky-header-column-table sticky-right-column-table"
      :virtual-scroll="state.audits.length >= 200"
    >
      <template #loading>
        <q-inner-loading showing color="primary" />
      </template>

      <template #top>
        <div class="w-full flex flex-row justify-between">
          <div class="flex flex-row space-x-1">
            <q-input
              outlined
              :label="``"
              dense
              v-model="state.searchKey"
              @keyup.enter="getAudits"
            ></q-input>
            <q-btn icon="search" color="primary" @click="getAudits" />
            <q-btn color="primary" :label="t('添加')" @click="showDialog(t('添加'))" />
            <q-btn
              color="danger"
              :label="t('删除')"
              @click="deleteByIds(state.selected.map(s => s.id))"
            />
          </div>
          <div>
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
              class="float-right"
              menu-anchor="bottom middle"
              menu-self="bottom middle"
            />
          </div>
        </div>
      </template>

      <template #pagination>
        <q-pagination
          v-model="state.pagination.page"
          color="primary"
          :max-pages="9"
          :max="state.pagination.totalPages"
          boundary-numbers
          @click="getAudits"
        />
      </template>

      <template #body-cell-actions="props">
        <q-td :props="props" class="space-x-1 w-1">
          <q-btn dense color="primary" icon="edit" @click="showDialog(t('编辑'), props.row.id)" />
          <q-btn dense color="danger" icon="remove" @click="deleteByIds([props.row.id])" />
        </q-td>
      </template>

      <!-- <template #body-cell-sex="props">
        <q-td :props="props" class="w-1">
          <q-icon
            size="2rem"
            :color="props.row.sex ? 'primary' : 'pink-3'"
            :name="props.row.sex ? 'male' : 'female'"
          ></q-icon>
        </q-td> 
      </template> -->

    </q-table>

    <!-- Dialog -->
    <q-dialog v-model="state.dialogVisible">
      <q-card class="w-2/4">
        <q-card-section>
          <div class="text-h6">{{ t(state.dialogTitle) }}</div>
        </q-card-section>
        <q-separator></q-separator>

        <q-card-section class="q-pt-none mt-8">
          <q-form class="space-y-2" @submit="dialogFormSubmit">
            <q-input dense outlined v-model="state.form.userRoles" :label="t('用户持有角色')"></q-input>
<q-input dense outlined v-model="state.form.serviceName" :label="t('服务 (类/接口) 名')"></q-input>
<q-input dense outlined v-model="state.form.methodName" :label="t('执行方法名称')"></q-input>
<q-input dense outlined v-model="state.form.path" :label="t('请求路径')"></q-input>
<q-input dense outlined v-model="state.form.body" :label="t('Body参数')"></q-input>
<q-input dense outlined v-model="state.form.query" :label="t('Query参数')"></q-input>
<q-input dense outlined v-model="state.form.httpMethod" :label="t('Http请求方法')"></q-input>
<q-input dense outlined v-model="state.form.returnValue" :label="t('返回值')"></q-input>
<q-input dense outlined v-model="state.form.executionMs" :label="t('方法调用的总持续时间（毫秒）')"></q-input>
<q-input dense outlined v-model="state.form.clientIpAddress" :label="t('客户端的IP地址')"></q-input>
<q-input dense outlined v-model="state.form.exception" :label="t('方法执行期间发生异常')"></q-input>
<q-input dense outlined v-model="state.form.id" :label="t('id主键')"></q-input>
<q-input dense outlined v-model="state.form.createdId" :label="t('创建者id')"></q-input>
<q-input dense outlined v-model="state.form.createdBy" :label="t('创建者')"></q-input>
<q-input dense outlined v-model="state.form.createdTime" :label="t('创建时间')"></q-input>
<q-input dense outlined v-model="state.form.updatedId" :label="t('更新者id')"></q-input>
<q-input dense outlined v-model="state.form.updatedBy" :label="t('更新者')"></q-input>
<q-input dense outlined v-model="state.form.updatedTime" :label="t('更新时间')"></q-input>

            

            <q-btn class="float-right" :label="t(state.dialogTitle)" color="primary" type="submit" />
            <q-card-actions class="w-full"></q-card-actions>
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </div>
</template>
<script lang="ts">
// 声明额外的选项
export default {
  name:'audit'
}
</script>
<script lang="ts" setup>
import { AddAudit, DeleteAuditByIds, GetAuditById, GetAuditPage, UpdateAudit, Audit } from '@/api/audit'
import { ref, defineComponent, toRefs, reactive, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { dateFormat } from '@/utils/date-util'
import { useQuasar } from 'quasar'
import { ls } from '@/utils'
import { PageRequest } from '@/utils/page-request'
import { FilterCondition, FilterOperate, ListSortType } from '@/utils/page-request/enums'

const AUDIT_VISIBLE_COLUMNS = `audit_visibleColumns`

// 默认显示的Table列
const defaultVisibleColumns = ['userRoles','serviceName','methodName','path','body','query','httpMethod','returnValue','executionMs','clientIpAddress','exception','id','createdId','createdBy','createdTime','updatedId','updatedBy','updatedTime']

// Form表单默认内容
const defaultForm: Audit = {
}

const $q = useQuasar()
const t = useI18n().t
const columns = [
    {
    label: t('用户持有角色'),
    name: 'userRoles',
    field: 'userRoles',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('服务 (类/接口) 名'),
    name: 'serviceName',
    field: 'serviceName',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('执行方法名称'),
    name: 'methodName',
    field: 'methodName',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('请求路径'),
    name: 'path',
    field: 'path',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('Body参数'),
    name: 'body',
    field: 'body',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('Query参数'),
    name: 'query',
    field: 'query',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('Http请求方法'),
    name: 'httpMethod',
    field: 'httpMethod',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('返回值'),
    name: 'returnValue',
    field: 'returnValue',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('方法调用的总持续时间（毫秒）'),
    name: 'executionMs',
    field: 'executionMs',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('客户端的IP地址'),
    name: 'clientIpAddress',
    field: 'clientIpAddress',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('方法执行期间发生异常'),
    name: 'exception',
    field: 'exception',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('id主键'),
    name: 'id',
    field: 'id',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('创建者id'),
    name: 'createdId',
    field: 'createdId',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('创建者'),
    name: 'createdBy',
    field: 'createdBy',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('创建时间'),
    name: 'createdTime',
    field: 'createdTime',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('更新者id'),
    name: 'updatedId',
    field: 'updatedId',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('更新者'),
    name: 'updatedBy',
    field: 'updatedBy',
    sortable: true,
    align: 'center',
    textClasses: '',
  },{
    label: t('更新时间'),
    name: 'updatedTime',
    field: 'updatedTime',
    sortable: true,
    align: 'center',
    textClasses: '',
  },
  {
    label: t('操作'),
    name: 'actions',
    align: 'center',
    required: true,
  },
] as any[]
const state = reactive({
  columns,
  visibleColumns: (ls.getItem(AUDIT_VISIBLE_COLUMNS) || defaultVisibleColumns) as string[],
  // table选中项
  selected: [] as Audit[],
  // table 数据
  audits: [] as Audit[],
  loading: false,
  // 搜索框值
  searchKey: '',
  // 搜索过滤
  pageRequest: new PageRequest(1, 10, [
    {
      field: 'name',
      value: '',
      operate: FilterOperate.contains,
      condition: FilterCondition.or,
    },
  ]),
  pagination: {
    sortBy: 'id',
    descending: false,
    page: 1,
    rowsPerPage: 10,
    rowsNumber: 1,
    totalPages: 1,
  },
  dialogVisible: false,
  dialogTitle: '添加',
  form: { ...defaultForm },
})

watch(
  () => state.visibleColumns,
  (v, ov) => {
    ls.set(AUDIT_VISIBLE_COLUMNS, v)
  },
)

// 排序触发事件
const tableHandler = async ({ pagination, filter }) => {
  state.pagination = pagination
  await getAudits()
}

// 获取Audit数据
const getAudits = async () => {
  const { pageRequest } = state

  pageRequest.setAllRulesValue(state.searchKey)
  pageRequest.setOrder(state.pagination)

  state.loading = true
  const res = await GetAuditPage(pageRequest)
  state.loading = false
  res.notifyOnErr()
  if (res.succeeded) {
    state.audits = res.data?.items as Audit[]
    state.pagination.rowsNumber = res.data?.totalCount as number
    state.pagination.totalPages = res.data?.totalPages as number
  }
}

// 显示 dialog
const showDialog = async (type: string, id?: number) => {
  if (type == t('添加')) {
    state.form = { ...defaultForm }
  } else {
    const res = await GetAuditById(id as number)
    res.notifyOnErr()
    if (!res.succeeded) return
    state.form = { ...res.data } as Audit
  }
  state.dialogTitle = type
  state.dialogVisible = true
}

// dialog form 表单提交
const dialogFormSubmit = async () => {
  const type = state.dialogTitle
  let res
  if (type == t('添加')) {
    res = await AddAudit({ ...state.form })
  } else {
    res = await UpdateAudit({ ...state.form })
  }
  res.notify()
  state.dialogVisible = !res?.succeeded
  if (res?.succeeded) getAudits()
}

// 根据Id 数组删除 Audit
const deleteByIds = (ids: number[]) => {
  $q.dialog({
    message:
      ids.length > 1
        ? `${t('已选中')}${ids.length}，${t('确定要删除这些数据吗')}`
        : t('确定要删除这个数据吗'),
  }).onOk(async () => {
    state.loading = true
    const res = await DeleteAuditByIds(ids)
    state.loading = false
    res.notify()
    if (res.succeeded) getAudits()
  })
}

// 获取数据
getAudits()

</script>
<style lang="sass"></style>
