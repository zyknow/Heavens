using Bing;
using Bing.Expressions;
using Bing.Extensions;
using Heavens.Core.Entities;
using Heavens.Core.Extension.Extensions;
using Heavens.Core.Extension.QueayFilter.common;
using Heavens.Core.Extension.QueayFilter.helper;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Heavens.Tests.Core;
public class QueayFilter_Test
{
    [Fact]
    public void Collection_Test()
    {
        var list = new[] { 1, 2, 3 };

        // 目标表达式
        Expression<Func<Student, bool>> exp = e => e.Grades.Any(p => list.Contains(p.Id));


        var data = new List<Student>()
        {
            new Student()
            {
                Id = 1,
                Grades = new List<Grade>()
                {
                    new Grade()
                    {
                        Id = 1,
                    },
                    new Grade()
                    {
                        Id = 2,
                    },
                    new Grade()
                    {
                        Id = 3,
                    }
                }
            },
            new Student()
            {
                Id = 2,
                Grades = new List<Grade>()
                {
                    new Grade()
                    {
                        Id = 4,
                    },
                    new Grade()
                    {
                        Id = 5,
                    },
                    new Grade()
                    {
                        Id = 6,
                    }
                }
            }
        };


        var req = new Request();
        req.Filters = new List<FilterRule>()
        {
            new FilterRule("Grades.Id",list,FilterOperate.In),
        };
        var genExp = req.GetRulesExpression<Student>();
        var res = data.AsQueryable().Where(genExp).ToList();
        Assert.True(res.Count == 1);
    }
}
