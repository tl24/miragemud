using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mirage.Game.World.Containers;
using System.Collections;

namespace Mirage.Game.World.Query
{
    public static class QueryExtensions
    {
        public static Regex _parser = new Regex(@"^(?<index>\d+\.)");

        /// <summary>
        /// Shortcut for finding the first matching item
        /// </summary>
        /// <typeparam name="T">the type of the items in the collection</typeparam>
        /// <param name="container">the container</param>
        /// <param name="query">the query</param>
        /// <param name="matchType">The type of match: partial or exact</param>
        /// <returns>first matching item or null</returns>
        public static T FindOne<T>(this IEnumerable<T> container, string query, QueryMatchType matchType, Func<T, string, QueryMatchType, bool> matcher)
        {
            return container.Find(query, matchType, matcher).FirstOrDefault();
        }

        /// <summary>
        /// Shortcut for finding the first matching item
        /// </summary>
        /// <typeparam name="T">the type of the items in the collection</typeparam>
        /// <param name="container">the container</param>
        /// <param name="query">the query</param>
        /// <param name="matchType">The type of match: partial or exact</param>
        /// <returns>first matching item or null</returns>
        public static T FindOne<T>(this IEnumerable<T> container, string query, QueryMatchType matchType = QueryMatchType.Partial) where T : ISupportUri
        {
            return container.Find(query, matchType).FirstOrDefault();
        }

        /// <summary>
        /// Shortcut for finding the first matching item
        /// </summary>
        /// <typeparam name="T">the type of the items in the collection</typeparam>
        /// <param name="container">the container</param>
        /// <param name="query">the query</param>
        /// <param name="matchType">The type of match: partial or exact</param>
        /// <returns>first matching item or null</returns>
        public static object FindOne(this IEnumerable container, string query, QueryMatchType matchType = QueryMatchType.Partial)
        {
            return container.OfType<ISupportUri>().FindOne(query, matchType);
        }

        /// <summary>
        /// Finds the items in a collection that match on name
        /// </summary>
        /// <typeparam name="T">the type of the items in the collection</typeparam>
        /// <param name="container">the container</param>
        /// <param name="query">the query</param>
        /// <returns>matching items</returns>
        public static IEnumerable Find(this IEnumerable container, string query, QueryMatchType matchType = QueryMatchType.Partial)
        {
            return container.OfType<ISupportUri>().Find(query, matchType);
        }

        /// <summary>
        /// Finds the items in a collection that match on name
        /// </summary>
        /// <typeparam name="T">the type of the items in the collection</typeparam>
        /// <param name="container">the container</param>
        /// <param name="query">the query</param>
        /// <returns>matching items</returns>
        public static IEnumerable<T> Find<T>(this IEnumerable<T> container, string query, QueryMatchType matchType = QueryMatchType.Partial) where T : ISupportUri
        {
            return container.Find(query, matchType, Match);
        }

        /// <summary>
        /// Finds the items in a collection that match on name
        /// </summary>
        /// <typeparam name="T">the type of the items in the collection</typeparam>
        /// <param name="container">the container</param>
        /// <param name="query">the query</param>
        /// <returns>matching items</returns>
        public static IEnumerable<T> Find<T>(this IEnumerable<T> container, string query, QueryMatchType matchType, Func<T, string, QueryMatchType, bool> matcher)
        {
            query = query.Trim();
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<T>();

            Match indexMatch = _parser.Match(query);
            if (indexMatch.Success)
            {
                var indexStr = indexMatch.Groups["index"].Value;
                int index = int.Parse(indexStr.TrimEnd('.'));
                if (index <= 0)
                    index = 1; // make it 1-based
                query = query.Substring(indexStr.Length);
                return container.Where(i => matcher(i, query, matchType)).Skip(index - 1).Take(1);
            }
            else
            {
                // no indexer so just find the match
                return container.Where(i => matcher(i, query, matchType));
            }
        }

        private static bool Match<T>(T uriObject, string query, QueryMatchType matchType) where T : ISupportUri
        {
            switch (matchType)
            {
                case QueryMatchType.Default:
                case QueryMatchType.Partial:
                    return uriObject.Uri.ToLower().Contains(query.ToLower());
                case QueryMatchType.Exact:
                    return uriObject.Uri.Equals(query, StringComparison.CurrentCultureIgnoreCase);
                case QueryMatchType.All:
                    return true;
                default:
                    throw new IndexOutOfRangeException("Unrecognized match type: " + matchType);
            }
        }
    }
}
