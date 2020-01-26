using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Epok.Core.Utilities
{
    public static class Extension
    {
        /// <summary>
        /// Creates deep copy of an object through binary serialization.
        /// </summary>
        public static T DeepCopy<T>(this T obj) where T : class
        {
            if (obj == null)
                return null;

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                stream.Seek(0, SeekOrigin.Begin);
                return (T) formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Puts current object into a collection with a single element.
        /// </summary>
        public static ICollection<T> Collect<T>(this T obj)
        {
            return new[] {obj};
        }

        public static HashSet<T> CollectToHashSet<T>(this T obj)
        {
            return new HashSet<T> {obj};
        }

        /// <summary>
        /// Converts collection into a string
        /// by invoking ToString() on each item in the collection
        /// and concatenating them into a single string through
        /// a string builder.
        /// </summary>
        public static string ToText<T>(this IEnumerable<T> collection)
        {
            var sb = new StringBuilder();
            foreach (var item in collection)
                sb.Append(item);
            return sb.ToString();
        }
    }
}
