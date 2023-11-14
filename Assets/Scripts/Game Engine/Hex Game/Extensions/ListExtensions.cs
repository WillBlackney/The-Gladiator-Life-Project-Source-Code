using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace WeAreGladiators
{
    static class ListExtensions
    {

        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

        /// <summary>
        ///     Randomizes the order of all elements currently in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static List<T> Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do
                {
                    provider.GetBytes(box);
                }
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = box[0] % n;
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return (List<T>)list;
        }

        /// <summary>
        ///     Creates a copy of the list, randomizes the order of the elements, then returns the copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> ShuffledCopy<T>(this IList<T> list)
        {
            List<T> l = new List<T>();
            l.AddRange(list);
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = l.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do
                {
                    provider.GetBytes(box);
                }
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = box[0] % n;
                n--;
                T value = l[k];
                l[k] = l[n];
                l[n] = value;
            }
            return l;
        }

        /// <summary>
        ///     Returns a random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }
            if (list.Count == 1)
            {
                return list[0];
            }
            return list[NumberBetween(0, list.Count - 1)];
        }

        /// <summary>
        ///     Removes a random element from the list, then returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T PopRandomElement<T>(this IList<T> list)
        {
            if (list.Count == 1)
            {
                T element = list[0];
                list.Remove(element);
                return element;
            }
            else
            {
                T element = list[NumberBetween(0, list.Count - 1)];
                list.Remove(element);
                return element;
            }
        }

        /// <summary>
        ///     Returns a random element from the list X times, where X is the argument 'elementsCount'.
        ///     This function can return the same element multiple times
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="elementsCount"></param>
        /// <returns></returns>
        public static List<T> GetRandomElements<T>(this List<T> list, int elementsCount)
        {
            return list.OrderBy(arg => Guid.NewGuid()).Take(list.Count < elementsCount ? list.Count : elementsCount)
                .ToList();
        }

        /// <summary>
        ///     Returns a random element from the list X times, where X is the argument 'elementsCount'.
        ///     This function will NOT return the same element twice
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="elementsCount"></param>
        /// <returns></returns>
        public static List<T> GetRandomDifferentElements<T>(this List<T> list, int elementsCount)
        {
            List<T> listRet = new List<T>();
            List<T> shuffledList = list.ShuffledCopy();
            for (int i = 0; i < elementsCount && i < list.Count; i++)
            {
                listRet.Add(shuffledList[i]);
            }
            return listRet;
        }

        /// <summary>
        ///     Returns the last element in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetLast<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        /// <summary>
        ///     Returns a new list, with all elements copied over.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this IList<T> list)
        {
            List<T> l = new List<T>();
            l.AddRange(list);
            return l;
        }
        private static int NumberBetween(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];

            _generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            // We are using Math.Max, and substracting 0.00000000001,
            // to ensure "multiplier" will always be between 0.0 and .99999999999
            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            double multiplier = Math.Max(0, asciiValueOfRandomCharacter / 255d - 0.00000000001d);

            // We need to add one to the range, to allow for the rounding done with Math.Floor
            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int) (minimumValue + randomValueInRange);
        }
    }
}
