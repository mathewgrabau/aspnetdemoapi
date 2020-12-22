using System;
using System.Linq.Expressions;

namespace DemoApi.Infrastructure
{
    public class DecimalToIntSearchExpressionProvider : DefaultSearchExpressionProvider {
        public override ConstantExpression GetValue(string input)
        {
            if (!decimal.TryParse(input, out decimal decimalValue))
            {
                throw new ArgumentException("Invalid search value");
            }

            var places = BitConverter.GetBytes(decimal.GetBits(decimalValue)[3])[2];
            if (places < 2)
            {
                places = 2;
            }

            // Get digits from the decimal
            var digitsOnly = (int)(decimalValue * (decimal)Math.Pow(10, places));

            return Expression.Constant(digitsOnly); 
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
        }
    }
}
