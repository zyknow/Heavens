using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Entities.Base;
public class BaseEntityProp<TKey>
{
    /// <summary>
    /// id主键
    /// </summary>
    public TKey Id { get; set; }
    /// <summary>
    /// 创建者id
    /// </summary>
    public int? CreatedId { get; set; }
    /// <summary>
    /// 创建者
    /// </summary>
    public string CreatedBy { get; set; } = "";
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.Now;
    /// <summary>
    /// 更新者id
    /// </summary>
    public int? UpdatedId { get; set; }
    /// <summary>
    /// 更新者
    /// </summary>
    public string UpdatedBy { get; set; } = "";
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.Now;
}

public class BaseEntityProp : BaseEntityProp<int>
{

}
