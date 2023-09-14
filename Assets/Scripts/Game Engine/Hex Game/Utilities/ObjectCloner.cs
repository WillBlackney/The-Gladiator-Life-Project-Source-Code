using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace WeAreGladiators.Utilities
{
    public static class ObjectCloner
    {
        /// <summary>
        ///     Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T) formatter.Deserialize(stream);
            }
        }

        public static T CloneJSON<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                Debug.LogWarning("ObjectCloner.CloneJSON() was givien a null object to clone, returning the original...");
                return default(T);
            }

            Type typeParameterType = typeof(T);

            string jsonString = JsonUtility.ToJson(source);
            return (T) JsonUtility.FromJson(jsonString, typeParameterType);
        }
    }
}
