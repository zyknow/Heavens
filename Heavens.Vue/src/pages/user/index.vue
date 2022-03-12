<template>
  <div class="h-full">
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
      square
      :rows-per-page-options="[10, 15, 50, 500, 1000, 10000]"
      table-header-class="bg-gray-100"
      class="h-full relative sticky-header-column-table sticky-right-column-table"
      :virtual-scroll="pageQuery.data.length >= 100"
      @request="tableHandler"
    >
      <template #loading>
        <q-inner-loading showing color="primary" />
      </template>

      <template #top>
        <query-filter
          :base-query="pageQuery"
          :loading="pageQuery.loading"
          easy-text-input-class="w-60"
          @on-search="getUsers"
        >
          <template #btn>
            <q-btn color="primary" :label="t('添加')" @click="showDialog(t('添加'))" />
            <q-btn color="danger" :label="t('删除')" @click="deleteByIds(pageQuery.selected.map((s) => s.id))" />
          </template>
        </query-filter>
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
          menu-anchor="bottom middle"
          menu-self="bottom middle"
          class="absolute right-4 top-3"
        />
      </template>

      <template #pagination>
        <q-pagination
          v-model="pageQuery.pagination.page"
          color="primary"
          :max-pages="9"
          :max="pageQuery.pagination.totalPages"
          boundary-numbers
          @click="getUsers"
        />
      </template>

      <template #body-cell-enabled="props">
        <!-- 状态显示为Icon -->
        <q-td :props="props" class="w-1">
          <q-icon
            size="2rem"
            :color="props.row.enabled ? 'success' : 'danger'"
            :name="props.row.enabled ? 'r_face_retouching_natural' : 'r_face_retouching_off'"
          />
        </q-td>
        <!-- 状态显示为文字 -->
        <!-- <q-td :props="props" class="w-1">
          <q-chip dense outline square :color="props.row.enabled ? 'success' : 'danger'">
            {{ props.row.enabled ? '启用' : '禁用' }}
          </q-chip>
        </q-td>-->
      </template>

      <template #body-cell-sex="props">
        <!-- 状态显示为Icon -->
        <q-td :props="props" class="w-1">
          <q-icon size="2rem" :color="props.row.sex ? 'primary' : 'pink-3'" :name="props.row.sex ? 'male' : 'female'" />
        </q-td>
      </template>

      <template #body-cell-roles="props">
        <q-td :props="props">
          <q-chip v-for="role in props.row.roles" :key="role" dense outline square color="primary">{{ role }}</q-chip>
        </q-td>
      </template>

      <template #body-cell-actions="props">
        <q-td :props="props" class="space-x-1 w-1">
          <q-btn dense color="primary" icon="edit" @click="showDialog(t('编辑'), props.row.id)" />
          <q-btn dense color="danger" icon="remove" @click="deleteByIds([props.row.id])" />
        </q-td>
      </template>
    </q-table>

    <q-dialog v-model="state.dialogVisible">
      <q-card class="w-2/4">
        <q-card-section>
          <div class="text-h6">{{ t(state.dialogTitle) }}</div>
        </q-card-section>
        <q-separator />

        <q-card-section class="q-pt-none mt-8">
          <q-form class="space-y-2" @submit="dialogFormSubmit">
            <q-input v-model="state.form.name" dense outlined :label="t('用户名')" />
            <q-input
              v-model="state.form.account"
              dense
              outlined
              :label="t('账号')"
              :rules="[(v: string) => v.length > 0 || t('必填')]"
            />
            <q-input v-model="state.form.passwd" dense outlined :label="t('密码')" />
            <q-select
              v-model="state.form.roles"
              outlined
              multiple
              :options="roles"
              :label="t('持有角色')"
              use-chips
              color="primary"
            />
            <q-toggle
              v-model="state.form.enabled"
              size="4rem"
              :color="state.form.enabled ? 'success' : 'danger'"
              :label="t('状态')"
              left-label
              :icon="state.form.enabled ? 'r_face_retouching_natural' : 'r_face_retouching_off'"
            />

            <!-- <q-input dense outlined v-model="name" :label="t('状态')" ></q-input> -->
            <q-input v-model="state.form.email" dense outlined :label="t('邮箱')" />
            <q-input v-model="state.form.phone" dense outlined :label="t('手机')" />
            <q-input v-model="state.form.description" dense outlined type="textarea" :label="t('备注')" />
            <q-btn class="float-right" :label="t(state.dialogTitle)" color="primary" type="submit" />
            <q-card-actions class="w-full" />
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </div>
</template>
<script lang="ts">
export default {
  name: 'User'
}
</script>
<script lang="ts" setup>
import { AddUser, DeleteUserByIds, GetUserById, GetUserPage, UpdateUser, User } from '@/api/user'
import { reactive, computed, watch, onBeforeUnmount } from 'vue'
import { useI18n } from 'vue-i18n'
import { useQuasar } from 'quasar'
import { staticRoles } from '@/router/routes'
import { FieldOption, FieldType, Operate } from '@/utils/page-request/typing'
import { PageQuery } from '@/utils/page-request/query'
import QueryFilter from '@/components/query/query-filter.vue'

const $q = useQuasar()
const t = useI18n().t

const defaultForm: User = {
  name: '',
  account: '',
  passwd: '',
  enabled: false,
  roles: [],
  email: '',
  phone: '',
  sex: false,
  description: '',
  id: 0,
  createdBy: '',
  createdTime: '',
  updatedBy: ''
}

const pageQuery = reactive(
  new PageQuery<User>([
    {
      field: 'name',
      label: '用户名',
      type: FieldType.text,
      operate: Operate.contains,
      easy: true
    },
    {
      field: 'account',
      label: '账号',
      type: FieldType.text,
      operate: Operate.contains,
      easy: true
    },
    {
      field: 'enabled',
      label: '状态',
      type: FieldType.boolSelect,
      operate: Operate.equal,

      selectOptions: [
        {
          label: '启用',
          value: 'true'
        },
        {
          label: '禁用',
          value: 'false'
        }
      ]
    },
    {
      field: 'email',
      label: '邮箱',
      type: FieldType.text,
      operate: Operate.contains,
      easy: true
    },
    {
      field: 'phone',
      label: '手机号',
      type: FieldType.text,
      operate: Operate.contains,
      easy: true
    },
    {
      field: 'description',
      label: '备注',
      type: FieldType.text,
      operate: Operate.contains,
      easy: true
    },
    {
      field: 'sex',
      label: '性别',
      type: FieldType.boolSelect,
      operate: Operate.equal,

      selectOptions: [
        {
          label: '男性',
          value: 'true'
        },
        {
          label: '女性',
          value: 'false'
        }
      ]
    },
    {
      field: 'createdTime',
      label: '创建时间',
      type: FieldType.date,
      operate: Operate.equal
    },
    {
      field: 'updatedTime',
      label: '修改时间',
      type: FieldType.date,
      operate: Operate.equal
    },
    {
      label: t('操作'),
      field: 'actions',
      columns: { required: true, sortable: false },
      excludeQuery: true
    } as FieldOption
  ])
)
onBeforeUnmount(() => {
  pageQuery.saveOption()
})

const state = reactive({
  dialogVisible: false,
  dialogTitle: '添加',
  form: { ...defaultForm }
})

const tableHandler = async ({ pagination }: any) => {
  pageQuery.pagination = pagination
  await getUsers()
}

const getUsers = async () => {
  pageQuery.loading = true
  const res = await GetUserPage(pageQuery.toPageRequest())
  pageQuery.loading = false
  res.notifyOnErr()
  if (res.succeeded) {
    pageQuery.data = res.data?.items as User[]
    pageQuery.pagination.rowsNumber = res.data?.totalCount as number
    pageQuery.pagination.totalPages = res.data?.totalPages as number
  }
}

const showDialog = async (type: string, id?: number) => {
  if (type == t('添加')) {
    state.form = { ...defaultForm }
  } else {
    const res = await GetUserById(id as number)
    res.notifyOnErr()
    if (!res.succeeded) return
    state.form = { ...res.data } as User
  }
  state.dialogTitle = type
  state.dialogVisible = true
}

const dialogFormSubmit = async () => {
  const type = state.dialogTitle
  let res
  if (type == t('添加')) {
    res = await AddUser({ ...state.form })
  } else {
    res = await UpdateUser({ ...state.form })
  }
  res.notify()
  state.dialogVisible = !res?.succeeded
  if (res?.succeeded) getUsers()
}

const deleteByIds = (ids: number[]) => {
  $q.dialog({
    message: ids.length > 1 ? `${t('已选中')}${ids.length}，${t('确定要删除这些数据吗')}` : t('确定要删除这个数据吗')
  }).onOk(async () => {
    pageQuery.loading = true
    const res = await DeleteUserByIds(ids)
    pageQuery.loading = false
    res.notify()
    if (res.succeeded) getUsers()
  })
}

getUsers()

const roles = computed(() => Object.values<string>(staticRoles))
</script>
<style lang="sass" scoped></style>
