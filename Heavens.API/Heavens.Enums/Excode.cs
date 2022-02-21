

using Furion.FriendlyException;
using System.ComponentModel;

namespace Heavens.Enums;

/// <summary>
/// 异常状态码
/// </summary>
[ErrorCodeType]
public enum Excode
{
    #region 通用
    /// <summary>
    /// 非开发模式
    /// </summary>
    [ErrorCodeItemMetadata("非开发模式")]
    NON_DEVELOPER_MODE,
    /// <summary>
    /// 数据不存在
    /// </summary>
    [ErrorCodeItemMetadata("数据不存在")]
    DATA_NO_EXSITS,
    /// <summary>
    /// 添加数据失败
    /// </summary>
    [ErrorCodeItemMetadata("添加数据失败")]
    ADD_DATA_FAILD,
    /// <summary>
    /// 资源键值重复
    /// </summary>
    [ErrorCodeItemMetadata("资源键值重复")]
    RESOURCE_KEY_REPEAT,
    /// <summary>
    /// 未包含文件
    /// </summary>
    [ErrorCodeItemMetadata("未包含文件")]
    NO_INCLUD_FILE,
    /// <summary>
    /// 存在相同文件
    /// </summary>
    [ErrorCodeItemMetadata("存在相同文件")]
    EXSIT_FILE,
    /// <summary>
    /// 未包含相关数据
    /// </summary>
    [ErrorCodeItemMetadata("未包含相关数据")]
    NO_INCLUD_CHILDREN_DATA,
    /// <summary>
    /// 请求token无效
    /// </summary>
    [ErrorCodeItemMetadata("请求token无效")]
    TOKEN_INVALID,
    /// <summary>
    /// 传入数据为空
    /// </summary>
    [ErrorCodeItemMetadata("传入数据为空")]
    REQUEST_DATA_EMPTY,
    /// <summary>
    /// 传入参数不正确
    /// </summary>
    [ErrorCodeItemMetadata("传入参数不正确")]
    REQUEST_DARA_ERROR,
    /// <summary>
    /// 指定的属性“{0}”在类型“{1}”中不存在
    /// </summary>
    [Description("指定的属性“{0}”在类型“{1}”中不存在")]
    FIELD_IN_TYPE_NOT_FOUND,
    /// <summary>
    /// 查询的值类型“{0}”未找到转换器
    /// </summary>
    [Description("查询的值类型“{0}”未找到转换器")]
    QUERY_VALUE_TYPE_NO_FIND_CONVERTER,
    #endregion

    #region User
    /// <summary>
    /// 用户被禁用
    /// </summary>
    [ErrorCodeItemMetadata("用户被禁用")]
    USER_DISABLED,
    /// <summary>
    /// 用户名或密码错误
    /// </summary>
    [ErrorCodeItemMetadata("用户名或密码错误")]
    USER_NAME_OR_PASSWD_ERROR,
    /// <summary>
    /// 旧密码错误
    /// </summary>
    [ErrorCodeItemMetadata("旧密码错误")]
    USER_OLD_PASSWD_ERROR,
    /// <summary>
    /// 用户名重复
    /// </summary>
    [ErrorCodeItemMetadata("用户名重复")]
    USER_NAME_REPEAT,
    #endregion

    #region Authroze
    /// <summary>
    /// 刷新token不存在或已过期
    /// </summary>
    [ErrorCodeItemMetadata("刷新token不存在或已过期")]
    REFRESHTOKEN_NO_EXIST_OR_EXPIRE,
    #endregion

    #region Db
    /// <summary>
    /// soft 字段内容不对
    /// </summary>
    //[ErrorCodeItemMetadata("soft 字段内容不对，请查看示例 示例： [Id asc]|[Id desc]|[Id OrderByDescending]|[Id OrderBy]")]
    //QUERY_BY_SOFT_FIELD_ERROR,

    [ErrorCodeItemMetadata("定义的queryActions参数名称不同，将无法转换")]
    QUERY_ACTION_PARAM_ERROR,

    #endregion


}
