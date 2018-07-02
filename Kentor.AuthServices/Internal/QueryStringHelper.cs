using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    /// <summary>
    /// Class implements static methods to help parse a query string.
    /// </summary>
    internal static class QueryStringHelper
    {
        public static Uri AddQueryParamToRelativeUri(Uri relativeUri, string paramName, string paramValue)
        {
            if (relativeUri.IsAbsoluteUri)
            {
                throw new InvalidOperationException();
            }

            var parts = relativeUri.ToString().Split('?');
            var relativeUrl = parts.First();
            var queryParams = new Dictionary<string, string>();

            if (parts.Length > 1)
            {
                var existingParams = ParseQueryString(parts.Last());
                foreach (var param in existingParams)
                {
                    queryParams.Add(param.Key, existingParams[param.Key].SingleOrDefault());
                }
            }

            queryParams[Uri.EscapeUriString(paramName)] = paramValue; // TODO: Under what circumstance is it necessary to URI encode param value??

            var url = queryParams.Count() == 0 ? relativeUrl :
                relativeUrl + "?" + string.Join("&", queryParams.Select(q => q.Key + "=" + q.Value).ToList());
            return new Uri(url, UriKind.Relative);
        }

        /// <summary>
        /// Splits a query string into its key/value pairs.
        /// </summary>
        /// <param name="queryString">A query string, with or without the leading '?' character.</param>
        /// <returns>A collecktion with the parsed keys and values.</returns>
        public static ILookup<String, String> ParseQueryString(String queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            if (queryString.Length != 0 && queryString[0] == '?')
            {
                queryString = queryString.Substring(1);
            }

            return queryString.Split('&')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Split('='))
                .ToLookup(y => y[0], y => y.Length > 1 ? Uri.UnescapeDataString(y[1]) : null);
        }
    }
}
