using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DemoApi.Infrastructure
{
    /// <summary>
    /// Produces strings by default. Default is equality comparison
    /// </summary>
    public class DefaultSearchExpressionProvider : ISearchExpressionProvider
    {
        protected const string EqualsOperator = "eq";

        public virtual IEnumerable<string> GetOperators()
        {
            yield return EqualsOperator;
        }

        public virtual Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            if (!op.Equals(EqualsOperator, System.StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Invalid operator '{op}'");
            }

            return Expression.Equal(left, right); 
        }

        public virtual ConstantExpression GetValue(string input)
        {
            return Expression.Constant(input);
        }
    }

}
