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
public class QueayFilterRecursionField_Test
{
    //0 for false, 1 for true.
    private static int usingResource = 0;

    public List<Student> students { get; set; } = new List<Student>()
    {
        new Student()
        {
            Id = 1,
            Grade = new Grade()
            {
                Id= 2,
            },
            Grades = new List<Grade>()
            {
                new Grade()
                {
                    Id=  3,
                    Students = new List<Student>()
                    {
                        new Student()
                        {
                            Id = 4,
                        }
                    }
                }
            }
        },
        new Student()
        {
            Id = 5
        }
    };

    [Fact]
    public void collection_Test()
    {
        var ids = new[] { 3 };

        Expression<Func<Student, bool>> targetExp = (p) => p.Grades.Any(g => ids.Contains(g.Id));


        var request = new Request()
        {
            Filters = new List<FilterRule>
            {
                new FilterRule(@$"id",ids,FilterOperate.In),
            }
        };

        var exp = request.GetRulesExpression<Student>(null);

        var lambdaStr = exp.ToLambdaString();

        var data = students.AsQueryable().Where(exp).ToList();

    }

    [Fact]
    public void RecursionField_Test()
    {
        var ids = new[] { 4, 3 };

        Expression<Func<Student, bool>> targetExp = (p) => p.Grades.Any(g => g.Students.Any(s => ids.Contains(s.Id)));


        var request = new Request()
        {
            Filters = new List<FilterRule>
            {
                new FilterRule(@$"Grade.Student.Grade.id","3",FilterOperate.Equal)
            }
        };

        var exp = request.GetRulesExpression<Student>(null);

        var lambdaStr = exp.ToLambdaString();

        var data = students.AsQueryable().Where(exp).ToList();

    }


}
