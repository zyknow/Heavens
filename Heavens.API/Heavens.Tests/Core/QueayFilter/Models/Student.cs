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
public class Student
{
    public int Id { get; set; }
    public int? IdNull { get; set; }

    public short ShortId { get; set; }
    public short? ShortNullId { get; set; }

    public long LongId { get; set; }
    public long? LongNullId { get; set; }

    public double? MarkNull { get; set; }
    public double Mark { get; set; }

    public decimal? MoneyNull { get; set; }
    public decimal Money { get; set; }

    public bool Sex { get; set; }

    public DateTimeOffset OffsetTime { get; set; }

    public DateTime Time { get; set; }

    public string Name { get; set; } = string.Empty;

    public enum IsDie
    {
        Live,
        Die
    }

    public IsDie _IsDie { get; set; }

    public Guid GradeId { get; set; }

    public virtual Grade Grade { get; set; }

    public virtual ICollection<Grade> Grades { get; set; }

}
