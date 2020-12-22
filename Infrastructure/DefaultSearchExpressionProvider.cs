using System.Linq.Expressions;

namespace DemoApi.Infrastructure
{
    /// <summary>
    /// Produces strings by default.
    /// </summary>
    public class DefaultSearchExpressionProvider : ISearchExpressionProvider
    {
        public virtual ConstantExpression GetValue(string input)
        {
            return Expression.Constant(input);
        }
    }
}
