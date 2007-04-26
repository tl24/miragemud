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
        IQueryable find(ObjectQuery query);

        /// <summary>
        /// Finds all objects matching the given query
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <returns>array of matching objects</returns>
        IList<IQueryable> findAll(ObjectQuery query);

        /// <summary>
        /// For a query that returns multiple objects, the nth object in the list.
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <param name="index">the index of the item in the result set to return</param>
        /// <returns>the nth item in the result of the query</returns>
        IQueryable find(ObjectQuery query, int index);

        /// <summary>
        /// Finds the first object matching the given query
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <returns>the first matching object</returns>
        IQueryable find(string query);

        /// <summary>
        /// Finds all objects matching the given query
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <returns>array of matching objects</returns>
        IList<IQueryable> findAll(string query);

        /// <summary>
        /// For a query that returns multiple objects, the nth object in the list.
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <param name="index">the index of the item in the result set to return</param>
        /// <returns>the nth item in the result of the query</returns>
        IQueryable find(string query, int index);
    }

}
