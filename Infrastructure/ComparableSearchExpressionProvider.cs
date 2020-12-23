using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DemoApi.Infrastructure
{
    public abstract class ComparableSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        private const string GreaterThanOperator = "gt";
        private const string LessThanOperator = "lt";
        private const string GreaterThanEqualToOperator = "gte";
        private const string LessThanEqualToOperator = "lte";

        public override IEnumerable<string> GetOperators()
        {
            return base.GetOperators().Concat(new[] { GreaterThanOperator, GreaterThanEqualToOperator, LessThanOperator, LessThanEqualToOperator });
        }

        public override Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            Expression expression = null;

            switch (op.ToLower())
            {
                case LessThanOperator:
                    expression = Expression.LessThan(left, right);
                    break;
                case LessThanEqualToOperator:
                    expression = Expression.LessThanOrEqual(left, right);
                    break;
                case EqualsOperator:
                    expression = Expression.Equal(left, right);
                    break;
                case GreaterThanOperator:
                    expression = Expression.GreaterThan(left, right);
                    break;
                case GreaterThanEqualToOperator:
                    expression = Expression.GreaterThanOrEqual(left, right);
                    break;
                default:
                    expression = base.GetComparison(left, op, right);
                    break;
            }

            return expression;
        }
    }

}
