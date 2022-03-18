using Bing;
using Bing.Expressions;
using Bing.Extensions;
using Bing.Text;
using Furion.FriendlyException;
using Furion.JsonSerialization;
using Heavens.Core.Entities;
using Heavens.Core.Extension.Extensions;
using Heavens.Core.Extension.QueayFilter;
using Heavens.Core.Extension.QueayFilter.common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Profiling.Internal;
using System.Buffers;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Heavens.Tests.Core;
public class QueayFilter_Test
{
    /// <summary>
    /// 查询的数据源
    /// </summary>
    List<Student> data = new List<Student>()
        {
            new Student()
            {
                Id = 1,
                Name = "测试2",
                Grades = new List<Grade>()
                {
                    new Grade()
                    {
                        Id = 1,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 11,
                                Name = "student中的grade中的student"
                            }
                        }
                    },
                    new Grade()
                    {
                        Id = 2,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 12,
                                Name = "student中的grade中的student2"
                            }
                        }
                    },
                    new Grade()
                    {
                        Id = 3,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 13,
                                Name = "student中的grade中的student3"
                            }
                        }
                    }
                },
                Time = DateTime.Now
            },
            new Student()
            {
                Id = 2,
                Name = "2测试222222222",
                Grades = new List<Grade>()
                {
                    new Grade()
                    {
                        Id = 4,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 14,
                                Name = "student中的grade中的student4"
                            }
                        }
                    },
                    new Grade()
                    {
                        Id = 5,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 15,
                                Name = "student中的grade中的student5"
                            }
                        }
                    },
                    new Grade()
                    {
                        Id = 6,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 16,
                                Name = "student中的grade中的student6"
                            }
                        }
                    }
                },
                Time = DateTime.Now
            },
            new Student()
            {
                Id = 3,
                Name = "王五.3",
                Grades = new List<Grade>()
                {
                    new Grade()
                    {
                        Id = 7,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 17,
                                Name = "student中的grade中的student7"
                            }
                        }
                    },
                    new Grade()
                    {
                        Id = 8,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 18,
                                Name = "student中的grade中的student8"
                            }
                        }
                    },
                    new Grade()
                    {
                        Id = 9,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 19,
                                Name = "student中的grade中的student9"
                            }
                        }
                    }
                },
                Time = DateTime.Today.AddDays(-1)
            },
            new Student()
            {
                Id = 4,
                Name = "六.4",
                Grades = new List<Grade>()
                {
                    new Grade()
                    {
                        Id = 10,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 20,
                                Name = "student中的grade中的student10"
                            }
                        }
                    },
                    new Grade()
                    {
                        Id = 11,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 21,
                                Name = "student中的grade中的student11"
                            }
                        }
                    },
                    new Grade()
                    {
                        Id = 12,
                        Students = new List<Student>()
                        {
                            new Student()
                            {
                                Id = 22,
                                Name = "student中的grade中的student12"
                            }
                        }
                    }
                },
                Time = DateTime.Today.AddDays(-1)
            }
        };

    private static void testAssert(List<Student> text, List<Student> req)
    {
        if (text.Count == req.Count)
        {
            Assert.Equal(text.Select(s => s.Id), req.Select(s => s.Id));
        }
        else
        {
            throw Oops.Oh($"手写表达式：{text.Count},动态生成表达式：{req.Count},返回结果数不一样");
        }
    }

    [Fact]
    public void Collection_Test()
    {
        var list = new[] { 1, 2, 3 };

        // 目标表达式
        Expression<Func<Student, bool>> exp = e => e.Grades!.Any(p => list.Contains(p.Id));

        var req = new Request();
        req.Filters = new List<FilterRule>()
        {
            new FilterRule("Grades.Id",list,FilterOperate.In),
        };
        var genExp = req.GetRulesExpression<Student>();
        var res = data.AsQueryable().Where(genExp).ToList();
        Assert.True(res.Count == 1);
    }

    /// <summary>
    /// where条件
    /// </summary>
    [Fact]
    public void Custom_Request_Where()
    {
        var value = "测试";

        // 目标表达式
        Expression<Func<Student, bool>> testExpression = s => !s.Name.Contains(value) && s.Name.Contains("王五");

        #region 这是用户请求的Request

        var sut = new Request {
            Filters = new List<FilterRule>()//用户筛选条件
            {
                new FilterRule(){
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.NotContains,
                    Value = value
                },
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Contains,
                    Value = "王五"
                }
            }
        };
        #endregion

        // 根据Request生成表达式树
        var reqExpression = sut.GetRulesExpression<Student>();

        //根据表达式筛选数据
        var testResult = data.AsQueryable().Where(testExpression).ToList();
        var requestResult = data.AsQueryable().Where(reqExpression).ToList();

        testAssert(testResult, requestResult);
    }

    /// <summary>
    /// SortBy条件
    /// </summary>
    [Fact]
    public void Custom_Request_SortBy()
    {
        // 目标表达式
        Expression<Func<Student, bool>> testExpression = s => true;

        #region 这是用户请求的Request

        var requestDesc = new Request
        {
            Sort = new SortBy() //排序条件
            {
                Field = nameof(Student.Id),
                SortType = SortType.Desc
            }
        };

        var requestAsc = new Request()
        {
            Sort = new SortBy()
            {
                Field = nameof(Student.Id),
                SortType = SortType.Asc
            }
        };

        #endregion

        #region 后台自定义的表达式

        var queryActions = new List<IQueryAction<Student>>()
        {
            new QueryAction<Student, int, int>(nameof(Student.Id),p=> p.Id, p => p.Id),
        };

        #endregion

        // 根据Request生成表达式树
        var reqExpression = requestDesc.GetRulesExpression<Student>();

        //根据表达式筛选数据
        var testDescResult = data.AsQueryable().Where(testExpression).OrderByDescending(s => s.Id).ToList();
        var requestDescResult = data.AsQueryable().Where(reqExpression).SortBy(requestDesc.Sort).ToList();

        var testAscResult = data.AsQueryable().Where(testExpression).OrderBy(s => s.Id).ToList();
        var requestAscResult = data.AsQueryable().Where(reqExpression).SortBy(requestAsc.Sort).ToList();

        testAssert(testDescResult, requestDescResult);
        testAssert(testAscResult, requestAscResult);

    }

    /// <summary>
    /// WhereAndSortBy条件
    /// </summary>
    [Fact]
    public void Custom_Request_WhereAndSortBy()
    {
        //条件
        var value = "测试";
        var startTime = DateTime.Today;
        var endTime = startTime.AddDays(1);

        //目标表达式
        Expression<Func<Student, bool>> testExpression = s => s.Id == 2 && s.Name.Contains(value) && s.Time >= startTime && s.Time <= endTime;

        #region 这是用户请求的Request

        var sut = new Request
        {
            Filters = new List<FilterRule>()//用户筛选条件
            {
                new FilterRule()
                {
                    Field = nameof(Student.Id),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = 2
                },
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Contains,
                    Value = value
                },
                new FilterRule()
                {
                    Field = nameof(Student.Time),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.GreaterOrEqual,
                    Value = startTime
                },
                new FilterRule()
                {
                    Field = nameof(Student.Time),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.LessOrEqual,
                    Value = endTime
                }
            },
            Sort = new SortBy()
            {
                Field = nameof(Student.Id),
                SortType = SortType.Asc
            },
            Limit = 3
        };

        #endregion

        #region 后台自定义的表达式

        var queryActions = new List<IQueryAction<Student>>()
        {
            new QueryAction<Student,int,bool>(nameof(Student.Id), s => s.Id, s => s.Id > 1)
        };

        #endregion

        // 根据Request生成表达式树
        var reqExpression = sut.GetRulesExpression<Student>(queryActions);

        var testStr = testExpression.ToLambdaString();
        var reqStr = reqExpression.ToLambdaString();

        //根据表达式筛选数据
        var testResult = data.AsQueryable().Where(testExpression).OrderBy(s => s.Id).ToList();
        var requestResult = data.AsQueryable().Where(reqExpression).SortBy(sut.Sort, queryActions).ToList();

        testAssert(testResult, requestResult);
    }

    /// <summary>
    /// 异常
    /// </summary>
    [Fact]
    public void Custom_Request_Exception()
    {
        var appFriendlyException = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = new List<int>() {1}
                }
            }
        };

        var nullAttribute = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Money),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.GreaterOrEqual,
                    Value = (double)1
                }
            }
        };

        var guid = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.GradeId),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Contains,
                    Value = Guid.Empty
                }
            }
        };

        var startsWith = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.StartsWith,
                    Value = new List<int>(){1}
                }
            }
        };

        var endsWith = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.EndsWith,
                    Value = new List<int>(){1}
                }
            }
        };

        var nullString = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.EndsWith,
                    Value = null
                }
            }
        };

        var stringEmpty = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Contains,
                    Value = string.Empty
                }
            }
        };

        var containsType = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Id),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Contains,
                    Value = 1
                }
            }
        };

        var notContainsType = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Id),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.NotContains,
                    Value = 1
                }
            }
        };

        var startWithType = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Id),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.StartsWith,
                    Value = 11
                }
            }
        };

        var endsWithType = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Id),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.EndsWith,
                    Value = 1
                }
            }
        };

        Assert.IsNotType<Exception>(nullAttribute.GetRulesExpression<Student>());
        Assert.Throws<AppFriendlyException>(() => appFriendlyException.GetRulesExpression<Student>());
        Assert.Throws<NotSupportedException>(() => guid.GetRulesExpression<Student>());
        Assert.Throws<AppFriendlyException>(() => startsWith.GetRulesExpression<Student>());
        Assert.Throws<AppFriendlyException>(() => endsWith.GetRulesExpression<Student>());
        testAssert(data.AsQueryable().Where(s => true).ToList(), data.AsQueryable().Where(nullString.GetRulesExpression<Student>()).ToList());
        testAssert(data.AsQueryable().Where(s => true).ToList(), data.AsQueryable().Where(stringEmpty.GetRulesExpression<Student>()).ToList());
        Assert.Throws<NotSupportedException>(() => containsType.GetRulesExpression<Student>());
        Assert.Throws<NotSupportedException>(() => endsWithType.GetRulesExpression<Student>());
        Assert.Throws<NotSupportedException>(() => startWithType.GetRulesExpression<Student>());
        Assert.Throws<NotSupportedException>(() => notContainsType.GetRulesExpression<Student>());
    }

    /// <summary>
    /// 条件
    /// </summary>
    [Fact]
    public void Custom_Request_FilterHelper()
    {
        var value = "测试2";
        var time = DateTime.Today;
        var idList = new[] { 1, 6, 9 };

        var equal = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = value
                }
            }
        };

        var notEqual = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.NotEqual,
                    Value = value
                }
            }
        };

        var less = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Time),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Less,
                    Value = time
                }
            }
        };

        var greater = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Time),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Greater,
                    Value = time
                }
            }
        };

        var lessOrEqual = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Time),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.LessOrEqual,
                    Value = time
                }
            }
        };

        var greaterOrEqual = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Time),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.GreaterOrEqual,
                    Value = time
                }
            }
        };

        var startsWith = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.StartsWith,
                    Value = value
                }
            }
        };

        var endsWith = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.EndsWith,
                    Value = 2
                }
            }
        };

        var contains = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Contains,
                    Value = value
                }
            }
        };

        var notContains = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.NotContains,
                    Value = value
                }
            }
        };

        var include = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Id),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.In,
                    Value = idList
                }
            }
        };

        //equal
        testAssert(data.AsQueryable().Where(s => s.Name.Equals(value)).ToList(), data.AsQueryable().Where(equal.GetRulesExpression<Student>()).ToList());
        //notEqual
        testAssert(data.AsQueryable().Where(s => !s.Name.Equals(value)).ToList(), data.AsQueryable().Where(notEqual.GetRulesExpression<Student>()).ToList());
        //less
        testAssert(data.AsQueryable().Where(s => s.Time < time).ToList(), data.AsQueryable().Where(less.GetRulesExpression<Student>()).ToList());
        //greater
        testAssert(data.AsQueryable().Where(s => s.Time > time).ToList(), data.AsQueryable().Where(greater.GetRulesExpression<Student>()).ToList());
        //lessOrEqual
        testAssert(data.AsQueryable().Where(s => s.Time <= time).ToList(), data.AsQueryable().Where(lessOrEqual.GetRulesExpression<Student>()).ToList());
        //greaterOrEqual
        testAssert(data.AsQueryable().Where(s => s.Time >= time).ToList(), data.AsQueryable().Where(greaterOrEqual.GetRulesExpression<Student>()).ToList());
        //startsWith
        testAssert(data.AsQueryable().Where(s => s.Name.StartsWith(value)).ToList(), data.AsQueryable().Where(startsWith.GetRulesExpression<Student>()).ToList());
        //endsWith
        testAssert(data.AsQueryable().Where(s => s.Name.EndsWith("2")).ToList(), data.AsQueryable().Where(endsWith.GetRulesExpression<Student>()).ToList());
        //contains
        testAssert(data.AsQueryable().Where(s => s.Name.Contains(value)).ToList(), data.AsQueryable().Where(contains.GetRulesExpression<Student>()).ToList());
        //notContains
        testAssert(data.AsQueryable().Where(s => !s.Name.Contains(value)).ToList(), data.AsQueryable().Where(notContains.GetRulesExpression<Student>()).ToList());
        //in
        testAssert(data.AsQueryable().Where(s => idList.Contains(s.Id)).ToList(), data.AsQueryable().Where(include.GetRulesExpression<Student>()).ToList());
    }

    /// <summary>
    /// 各种类型的Value
    /// </summary>
    [Fact]
    public void Custom_Request_JsonElement()
    {
        var json = "{\"name\":\"测试\", \"money\":4.52, \"id\":1, \"time\":\"2022-01-02\", \"guid\":\"6F9619FF-8B86-D011-B42D-00C04FC964FF\", \"sex\": true, \"ids\":[1,2]}";
        var jsonString = "{\"names\":[\"测试\",\"测试1\",\"测试2\"]}";

        /*var jArray = JsonConvert.DeserializeObject<JObject>(jsonString)["names"].ToArray();*/
        var jArray = new JArray();
        jArray.Add("测试");
        var jDoc = JsonDocument.Parse(json);

        Array array = new [] {1, 2};
        var arrayInt = new[] { 1, 2 };
        var str = new List<string>()
        {
            "测试",
            "测试2",
            "测试3"
        };

        #region 从JsonElement提取数据

        //int
        var intElement = jDoc.RootElement.GetProperty("id");
        //string
        var stringElement = jDoc.RootElement.GetProperty("name");
        //double
        var doubleElement = jDoc.RootElement.GetProperty("money");
        //DateTime
        var dateElement = jDoc.RootElement.GetProperty("time");
        //guid
        var guidElement = jDoc.RootElement.GetProperty("guid");
        //bool
        var boolElement = jDoc.RootElement.GetProperty("sex");
        //array
        var arrayElement = jDoc.RootElement.GetProperty("ids");

        #endregion

        #region 各种类型的筛选条件

        var shortRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.ShortId),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = intElement
                }
            }
        };

        var shortNullRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.ShortNullId),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = intElement
                }
            }
        };

        var intRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Id),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = intElement
                }
            }
        };

        var intNullRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.IdNull),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = intElement
                }
            }
        };

        var longRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.LongId),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = intElement
                }
            }
        };

        var longNullRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.LongNullId),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = intElement
                }
            }
        };

        var decimalRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Money),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.LessOrEqual,
                    Value = doubleElement
                }
            }
        };

        var decimalNullRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.MoneyNull),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.LessOrEqual,
                    Value = doubleElement
                }
            }
        };

        var doubleRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Mark),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.LessOrEqual,
                    Value = doubleElement
                }
            }
        };

        var doubleNullRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.MarkNull),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.LessOrEqual,
                    Value = doubleElement
                }
            }
        };

        var stringRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Contains,
                    Value = stringElement
                }
            }
        };

        var dateRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Time),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.GreaterOrEqual,
                    Value = dateElement
                }
            }
        };

        var guidRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.GradeId),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = guidElement
                }
            }
        };

        var boolRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Sex),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = boolElement
                }
            }
        };

        var arrayRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Id),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.In,
                    Value = array
                }
            }
        };

        var arrayElementRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Id),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.In,
                    Value = arrayElement
                }
            }
        };

        var enumElementRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student._IsDie),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = intElement
                }
            }
        };

        var jArrayElementRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = nameof(Student.Name),
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.In,
                    Value = jArray
                }
            }
        };

        #endregion


        //short
        testAssert(data.AsQueryable().Where(s => s.ShortNullId.Equals(1)).ToList(), data.AsQueryable().Where(shortNullRequest.GetRulesExpression<Student>()).ToList());
        testAssert(data.AsQueryable().Where(s => s.ShortId.Equals(1)).ToList(), data.AsQueryable().Where(shortRequest.GetRulesExpression<Student>()).ToList());
        //int
        testAssert(data.AsQueryable().Where(s => s.Id.Equals(1)).ToList(), data.AsQueryable().Where(intRequest.GetRulesExpression<Student>()).ToList());
        testAssert(data.AsQueryable().Where(s => s.IdNull.Equals(1)).ToList(), data.AsQueryable().Where(intNullRequest.GetRulesExpression<Student>()).ToList());
        //long
        testAssert(data.AsQueryable().Where(s => s.LongId.Equals(1)).ToList(), data.AsQueryable().Where(longRequest.GetRulesExpression<Student>()).ToList());
        testAssert(data.AsQueryable().Where(s => s.LongNullId.Equals(1)).ToList(), data.AsQueryable().Where(longNullRequest.GetRulesExpression<Student>()).ToList());
        //decimal
        testAssert(data.AsQueryable().Where(s => s.Money <= (decimal)4.52).ToList(), data.AsQueryable().Where(decimalRequest.GetRulesExpression<Student>()).ToList());
        testAssert(data.AsQueryable().Where(s => s.MoneyNull <= (decimal)4.52).ToList(), data.AsQueryable().Where(decimalNullRequest.GetRulesExpression<Student>()).ToList());
        //double
        testAssert(data.AsQueryable().Where(s => s.Mark <= 4.52).ToList(), data.AsQueryable().Where(doubleRequest.GetRulesExpression<Student>()).ToList());
        testAssert(data.AsQueryable().Where(s => s.MarkNull <= 4.52).ToList(), data.AsQueryable().Where(doubleNullRequest.GetRulesExpression<Student>()).ToList());
        //string
        testAssert(data.AsQueryable().Where(s => s.Name.Contains("测试")).ToList(), data.AsQueryable().Where(stringRequest.GetRulesExpression<Student>()).ToList());
        //DateTime
        testAssert(data.AsQueryable().Where(s => s.Time >= new DateTime(2022, 1, 2)).ToList(), data.AsQueryable().Where(dateRequest.GetRulesExpression<Student>()).ToList());
        //guid
        testAssert(data.AsQueryable().Where(s => s.GradeId == new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF")).ToList(), data.AsQueryable().Where(guidRequest.GetRulesExpression<Student>()).ToList());
        //bool
        testAssert(data.AsQueryable().Where(s => s.Sex == true).ToList(), data.AsQueryable().Where(boolRequest.GetRulesExpression<Student>()).ToList());
        //array
        testAssert(data.AsQueryable().Where(s => arrayInt.Contains(s.Id)).ToList(), data.AsQueryable().Where(arrayRequest.GetRulesExpression<Student>()).ToList());
        testAssert(data.AsQueryable().Where(s => arrayInt.Contains(s.Id)).ToList(), data.AsQueryable().Where(arrayElementRequest.GetRulesExpression<Student>()).ToList());
        //enum
        testAssert(data.AsQueryable().Where(s => s._IsDie == Student.IsDie.Die).ToList(), data.AsQueryable().Where(enumElementRequest.GetRulesExpression<Student>()).ToList());
        //jArray
        /*testAssert(data.AsQueryable().Where(s => str.Contains(s.Name)).ToList(), data.AsQueryable().Where(jArrayElementRequest.GetRulesExpression<Student>()).ToList());*/
    }

    [Fact]
    public void Custom_Request_Vaule()
    {
        var request = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = "Grades.Students.Id",
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = 11
                }
            }
        };

        var stringRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = "Grades.Students.Name",
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = "student中的grade中的student6"
                }
            }
        };

        var nameOfRequest = new Request()
        {
            Filters = new List<FilterRule>()
            {
                new FilterRule()
                {
                    Field = $"{nameof(Student.Grades)}.{nameof(Grade.Students)}.{nameof(Student.Name)}",
                    Condition = FilterCondition.And,
                    Operate = FilterOperate.Equal,
                    Value = "student中的grade中的student6"
                }
            }
        };

        Assert.Equal(data.AsQueryable().Where(s => s.Grades.Any(g => g.Students.Any(s => s.Id == 11))).ToList().Select(s => s.Grades.Select(s => s.Students.Select(s => s.Id))), data.AsQueryable().Where(request.GetRulesExpression<Student>()).ToList().Select(s => s.Grades.Select(s => s.Students.Select(s => s.Id))));
        Assert.Equal(data.AsQueryable().Where(s => s.Grades.Any(g => g.Students.Any(s => s.Name.Equals("student中的grade中的student6")))).ToList().Select(s => s.Grades.Select(s => s.Students.Select(s => s.Id))), data.AsQueryable().Where(stringRequest.GetRulesExpression<Student>()).ToList().Select(s => s.Grades.Select(s => s.Students.Select(s => s.Id))));
        Assert.Equal(data.AsQueryable().Where(s => s.Grades.Any(g => g.Students.Any(s => s.Name.Equals("student中的grade中的student6")))).ToList().Select(s => s.Grades.Select(s => s.Students.Select(s => s.Id))), data.AsQueryable().Where(nameOfRequest.GetRulesExpression<Student>()).ToList().Select(s => s.Grades.Select(s => s.Students.Select(s => s.Id))));
    }
}
