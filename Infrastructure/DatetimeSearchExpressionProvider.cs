using System;
using System.Linq.Expressions;

namespace DemoApi.Infrastructure
{
    public class DateTimeSearchExpressionProvider : ComparableSearchExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!DateTime.TryParse(input, out DateTime parsedDate))
            {
                throw new ArgumentException("Invalid search value");
            }

            return Expression.Constant(input);
        }
    }
}
