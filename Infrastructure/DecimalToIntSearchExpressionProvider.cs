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
    }
}
