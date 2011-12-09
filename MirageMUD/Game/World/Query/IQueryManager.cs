using System.Collections;
namespace Mirage.Game.World.Query
{

    /// <summary>
    /// An object that is capable of searching objects using criteria
    /// </summary>
    public interface IQueryManager
    {
        /// <summary>
        /// Searches from the root for the nth object matching the given query specified by index
        /// </summary>
        /// <param name="criteria">query criteria</param>
        /// <param name="index">the index in the search results of the object to return</param>
        /// <returns>the nth matching object</returns>
        object Find(ObjectQuery criteria, int index);

        /// <summary>
        /// Searches an object for the nth object matching the given query specified by index
        /// </summary>
        /// <param name="searched">the object to search</param>
        /// <param name="criteria">query criteria</param>
        /// <param name="index">the index in the search results of the object to return</param>
        /// <returns>the nth matching object</returns>
        object Find(object searched, ObjectQuery criteria, int index);

        /// <summary>
        /// Searches from the root for the nth object matching the given query specified by index
        /// </summary>
        /// <param name="criteria">query criteria</param>
        /// <param name="index">the index in the search results of the object to return</param>
        /// <returns>the nth matching object</returns>
        object Find(string criteria, int index);

        /// <summary>
        /// Searches an object for the nth object matching the given query specified by index
        /// </summary>
        /// <param name="searched">the object to search</param>
        /// <param name="criteria">query criteria</param>
        /// <param name="index">the index in the search results of the object to return</param>
        /// <returns>the nth matching object</returns>
        object Find(object searched, string criteria, int index);

        /// <summary>
        /// Finds the first object matching the given query
        /// </summary>
        /// <param name="searched">the object to search</param>
        /// <param name="criteria">query criteria</param>
        /// <returns>the first matching object</returns>
        object Find(object searched, string criteria);

        /// <summary>
        /// Finds the first object matching the given query
        /// </summary>
        /// <param name="searched">the object to search</param>
        /// <param name="criteria">query criteria</param>
        /// <returns>the first matching object</returns>
        object Find(object searched, ObjectQuery criteria);
        
        /// <summary>
        /// Searches from the root for the first object matching the given query
        /// </summary>
        /// <param name="criteria">query criteria</param>
        /// <returns>the first matching object</returns>
        object Find(string criteria);

        /// <summary>
        /// Searches from the root for the first object matching the given query
        /// </summary>
        /// <param name="criteria">query criteria</param>
        /// <returns>the first matching object</returns>
        object Find(ObjectQuery criteria);

        /// <summary>
        /// Finds all matches for a given query.
        /// </summary>
        /// <param name="searched">object to be searched</param>
        /// <param name="criteria">the query criteria</param>
        /// <param name="start">starting index to match</param>
        /// <param name="count">number of matches to return or 0 for all matches</param>
        /// <returns>enumerable search results</returns>
        IEnumerable FindAll(object searched, ObjectQuery criteria, int start, int count);
    }
}
