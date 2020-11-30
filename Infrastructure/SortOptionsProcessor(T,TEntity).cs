﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DemoApi.Infrastructure
{
    public class SortOptionsProcessor<T, TEntity>
    {
        readonly string[] _orderBy;

        public SortOptionsProcessor(string[] orderBy)
        {
            _orderBy = orderBy;
        }

        public IEnumerable<SortTerm> GetAllTerms()
        {
            if (_orderBy == null)
            {
                yield break;
            }

            foreach (var term in _orderBy)
            {
                if (string.IsNullOrEmpty(term))
                {
                    continue;
                }
                // Making the tokens returned on the space
                var tokens = term.Split(' ');

                if (tokens.Length == 0)
                {
                    yield return new SortTerm { Name = term };
                    continue;
                }

                // Otherwise need to extract the tokens
                var descending = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
                yield return new SortTerm { Name = tokens[0], Descending = descending };
            }
        }

        public IEnumerable<SortTerm> GetValidTerms()
        {
            var queryTerms = GetAllTerms().ToArray();
            if (!queryTerms.Any())
            {
                yield break;
            }

            var declaredTerms = GetTermsFromModel();

            foreach (var term in queryTerms)
            {
                var declaredTerm = declaredTerms.SingleOrDefault(x => x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));
                if (declaredTerm == null)
                {
                    continue;
                }

                yield return new SortTerm { Name = declaredTerm.Name, Descending = term.Descending, Default = declaredTerm.Default };
            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();
            if (!terms.Any())
            {
                // No terms pulls the terms from the model
                terms = GetTermsFromModel().Where(t => t.Default).ToArray();
            }
            // Nothing to apply, so return it.
            if (!terms.Any())
            {
                return query;
            }

            var modifiedQuery = query;
            // Need to track which to use OrderBy or OrderThenBy, since it's only allowed once for the former.
            var useThenBy = false;

            foreach (var term in terms)
            {
                // Look up term on that TEntity object.
                var propertyInfo = ExpressionHelper.GetPropertyInfo<TEntity>(term.Name);
                // Generic reference
                var obj = ExpressionHelper.Parameter<TEntity>();

                // Build the LINQ expression backwards:
                // query = query.OrderBy(x=>x.Property);

                // First build the inside term (x=>x.Property);
                var key = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);
                var keySelector = ExpressionHelper.GetLambda(typeof(TEntity), propertyInfo.PropertyType, obj, key);

                // Call either OrderBy/ThenBy[Descending](x=>x.Property);
                modifiedQuery = ExpressionHelper.CallOrderByOrThenBy(modifiedQuery, useThenBy, term.Descending, propertyInfo.PropertyType, keySelector);
                useThenBy = true;   // for every iteration now on.
            }

            return modifiedQuery;
        }

        private static IEnumerable<SortTerm> GetTermsFromModel()
        {
            return typeof(T).GetTypeInfo().DeclaredProperties.Where(p => p.GetCustomAttributes<SortableAttribute>().Any())
                .Select(p => new SortTerm
                {
                    Name = p.Name,
                    Default = p.GetCustomAttribute<SortableAttribute>().Default
                });
        }
    }
}
