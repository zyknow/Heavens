using Heavens.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Entities;
/// <summary>
/// QueryFilter 测试实体
/// </summary>
public class Grade
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public DateTimeOffset OffsetTime { get; set; }

    public DateTime Time { get; set; }

    public virtual ICollection<Student> Students { get; set; }

    public virtual Student Student { get; set; }

}
