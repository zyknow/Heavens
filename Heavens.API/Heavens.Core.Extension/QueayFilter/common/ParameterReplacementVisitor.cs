using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Heavens.Core.Extension.QueayFilter.common;

public class ParameterReplacementVisitor : ExpressionVisitor
{
    private readonly ParameterExpression _parameter;

    public ParameterReplacementVisitor(ParameterExpression parameter)
    {
        _parameter = parameter;
    }

    public ParameterExpression Parameter
    {
        get { return _parameter; }
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (node.Name == _parameter.Name)
        {
            return _parameter;
        }

        return base.VisitParameter(node);
    }
}
