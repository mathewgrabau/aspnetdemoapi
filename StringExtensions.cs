using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApi
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var first = input.Substring(0, 1).ToLower();
            if (input.Length == 1)
            {
                return first;
            }

            // Add the rest of it on
            return first + input.Substring(1);
        }
    }
}
