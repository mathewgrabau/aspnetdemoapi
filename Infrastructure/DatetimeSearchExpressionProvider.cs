using System;
using System.Linq.Expressions;

namespace DemoApi.Infrastructure
{
    public class DateTimeSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!DateTime.TryParse(input, out DateTime parsedDate))
            {
                throw new ArgumentException("Invalid search value");
            }

            return Expression.Constant(input);
        }

        const string OP_LT = "lt";
        const string OP_LE = "lte";
        const string OP_EQ = "eq";
        const string OP_GT = "gt";
        const string OP_GE = "gte";

        public override Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            Expression expression = null;

            switch (op.ToLower())
            {
                case OP_LT:
                    expression = Expression.LessThan(left, right);
                    break;
                case OP_LE:
                    expression = Expression.LessThanOrEqual(left, right);
                    break;
                case OP_EQ:
                    expression = Expression.Equal(left, right);
                    break;
                case OP_GT:
                    expression = Expression.GreaterThan(left, right);
                    break;
                case OP_GE:
                    expression = Expression.GreaterThanOrEqual(left, right);
                    break;
                default:
                    expression = base.GetComparison(left, op, right);
                    break;
            }

            return expression;

            return base.GetComparison(left, op, right);
        }
    }
}
