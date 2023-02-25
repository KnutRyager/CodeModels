using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Common.Util;

// Original: https://stackoverflow.com/questions/38316519/replace-parameter-type-in-lambda-expression
public class ParameterTypeVisitor : ExpressionVisitor
{
    private ReadOnlyCollection<ParameterExpression>? _parameters;

    private System.Type Source { get; init; }
    private System.Type Target { get; init; }

    public ParameterTypeVisitor(System.Type source, System.Type target)
    {
        Source = source;
        Target = target;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _parameters?.FirstOrDefault(p => p.Name == node.Name) ??
            (node.Type == Source ? Parameter(Target, node.Name) : node);
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        _parameters = VisitAndConvert<ParameterExpression>(node.Parameters, "VisitLambda");
        return Lambda(Convert(Visit(node.Body), Target), _parameters);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        return base.VisitMethodCall(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member.DeclaringType == Source)
        {
            return Property(Visit(node.Expression), node.Member.Name);
        }
        return base.VisitMember(node);
    }
}
