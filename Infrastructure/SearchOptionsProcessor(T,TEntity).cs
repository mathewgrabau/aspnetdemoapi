using DemoApi.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DemoApi.Models
{
    public class SearchOptionsProcessor<T, TEntity>
    {
        private readonly string[] _searchQuery;

        public SearchOptionsProcessor(string[] searchQuery)
        {
            _searchQuery = searchQuery;    
        }

        public IEnumerable<SearchTerm> GetAllTerms()
        {
            if (_searchQuery == null)
            {
                yield break;
            }

            foreach (var expression in _searchQuery)
            {
                if (string.IsNullOrEmpty(expression))
                {
                    continue;
                }

                // Expressions are something like:
                // "fieldname op value"
                var tokens = expression.Split(' '); // Need the pieces of the expression.
                if (tokens.Length == 0)
                {
                    // Report the error
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name = expression
                    };

                    continue;
                }

                if (tokens.Length < 3)
                {
                    // Report the error
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name = tokens[0]
                    };

                    continue;
                }
                yield return new SearchTerm
                {
                    ValidSyntax = true,
                    Name = tokens[0],
                    Operator = tokens[1],
                    Value = string.Join(" ", tokens.Skip(2))
                };
            }
        }
    
        public IEnumerable<SearchTerm> GetValidTerms()
        {
            // Get terms from the query
            var queryTerms = GetAllTerms().Where(x => x.ValidSyntax).ToArray();

            // No terms, then we're done.
            if (!queryTerms.Any())
            {
                yield break;
            }

            // Getting the declared ones, then using that to remove the ones that don't apply to the class
            var declaredTerms = GetTermsFromModel();

            foreach (var term in queryTerms)
            {
                // Check against the model
                var declaredTerm = declaredTerms.SingleOrDefault(x => x.Name.Equals(term.Name, System.StringComparison.OrdinalIgnoreCase));

                if (declaredTerm == null)
                {
                    continue;
                }

                yield return new SearchTerm
                {
                    ValidSyntax = term.ValidSyntax,
                    Name = declaredTerm.Name,
                    Operator = term.Operator,
                    Value = term.Value,
                    ExpressionProvider = declaredTerm.ExpressionProvider
                };
            }
        }

        // Need the query tracked using IQueryable
        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();
            if (!terms.Any())
            {
                return query;
            }

            var modifiedQuery = query;

            foreach(var term in terms)
            {
                var propertyInfo = ExpressionHelper.GetPropertyInfo<TEntity>(term.Name);
                var obj = ExpressionHelper.Parameter<TEntity>();

                // Build the linq expression backwards
                // query = query.Where(x=>x.Property == "Value");

                // x.Property
                var left = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);
                // Value
                var right = term.ExpressionProvider.GetValue(term.Value);

                // x.Property == Value
                var compareExpression = Expression.Equal(left, right);

                // x=>x.Property == Value
                var lambdaExpression = ExpressionHelper.GetLambda<TEntity, bool>(obj, compareExpression);

                // query = query.Where
                modifiedQuery = ExpressionHelper.CallWhere(modifiedQuery, lambdaExpression);
            }

            return modifiedQuery;
        }
        
        /// <summary>
        /// Returns the list of the valid terms on the model definition,
        /// supporting veritification.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<SearchTerm> GetTermsFromModel()
        {
            return typeof(T).GetTypeInfo().DeclaredProperties.Where(p => p.GetCustomAttributes<SearchableAttribute>().Any())
                .Select(p => new SearchTerm { 
                    Name = p.Name,  
                ExpressionProvider = p.GetCustomAttribute<SearchableAttribute>().ExpressionProvider});
        }
    }
}
