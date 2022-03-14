using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Tests.Core.SearchEngine;
public class SE_Student
{
    public int Id { get; set; }
    public string Name { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTimeOffset CreateTimeOffset { get; set; }

    public decimal Grade { get; set; }

    public SE_Sex Sex { get; set; }

    public List<string> StringList { get; set; }
}


public enum SE_Sex
{
    Female,
    Male,
}
