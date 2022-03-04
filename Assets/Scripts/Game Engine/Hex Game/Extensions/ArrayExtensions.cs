using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace HexGameEngine
{
    static class ArrayExtensions
    {

        /// <summary>
        /// Randomizes the order of all elements currently in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void Shuffle<T>(this T[] array)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = array.Length;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }


        /// <summary>
        /// Creates a copy of the array, randomizes the order of the elements, then returns the copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] ShuffledCopy<T>(this T[] array)
        {
            T[] newArray = new T[array.Length];
            Array.Copy(array, newArray, array.Length);
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = newArray.Length;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = newArray[k];
                newArray[k] = newArray[n];
                newArray[n] = value;
            }
            return newArray;
        }


        /// <summary>
        /// Returns a random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this T[] array)
        {
            if (array.Length == 1) return array[0];
            else return array[NumberBetween(0, array.Length - 1)];
        }


        /// <summary>
        /// Removes a random element from the array, then returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T PopRandomElement<T>(this T[] array)
        {
            int indexToRemove = NumberBetween(0, array.Length - 1);
            T ret = array[indexToRemove];
            array = array.Where((source, index) => index != indexToRemove).ToArray();
            return ret;
        }

        /// <summary>
        /// Returns a random element from the array X times, where X is the argument 'elementsCount'.
        /// This function will NOT return the same element twice
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="elementsCount"></param>
        /// <returns></returns>
        public static T[] GetRandomDifferentElements<T>(this T[] array, int elementsCount)
        {
            T[] arrRet = new T[elementsCount];
            List<int> indexes = new List<int>();
            for(int i = 0; i < array.Length; i++)            
                indexes.Add(i);
            indexes.Shuffle();
            for(int i = 0; i < elementsCount; i++)            
                arrRet[i] = array[indexes[i]];
            return arrRet;            
        }


        /// <summary>
        /// Returns the last element in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetLast<T>(this T[] array)
        {
            return array[array.Length - 1];
        }
               

        /// <summary>
        /// Returns a new array, with all elements copied over.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] CloneThis<T>(this T[] array)
        {
            T[] newArray = new T[array.Length];
            Array.Copy(array, newArray, array.Length);
            return newArray;
        }       




        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();
        private static int NumberBetween(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];

            _generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            // We are using Math.Max, and substracting 0.00000000001,
            // to ensure "multiplier" will always be between 0.0 and .99999999999
            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

            // We need to add one to the range, to allow for the rounding done with Math.Floor
            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }

    }
}


