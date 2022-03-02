using Bing;
using Heavens.Core.Entities.Base;
using Heavens.Core.Extension.Bing.CsCommentReader;
using System.Text.RegularExpressions;

namespace Heavens.Application._Framework.CodeGenApp.Services;

public class CodeGenService : ICodeGenService, IScoped
{
    public CodeGenService(IWebHostEnvironment env)
    {
        _env = env;
    }

    private IWebHostEnvironment _env { get; }
    private string ProjectPath { get; set; }
    private string GetProjectPath()
    {
        if (!_env.IsNull() && !_env.IsDevelopment())
        {
            throw Oops.Bah("非开发环境无法使用");
        }

        if (!ProjectPath.IsEmpty())
        {
            return ProjectPath;
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);

        while (directoryInfo.Name != "Heavens")
        {
            directoryInfo = directoryInfo.Parent;
        }

        ProjectPath = directoryInfo.FullName;
        return ProjectPath;
    }

    public void GenApplication(string path)
    {
        // 获取所有实体
        List<Type> entities = Assembly.Load("Heavens.Core").GetTypes().Where(p => p.Namespace == "Heavens.Core.Entities" && p.IsClass && p.Name != "BaseEntity" && !p.Name.Contains("<>")).ToList();

        string applicationPath = Path.Combine(GetProjectPath(), "Heavens.API", "Heavens.Application");

        List<string> entityNames = entities.Select(e => e.Name).ToList();
        foreach (Type item in entities)
        {
            string apidirName = @$"{item.Name}App";

            string apiDirPath = path.IsEmpty() ? Path.Combine(applicationPath, apidirName) : Path.Combine(path, apidirName);


            #region 创建文件夹目录

            if (!Directory.Exists(apiDirPath))
            {
                Directory.CreateDirectory(apiDirPath);
            }

            string dtoPath = Path.Combine(apiDirPath, "Dtos");
            if (!Directory.Exists(dtoPath))
            {
                Directory.CreateDirectory(dtoPath);
            }

            string servicesPath = Path.Combine(apiDirPath, "Services");
            if (!Directory.Exists(servicesPath))
            {
                Directory.CreateDirectory(servicesPath);
            }
            #endregion


            string apiCsName = @$"{item.Name}AppService";
            string[] entityCodeLines;


            #region 获取实体行
            try
            {
                entityCodeLines = File.ReadAllLines(Path.Combine(GetProjectPath(), "Heavens.API", "Heavens.Core", "Entities", @$"{item.Name}.cs"), Encoding.UTF8);

                entityCodeLines = entityCodeLines.Skip(entityCodeLines.IndexOf(e => e.Contains("public class ")) + 1).ToArray();

                entityCodeLines = entityCodeLines.Where(e => e.Contains("get;") && e.Contains("set;")).Select(s =>
                 {
                     string entityName = entityNames.FirstOrDefault(e => s.Contains(@$" {e} ") || s.Contains(@$"<{e}>"));
                     if (!entityName.IsEmpty())
                     {
                         return new Regex(entityName).Replace(s, @$"{entityName}Dto", 1);
                     }

                     return s;
                 }).ToArray();

            }
            catch (Exception)
            {
                throw Oops.Bah("实体名称必须和cs文件名一致");
            }

            #endregion

            #region apiCsCode

            string apiCsCode =
@$"using Heavens.Application.{apidirName}.Services;
using Heavens.Application.{apidirName}.Dtos;
namespace Heavens.Application.{apidirName};

/// <summary>
/// {CsCommentReader.Create(item)?.Summary ?? item.Name}接口
/// </summary>
[Authorize]
public class {apidirName}Service : {(item.BaseType.Name.Contains("BaseEntity") ? @$"BaseAppService<{item.Name}, {item.Name}Dto>" : "IDynamicApiController")}, IScoped
{{
    {(item.BaseType.Name.Contains("BaseEntity") ?
    @$"public I{item.Name}Service _{item.Name.ToCamelCase()}Service {{ get; set; }}
    public {apidirName}Service(IRepository<{item.Name}> {item.Name.ToCamelCase()}Repository,I{item.Name}Service {item.Name.ToCamelCase()}Service) : base({item.Name.ToCamelCase()}Repository)
    {{
        _{item.Name.ToCamelCase()}Service = {item.Name.ToCamelCase()}Service;
    }}
    " : "")}
    
}}";

            #endregion

            #region mapperCsCode

            string mapperCsCode =
$@"namespace Heavens.Application.{apidirName}.Dtos;

public class Mapper : IRegister
{{
    public void Register(TypeAdapterConfig config)
    {{

    }}
}}";
            #endregion

            #region dtoCsCode

            List<PropertyInfo> props = item.GetProperties().WhereIf(p => !item.BaseType.GetProperties().Any(bp => bp.Name == p.Name), item.BaseType == typeof(BaseEntity)).ToList();

            List<string> usingDtoNames = entityCodeLines.Where(line => line.Contains(@$"Dto")).Select(s =>
            {
                return entityNames.First(e => s.Contains(@$"{e}Dto"));
            }).ToList();

            string dtoCsCode =
@$"using Heavens.Core.Entities.Base;
{string.Join("", usingDtoNames.Select(entityName => @$"using Heavens.Application.{entityName}App.Dtos;"))}

namespace Heavens.Application.{apidirName}.Dtos;
/// <summary>
/// {CsCommentReader.Create(item)?.Summary ?? item.Name} Dto
/// </summary>
public class {item.Name}Dto {(item.BaseType.Name.Contains("BaseEntity") ? ": BaseEntityProp" : "")}
{{
{string.Join("", props.Select(p =>
 $@"{(CsCommentReader.Create(p)?.Summary != null ?
 @$"
    /// <summary>
    /// {CsCommentReader.Create(p)?.Summary}
    /// </summary>" : "")}
    {entityCodeLines.First(line => line.Contains(@$"{p.Name}") || line.Contains(@$"{p.Name}Dto")).SafeString()}"))}
}}";
            #endregion

            #region serviceCsCode && iserviceCsCode

            string serviceCsCode =
$@"namespace Heavens.Application.{apidirName}.Services;

public class {item.Name}Service : I{item.Name}Service
{{
}}
";
            string iserviceCsCode =
@$"namespace Heavens.Application.{apidirName}.Services;

public interface I{item.Name}Service
{{
}}
";
            #endregion

            #region 生成文件


            string apiCsPath = Path.Combine(apiDirPath, @$"{apiCsName}.cs");

            Console.WriteLine($@"start-------------{item.Name}-------------");

            if (!File.Exists(apiCsPath))
            {
                File.WriteAllText(apiCsPath, apiCsCode, Encoding.UTF8);
                Console.WriteLine(@$"生成：{apiCsPath}");
            }

            string mapperCsPath = Path.Combine(apiDirPath, "Dtos", "Mapper.cs");
            if (!File.Exists(mapperCsPath))
            {
                File.WriteAllText(mapperCsPath, mapperCsCode, Encoding.UTF8);
                Console.WriteLine(@$"生成：{mapperCsPath}");
            }

            string dtoCsPath = Path.Combine(apiDirPath, "Dtos", @$"{item.Name}Dto.cs");
            if (!File.Exists(dtoCsPath))
            {
                File.WriteAllText(dtoCsPath, dtoCsCode, Encoding.UTF8);
                Console.WriteLine(@$"生成：{dtoCsPath}");
            }

            string serviceCsPath = Path.Combine(apiDirPath, "Services", @$"{item.Name}Service.cs");
            string iserviceCsPath = Path.Combine(apiDirPath, "Services", @$"I{item.Name}Service.cs");
            if (!File.Exists(serviceCsPath))
            {
                File.WriteAllText(serviceCsPath, serviceCsCode, Encoding.UTF8);
                Console.WriteLine(@$"生成：{serviceCsPath}");

            }
            if (!File.Exists(iserviceCsPath))
            {
                File.WriteAllText(iserviceCsPath, iserviceCsCode, Encoding.UTF8);
                Console.WriteLine(@$"生成：{iserviceCsPath}");

            }

            Console.WriteLine($@"end-------------{item.Name}-------------");

            #endregion

        }
    }


    public void GenVueApi(string path)
    {
        Assembly assembly = Assembly.Load("Heavens.Application");
        IEnumerable<Type> types = assembly.GetTypes().Where(a => a.Namespace.Contains("Dtos") && a.Name != "Mapper" && !a.Name.Contains("<>"));


        string apiPath = path.IsEmpty() ? Path.Combine(GetProjectPath(), "Heavens.Vue", "src", "api") : Path.Combine(path, "api");

        if (!Directory.Exists(apiPath))
        {
            Directory.CreateDirectory(apiPath);
        }

        List<Type> allDtos = types.Where(d => d.IsClass).ToList();
        List<Type> allEnums = types.Where(d => d.IsEnum).ToList();

        IEnumerable<Type> appServices = assembly.GetTypes().Where(a => !a.IsAbstract && a.Name.Contains("AppService") && !a.Name.Contains("<>") && a.Name != "CodeGenAppService");

        foreach (Type appService in appServices)
        {
            //var appRoute = @$"api/{appService.Name.Replace("AppService", "").ToSnakeCase().Replace("_", "-")}";


            List<Type> dtos = allDtos.Where(d => d.Namespace == @$"{appService.Namespace}.Dtos").ToList();
            List<Type> enums = allEnums.Where(d => d.Namespace == @$"{appService.Namespace}.Dtos").ToList();

            List<string> importotherInterfaces = dtos.Select(d => d.GetProperties().Where(p => CsTypeToTs(p.PropertyType).Contains("Dto"))).SelectMany(p => p.Select(p => CsTypeToTs(p.PropertyType).Replace("Dto", "").Replace("[]", ""))).ToList();

            string imports =
@$"import {{ IndexSign }} from '@/typing'
import {{ PageRequest }} from '@/utils/page-request'
import request from 'src/utils/request'
import {{ BaseEntity,PagedList, RequestResult }} from './_typing'
{string.Join("", importotherInterfaces.Select(p =>
@$"import {{ {p} }} from './{p.ToCamelCase()}'"
))}
";

            string dtoStr = GetDtoStr(dtos);
            string enumStr = GetEnumStr(enums);
            string methodStr = GetMethodStr(appService);
            string fileStr = imports + enumStr + dtoStr + methodStr;

            string fileName = @$"{appService.Name.Replace("AppService", "").ToCamelCase()}.ts";
            if (!File.Exists(Path.Combine(apiPath, fileName)))
            {
                File.WriteAllText(Path.Combine(apiPath, fileName), fileStr, Encoding.UTF8);
            }
        }
    }

    public void GenVuePage(string path)
    {
        string pagePath = path.IsEmpty() ? Path.Combine(GetProjectPath(), "Heavens.Vue", "src", "pages") : Path.Combine(path, "api");

        List<Type> entities = Assembly.Load("Heavens.Core").GetTypes().Where(p => p.Namespace == "Heavens.Core.Entities" && p.IsClass && p.Name != "BaseEntity" && !p.Name.Contains("<>")).ToList();

        var entityNames = entities.Select(e => e.Name).ToList();
        Assembly assembly = Assembly.Load("Heavens.Application");
        IEnumerable<Type> types = assembly.GetTypes().Where(a => a.Namespace.Contains("Dtos") && a.Name != "Mapper" && !a.Name.Contains("<>") && entityNames.Contains(a.Name.Replace("Dto", "")));

        foreach (Type type in types)
        {
            var props = type.GetProperties();
            string str = @$"
<template>
  <div class=""p-2 h-full"">
    <!-- Table -->
    <q-table
      :rows=""state.[name]s""
      :columns=""state.columns""
      selection=""multiple""
      :loading=""state.loading""
      :row-key=""v => v.id""
      :visible-columns=""state.visibleColumns""
      v-model:selected.sync=""state.selected""
      flat
      @request=""tableHandler""
      v-model:pagination=""state.pagination""
      :rows-per-page-options=""[10, 15, 50, 500, 1000, 10000]""
      table-header-class=""bg-gray-100""
      class=""h-full relative sticky-header-column-table sticky-right-column-table""
      :virtual-scroll=""state.[name]s.length >= 200""
    >
      <template #loading>
        <q-inner-loading showing color=""primary"" />
      </template>

      <template #top>
        <div class=""w-full flex flex-row justify-between"">
          <div class=""flex flex-row space-x-1"">
            <q-input
              outlined
              :label=""``""
              dense
              v-model=""state.searchKey""
              @keyup.enter=""get[Name]s""
            ></q-input>
            <q-btn icon=""search"" color=""primary"" @click=""get[Name]s"" />
            <q-btn color=""primary"" :label=""t('添加')"" @click=""showDialog(t('添加'))"" />
            <q-btn
              color=""danger""
              :label=""t('删除')""
              @click=""deleteByIds(state.selected.map(s => s.id))""
            />
          </div>
          <div>
            <q-select
              v-model=""state.visibleColumns""
              multiple
              outlined
              dense
              options-dense
              :display-value=""$q.lang.table.columns""
              emit-value
              map-options
              :options=""state.columns""
              option-value=""name""
              options-cover
              class=""float-right""
              menu-anchor=""bottom middle""
              menu-self=""bottom middle""
            />
          </div>
        </div>
      </template>

      <template #pagination>
        <q-pagination
          v-model=""state.pagination.page""
          color=""primary""
          :max-pages=""9""
          :max=""state.pagination.totalPages""
          boundary-numbers
          @click=""get[Name]s""
        />
      </template>

      <template #body-cell-actions=""props"">
        <q-td :props=""props"" class=""space-x-1 w-1"">
          <q-btn dense color=""primary"" icon=""edit"" @click=""showDialog(t('编辑'), props.row.id)"" />
          <q-btn dense color=""danger"" icon=""remove"" @click=""deleteByIds([props.row.id])"" />
        </q-td>
      </template>

      <!-- <template #body-cell-sex=""props"">
        <q-td :props=""props"" class=""w-1"">
          <q-icon
            size=""2rem""
            :color=""props.row.sex ? 'primary' : 'pink-3'""
            :name=""props.row.sex ? 'male' : 'female'""
          ></q-icon>
        </q-td> 
      </template> -->

    </q-table>

    <!-- Dialog -->
    <q-dialog v-model=""state.dialogVisible"">
      <q-card class=""w-2/4"">
        <q-card-section>
          <div class=""text-h6"">{{{{ t(state.dialogTitle) }}}}</div>
        </q-card-section>
        <q-separator></q-separator>

        <q-card-section class=""q-pt-none mt-8"">
          <q-form class=""space-y-2"" @submit=""dialogFormSubmit"">
            {string.Join("", props.Select(p =>
                            @$"<q-input dense outlined v-model=""state.form.{p.Name.ToCamelCase()}"" :label=""t('{CsCommentReader.Create(p).Summary ?? p.Name.ToCamelCase() }')""></q-input>"
                            + "\r\n"))}
            

            <q-btn class=""float-right"" :label=""t(state.dialogTitle)"" color=""primary"" type=""submit"" />
            <q-card-actions class=""w-full""></q-card-actions>
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </div>
</template>
<script lang=""ts"">
// 声明额外的选项
export default {{
  name:'[name]'
}}
</script>
<script lang=""ts"" setup>
import {{ Add[Name], Delete[Name]ByIds, Get[Name]ById, Get[Name]Page, Update[Name], [Name] }} from '@/api/[name]'
import {{ ref, defineComponent, toRefs, reactive, computed, watch }} from 'vue'
import {{ useI18n }} from 'vue-i18n'
import {{ dateFormat }} from '@/utils/date-util'
import {{ useQuasar }} from 'quasar'
import {{ ls }} from '@/utils'
import {{ PageRequest }} from '@/utils/page-request'
import {{ FilterCondition, FilterOperate, ListSortType }} from '@/utils/page-request/enums'

const [NAME]_VISIBLE_COLUMNS = `[name]_visibleColumns`

// 默认显示的Table列
const defaultVisibleColumns = [{string.Join("", props.Select(p => @$"'{p.Name.ToCamelCase()}'"))}]

// Form表单默认内容
const defaultForm: [Name] = {{
}}

const $q = useQuasar()
const t = useI18n().t
const columns = [
    {string.Join("", props.Select(p => @$"{{
    label: t('{CsCommentReader.Create(p).Summary ?? p.Name.ToCamelCase() }'),
    name: '{p.Name.ToCamelCase()}',
    field: '{p.Name.ToCamelCase()}',
    sortable: true,
    align: 'center',
    textClasses: '',
  }},"))}
  {{
    label: t('操作'),
    name: 'actions',
    align: 'center',
    required: true,
  }},
] as any[]
const state = reactive({{
  columns,
  visibleColumns: (ls.getItem([NAME]_VISIBLE_COLUMNS) || defaultVisibleColumns) as string[],
  // table选中项
  selected: [] as [Name][],
  // table 数据
  [name]s: [] as [Name][],
  loading: false,
  // 搜索框值
  searchKey: '',
  // 搜索过滤
  pageRequest: new PageRequest(1, 10, [
    {{
      field: 'name',
      value: '',
      operate: FilterOperate.contains,
      condition: FilterCondition.or,
    }},
  ]),
  pagination: {{
    sortBy: 'id',
    descending: false,
    page: 1,
    rowsPerPage: 10,
    rowsNumber: 1,
    totalPages: 1,
  }},
  dialogVisible: false,
  dialogTitle: '添加',
  form: {{ ...defaultForm }},
}})

watch(
  () => state.visibleColumns,
  (v, ov) => {{
    ls.set([NAME]_VISIBLE_COLUMNS, v)
  }},
)

// 排序触发事件
const tableHandler = async ({{ pagination, filter }}) => {{
  state.pagination = pagination
  await get[Name]s()
}}

// 获取[Name]数据
const get[Name]s = async () => {{
  const {{ pageRequest }} = state

  pageRequest.setAllRulesValue(state.searchKey)
  pageRequest.setOrder(state.pagination)

  state.loading = true
  const res = await Get[Name]Page(pageRequest)
  state.loading = false
  res.notifyOnErr()
  if (res.succeeded) {{
    state.[name]s = res.data?.items as [Name][]
    state.pagination.rowsNumber = res.data?.totalCount as number
    state.pagination.totalPages = res.data?.totalPages as number
  }}
}}

// 显示 dialog
const showDialog = async (type: string, id?: number) => {{
  if (type == t('添加')) {{
    state.form = {{ ...defaultForm }}
  }} else {{
    const res = await Get[Name]ById(id as number)
    res.notifyOnErr()
    if (!res.succeeded) return
    state.form = {{ ...res.data }} as [Name]
  }}
  state.dialogTitle = type
  state.dialogVisible = true
}}

// dialog form 表单提交
const dialogFormSubmit = async () => {{
  const type = state.dialogTitle
  let res
  if (type == t('添加')) {{
    res = await Add[Name]({{ ...state.form }})
  }} else {{
    res = await Update[Name]({{ ...state.form }})
  }}
  res.notify()
  state.dialogVisible = !res?.succeeded
  if (res?.succeeded) get[Name]s()
}}

// 根据Id 数组删除 [Name]
const deleteByIds = (ids: number[]) => {{
  $q.dialog({{
    message:
      ids.length > 1
        ? `${{t('已选中')}}${{ids.length}}，${{t('确定要删除这些数据吗')}}`
        : t('确定要删除这个数据吗'),
  }}).onOk(async () => {{
    state.loading = true
    const res = await Delete[Name]ByIds(ids)
    state.loading = false
    res.notify()
    if (res.succeeded) get[Name]s()
  }})
}}

// 获取数据
get[Name]s()

</script>
<style lang=""sass""></style>
";
            str = str
                .Replace("[NAME]", type.Name.Replace("Dto","").ToUpper())
                .Replace("[Name]", type.Name.Replace("Dto", "").ToUpperFirstLetter())
                .Replace("[name]", type.Name.Replace("Dto", "").ToCamelCase());

            string dirPath = Path.Combine(pagePath, type.Name.Replace("Dto", "").ToCamelCase());
            string filePath = Path.Combine(dirPath, "index.vue");

            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                File.WriteAllText(filePath, str, Encoding.UTF8);
            }
        }
    }


    private string GetMethodStr(Type appType)
    {
        if (appType.BaseType.IsNull())
        {
            return "";
        }

        string appName = appType.Name.Replace("AppService", "");


        return @$"
// /**
//  * 获取分页
//  */
export async function Get{appName.ToUpperFirstLetter()}Page(req: PageRequest): Promise<RequestResult<PagedList<{appName.ToUpperFirstLetter()}>>> {{
  return request.post<any, RequestResult<PagedList<{appName.ToUpperFirstLetter()}>>>('/api/{appName.ToCamelCase()}/page', req)
}}

/**
 * 添加
 */
export async function Add{appName.ToUpperFirstLetter()}({appName.ToCamelCase()}: {appName.ToUpperFirstLetter()}): Promise<RequestResult<{appName.ToUpperFirstLetter()}>> {{
  return request.post<{appName.ToUpperFirstLetter()}, RequestResult<{appName.ToUpperFirstLetter()}>>('/api/{appName.ToCamelCase()}', {appName.ToCamelCase()})
}}

/**
 * 编辑
 */
export async function Update{appName.ToUpperFirstLetter()}({appName.ToCamelCase()}: {appName.ToUpperFirstLetter()}): Promise<RequestResult<{appName.ToUpperFirstLetter()}>> {{
  return request.put<{appName.ToUpperFirstLetter()}, RequestResult<{appName.ToUpperFirstLetter()}>>('/api/{appName.ToCamelCase()}', {appName.ToCamelCase()})
}}

/**
 * 根据Id查询
 */
export async function Get{appName.ToUpperFirstLetter()}ById(id: number): Promise<RequestResult<{appName.ToUpperFirstLetter()}>> {{
  return request.get<number, RequestResult<{appName.ToUpperFirstLetter()}>>('/api/{appName.ToCamelCase()}/by-id', {{ params: {{ id }} }})
}}

/**
 * 获取所有
 */
export async function GetAll{appName.ToUpperFirstLetter()}(): Promise<RequestResult<{appName.ToUpperFirstLetter()}[]>> {{
  return request.get<any, RequestResult<{appName.ToUpperFirstLetter()}[]>>('/api/{appName.ToUpperFirstLetter()}/all')
}}

/**
 * 删除
 */
export async function Delete{appName.ToUpperFirstLetter()}ByIds(ids: number[]): Promise<RequestResult<number>> {{
  return request.request<any, RequestResult<number>>({{
    url: '/api/{appName.ToCamelCase()}/by-ids',
    method: 'delete',
    data: ids,
  }})
}}";
    }

    private string GetDtoStr(List<Type> types)
    {
        string @interface = "";

        foreach (Type type in types)
        {
            //var props = type.GetProperties().WhereIf(t => !typeof(BaseEntityProp).GetProperties().Any(p => p == t), type.BaseType == typeof(BaseEntityProp));

            IEnumerable<PropertyInfo> props = type.GetProperties().Where(t => !typeof(BaseEntityProp).GetProperties().Select(p => p.Name).Contains(t.Name));

            @interface +=
    $@"/**
 * {CsCommentReader.Create(type)?.Summary?.Replace("Dto", "")}
 */
export interface {type.Name.Replace("Dto", "")} extends IndexSign{(type.BaseType == typeof(BaseEntityProp) ? ", BaseEntity" : "")} {{
  {string.Join("", props.Select(prop =>
    @$"
  /**
  * {CsCommentReader.Create(prop)?.Summary}
  */
  {prop.Name.ToCamelCase().Replace("Dto", "")}: {CsTypeToTs(prop.PropertyType).Replace("Dto", "")}"
    ))}
}}
";
        }

        return @interface;
    }

    private string GetEnumStr(List<Type> types)
    {
        string enumStr = "";

        foreach (Type type in types)
        {
            string propsStr = "";

            foreach (object item in type.GetEnumValues())
            {
                propsStr +=
    @$"/**
  * 
  */
  {Enum.GetName(type, item)} = {item},";
            }

            enumStr +=
    $@"/**
 * {CsCommentReader.Create(type)?.Summary}
 */
export enum {type.Name}{{
{propsStr}
}}
";
        }

        return enumStr;
        //    export enum Direction
        //{
        //    Up = 'UP',
        //    Right = 'RIGHT',
        //    Down = 'DOWN',
        //    Left = 'LEFT',
        //}
    }

    private string CsTypeToTs(Type csType)
    {
        string tsType = "";

        // 集合类型或者nullble包裹的类型
        if (csType.IsNullableType() || csType.IsCollectionType() && Type.GetTypeCode(csType) != TypeCode.String)
        {
            Type argumentType = csType.GetGenericArguments().FirstOrDefault();
            if (!argumentType.IsNull())
            {
                if (csType.IsArray || csType.IsCollectionType() && Type.GetTypeCode(csType) != TypeCode.String)
                {

                    if (!argumentType.IsNull())
                    {
                        return @$"{CsTypeToTs(argumentType)}[]";
                    }
                }
                else
                {
                    return CsTypeToTs(argumentType);
                }
            }
        }



        switch (Type.GetTypeCode(csType))
        {
            case TypeCode.Boolean:
                tsType = nameof(TypeCode.Boolean).ToCamelCase();
                break;
            case TypeCode.String:
            case TypeCode.Char:
                tsType = nameof(TypeCode.String).ToCamelCase();
                break;
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
                tsType = "Number".ToCamelCase();
                break;
            case TypeCode.DateTime:
                tsType = "Date";
                break;
            default:
                tsType = csType.Name;
                break;
        }
        return tsType;
    }
}
