<template>
  <div class="h-full">
    <q-table
      v-model:selected.sync="state.selected"
      v-model:pagination="state.pagination"
      :rows="state.users"
      :columns="columns"
      selection="multiple"
      :loading="state.loading"
      :row-key="(v) => v.id"
      :visible-columns="state.visibleColumns"
      flat
      square
      :rows-per-page-options="[10, 15, 50, 500, 1000, 10000]"
      table-header-class="bg-gray-100"
      class="h-full relative sticky-header-column-table sticky-right-column-table"
      :virtual-scroll="state.users.length >= 100"
      @request="tableHandler"
    >
      <template #loading>
        <q-inner-loading showing color="primary" />
      </template>

      <template #top>
        <div class="w-full flex flex-row justify-between">
          <div class="flex flex-row space-x-1">
            <q-input
              v-model="state.searchKey"
              outlined
              :label="`${t('用户名')}/${t('账号')}`"
              dense
              @keyup.enter="getUsers"
            />
            <q-btn icon="search" color="primary" @click="getUsers" />
            <q-btn color="primary" :label="t('添加')" @click="showDialog(t('添加'))" />
            <q-btn color="danger" :label="t('删除')" @click="deleteByIds(state.selected.map((s) => s.id))" />
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
              :options="columns"
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
import { reactive, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { dateFormat } from '@/utils/date-util'
import { useQuasar } from 'quasar'
import { ls } from '@/utils'
import { PageRequest } from '@/utils/page-request'
import { FilterCondition, FilterOperate } from '@/utils/page-request/enums'
import { staticRoles } from '@/router/routes'

const USER_VISIBLE_COLUMNS = `USER_VISIBLE_COLUMNS`
const defaultVisibleColumns = ['name', 'account', 'roles', 'enabled', 'sex', 'birth', 'createdTime']
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
  lastLoginTime: undefined,
  id: 0,
  createdId: 0,
  createdTime: '',
  updatedId: 0,
  updatedTime: ''
}

const $q = useQuasar()
const t = useI18n().t
const columns: any[] = [
  {
    label: t('用户名'),
    name: 'name',
    field: 'name',
    sortable: true,
    align: 'center',
    textClasses: ''
  },
  {
    label: t('账号'),
    name: 'account',
    field: 'account',
    sortable: true,
    align: 'center'
  },
  {
    label: t('持有角色'),
    name: 'roles',
    field: 'roles',
    sortable: true,
    align: 'center'
  },
  {
    label: t('状态'),
    name: 'enabled',
    field: 'enabled',
    sortable: true,
    align: 'center'
  },
  {
    label: t('邮箱'),
    name: 'email',
    field: 'email',
    sortable: true,
    align: 'center'
  },
  {
    label: t('手机号'),
    name: 'phone',
    field: 'phone',
    sortable: true,
    align: 'center'
  },
  {
    label: t('备注'),
    name: 'description',
    field: 'description',
    sortable: true,
    align: 'center'
  },
  {
    label: t('性别'),
    name: 'sex',
    field: 'sex',
    sortable: true,
    align: 'center'
  },
  {
    label: t('年龄'),
    name: 'age',
    field: 'age',
    sortable: true,
    format: (v: number) => `${v > 0 ? v : ''}`,
    align: 'center'
  },
  {
    label: t('生日'),
    name: 'birth',
    field: 'birth',
    format: (v: string) => `${v ? dateFormat(v) : ''}`,
    sortable: true,
    align: 'center'
  },
  {
    label: t('创建时间'),
    name: 'createdTime',
    field: 'createdTime',
    sortable: true,
    align: 'center'
  },
  {
    label: t('修改时间'),
    name: 'updatedTime',
    field: 'updatedTime',
    sortable: true,
    align: 'center'
  },
  {
    label: t('操作'),
    name: 'actions',
    align: 'center',
    required: true
  }
]

const state = reactive({
  visibleColumns: (ls.getItem(USER_VISIBLE_COLUMNS) || defaultVisibleColumns) as string[],
  selected: [] as User[],
  users: [] as User[],
  loading: false,
  searchKey: '',
  pageRequest: new PageRequest(1, 10, [
    {
      field: 'name',
      value: '123',
      operate: FilterOperate.contains
    },
    {
      field: 'account',
      value: '',
      operate: FilterOperate.contains
    },
    {},
    {
      field: 'createdTime',
      value: '2023-02-12',
      operate: FilterOperate.greater,
      condition: FilterCondition.and
    },
    {
      field: 'createdTime',
      value: '2023-02-12',
      operate: FilterOperate.greater,
      condition: FilterCondition.and
    }
  ]),
  pagination: {
    sortBy: 'id',
    descending: false,
    page: 1,
    rowsPerPage: 10,
    rowsNumber: 1,
    totalPages: 1
  },
  dialogVisible: false,
  dialogTitle: '添加',
  form: { ...defaultForm }
})

watch(
  () => state.visibleColumns,
  (v, ov) => {
    ls.set(USER_VISIBLE_COLUMNS, v)
  }
)

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const tableHandler = async ({ pagination }: any) => {
  state.pagination = pagination
  await getUsers()
}

const getUsers = async () => {
  const { pageRequest } = state

  // pageRequest.setAllRulesValue(state.searchKey, ['createdTime'])
  pageRequest.setOrder(state.pagination)

  state.loading = true
  const res = await GetUserPage(pageRequest)
  state.loading = false
  res.notifyOnErr()
  if (res.succeeded) {
    state.users = res.data?.items as User[]
    state.pagination.rowsNumber = res.data?.totalCount as number
    state.pagination.totalPages = res.data?.totalPages as number
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
    state.loading = true
    const res = await DeleteUserByIds(ids)
    state.loading = false
    res.notify()
    if (res.succeeded) getUsers()
  })
}

getUsers()

const roles = computed(() => Object.values<string>(staticRoles))
</script>
<style lang="sass" scoped></style>
