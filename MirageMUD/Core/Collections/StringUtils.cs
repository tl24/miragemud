
namespace Mirage.Core.Collections
{
    public static class StringUtils
    {
        /// <summary>
        /// Uppercases the first character of the string only
        /// </summary>
        /// <param name="str">the string to modify</param>
        /// <returns>the modified string</returns>
        public static string ToUpperFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            else if (char.IsUpper(str[0]))
                return str;
            else if (str.Length == 1)
                return str.ToUpper();
            else
                return str.Substring(0, 1).ToUpper() + str.Substring(1);
            
        }
    }
}
