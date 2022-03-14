using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Entities.Base;
public class BaseEntityProp<TKey>
{
    
    public TKey Id { get; set; }
    public int? CreatedId { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTime? CreatedTime { get; set; } = DateTime.Now;
    public int? UpdatedId { get; set; }
    public string UpdatedBy { get; set; } = "";
    public DateTime? UpdatedTime { get; set; } = DateTime.Now;
}

public class BaseEntityProp : BaseEntityProp<int>
{

}
