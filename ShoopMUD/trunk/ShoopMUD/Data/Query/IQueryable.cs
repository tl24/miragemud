using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Query
{
    /// <summary>
    /// IQueryable is an interface that designates an object is accessible
    /// through a URI naming scheme.  It provides methods for accessing the URI
    /// of an object as well as searching its children that also implement the interface.
    /// </summary>
    public interface IQueryable
    {
        /// <summary>
        /// Returns the URI for this object.
        /// </summary>
        string URI { get; }

        /// <summary>
        /// Finds the first object matching the given query
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <returns>the first matching object</returns>
        IQueryable Find(ObjectQuery query);

        /// <summary>
        /// Finds all objects matching the given query
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <returns>array of matching objects</returns>
        IList<IQueryable> FindAll(ObjectQuery query);

        /// <summary>
        /// For a query that returns multiple objects, the nth object in the list.
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <param name="index">the index of the item in the result set to return</param>
        /// <returns>the nth item in the result of the query</returns>
        IQueryable Find(ObjectQuery query, int index);

        /// <summary>
        /// Finds the first object matching the given query
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <returns>the first matching object</returns>
        IQueryable Find(string query);

        /// <summary>
        /// Finds all objects matching the given query
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <returns>array of matching objects</returns>
        IList<IQueryable> FindAll(string query);

        /// <summary>
        /// For a query that returns multiple objects, the nth object in the list.
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <param name="index">the index of the item in the result set to return</param>
        /// <returns>the nth item in the result of the query</returns>
        IQueryable Find(string query, int index);
    }

}
