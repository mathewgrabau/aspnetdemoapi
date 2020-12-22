using System.Linq.Expressions;

namespace DemoApi.Infrastructure
{
    /// <summary>
    /// Produces strings by default. Default is equality comparison
    /// </summary>
    public class DefaultSearchExpressionProvider : ISearchExpressionProvider
    {
        public virtual Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            return Expression.Equal(left, right); 
        }

        public virtual ConstantExpression GetValue(string input)
        {
            return Expression.Constant(input);
        }
    }
}
