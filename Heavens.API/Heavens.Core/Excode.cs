

using Furion.FriendlyException;
using System.ComponentModel;

namespace Heavens.Core;

/// <summary>
/// 异常状态码
/// </summary>
[ErrorCodeType]
public enum Excode
{
    #region 通用
    [ErrorCodeItemMetadata("非开发模式")]
    NON_DEVELOPER_MODE,
    [ErrorCodeItemMetadata("数据不存在")]
    DATA_NO_EXSITS,
    [ErrorCodeItemMetadata("添加数据失败")]
    ADD_DATA_FAILD,
    [ErrorCodeItemMetadata("资源键值重复")]
    RESOURCE_KEY_REPEAT,
    [ErrorCodeItemMetadata("未包含文件")]
    NO_INCLUD_FILE,
    [ErrorCodeItemMetadata("请求token无效")]
    TOKEN_INVALID,
    [ErrorCodeItemMetadata("传入数据为空")]
    REQUEST_DATA_EMPTY,
    [Description("指定的属性“{0}”在类型“{1}”中不存在")]
    FIELD_IN_TYPE_NOT_FOUND,
    [Description("查询的值类型“{0}”未找到转换器")]
    QUERY_VALUE_TYPE_NO_FIND_CONVERTER,
    #endregion

    #region User
    [ErrorCodeItemMetadata("用户被禁用")]
    USER_DISABLED,
    [ErrorCodeItemMetadata("用户名或密码错误")]
    USER_NAME_OR_PASSWD_ERROR,
    [ErrorCodeItemMetadata("旧密码错误")]
    USER_OLD_PASSWD_ERROR,
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
    [ErrorCodeItemMetadata("soft 字段内容不对，请查看示例 示例： [Id asc]|[Id desc]|[Id OrderByDescending]|[Id OrderBy]")]
    QUERY_BY_SOFT_FIELD_ERROR,

    #endregion

}
