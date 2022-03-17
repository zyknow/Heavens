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
    [Fact]
    public void RecursionField_Test()
    {
        var ids = new[] { 4, 3 };

        Expression<Func<Student, bool>> targetExp = (p) => p.Grades.Any(g => g.Students.Any(s => ids.Contains(s.Id)));

        var fields = new[] { "id" , "Grade.id", "Grade.student.grade.id", "Grades.id", "Grades.students.Grade.id", "Grades.students.Grade.id" };

        var request = new Request()
        {
            Filters = new List<FilterRule>
            {
                new FilterRule(@$"Grade.id","3",FilterOperate.Equal)
            }
        };

        foreach (var field in fields)
        {
            request.Filters.First().Field = field;

            var exp = request.GetRulesExpression<Student>();
            var lambdaStr = exp.ToLambdaString();

            Assert.NotNull(lambdaStr);

        }
    }


}
