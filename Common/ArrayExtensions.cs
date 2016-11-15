using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emlid.WindowsIot.Common
{
    /// <summary>
    /// Provides helper methods and extensions for working with arrays and collections.
    /// </summary>
    public static class ArrayExtensions
    {
        #region Public Methods

        /// <summary>
        /// Compares two list based arrays by value.
        /// </summary>
        public static bool AreEqual(IList array1, IList array2)
        {
            // Compare null
            if (array1 == null)
                return array2 == null;
            if (array2 == null)
                return false;

            // Compare length
            if (array1.Count != array2.Count)
                return false;

            // Compare values
            for (var index = 0; index < array1.Count; index++)
            {
                var value1 = array1[index];
                var value2 = array2[index];
                if (!ReferenceEquals(value1, null))
                {
                    // Compare nested array by value too
                    if (value1.GetType().IsArray)
                        return AreEqual((Array)value1, (Array)value2);

                    // Compare other objects using any defined comparer or operator overloads
                    // This will still compare reference types by reference when none are defined
                    if (!value1.Equals(value2))
                        return false;
                }
                else if (!ReferenceEquals(value2, null))
                    return false;
            }

            // Return same
            return true;
        }

        /// <summary>
        /// Compares two collections by value.
        /// </summary>
        public static bool AreEqual(IEnumerable enumeration1, IEnumerable enumeration2)
        {
            // Compare null
            if (enumeration1 == null)
                return enumeration2 == null;
            if (enumeration2 == null)
                return false;

            // Compare values
            var enumerator1 = enumeration1.GetEnumerator();
            var enumerator2 = enumeration2.GetEnumerator();
            do
            {
                // Get next item and check length
                var more1 = enumerator1.MoveNext();
                var more2 = enumerator2.MoveNext();
                if (more1 != more2)
                {
                    // Different lengths
                    return false;
                }
                if (!more1)
                {
                    // End with no differences
                    return true;
                }

                // Compare current values
                var value1 = enumerator1.Current;
                var value2 = enumerator2.Current;
                if (!ReferenceEquals(value1, null))
                {
                    // Compare nested array by value too
                    if (value1.GetType().IsArray)
                        return AreEqual((Array)value1, (Array)value2);

                    // Compare other objects using any defined comparer or operator overloads
                    // This will still compare reference types by reference when none are defined
                    if (!value1.Equals(value2))
                        return false;
                }
                else if (!ReferenceEquals(value2, null))
                    return false;

                // Next...
            } while (true);
        }

        /// <summary>
        /// Compares part of two arrays for equality.
        /// </summary>
        public static bool AreEqual(byte[] array1, int offset1, byte[] array2, int offset2, int length)
        {
            // Check length does not exceed boundaries
            if (offset1 + length > array1.Length || offset2 + length > array2.Length)
                return false;

            // Compare array contents
            for (var i = 0; i < length; i++)
            {
                if (array1[offset1 + i] != array2[offset2 + i])
                    return false;
            }
            return true;
        }

       
        /// <summary>
        /// Searches any array for a value, i.e. without having to create a list or collection.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="array">Array to search.</param>
        /// <param name="value">Value to find.</param>
        /// <returns>True when present.</returns>
        public static bool Contains<T>(this IEnumerable<T> array, T value)
        {
            return array.Any(item => item.Equals(value));
        }

        /// <summary>
        /// Checks if the string array contains the specified value optionally ignoring case.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="value">Value to search for.</param>
        /// <param name="comparisonType">Comparison options, e.g. set to <see cref="StringComparison.OrdinalIgnoreCase"/> for a case insensitive comparison.</param>
        /// <returns>True when found.</returns>
        public static bool Contains(this IEnumerable<string> array, string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            return array.FirstOrDefault(item => String.Compare(item, value, comparisonType) == 0) != null;
        }

        /// <summary>
        /// Gets the hash code of all items in the array.
        /// </summary>
        public static int GetHashCodeOfItems(this IList array)
        {
            return array.Cast<object>().Aggregate(0, (current, item) => current ^ (!ReferenceEquals(item, null) ? item.GetHashCode() : 0));
        }

        /// <summary>
        /// Gets the hash code of all items in the array, or zero when null.
        /// </summary>
        public static int GetHashCodeOfItemsIfExists(IList array)
        {
            if (ReferenceEquals(array, null))
                return 0;
            return array.GetHashCodeOfItems();
        }

        /// <summary>
        /// Disposes all members implementing <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="list">List of items to dispose.</param>
        public static void Dispose(this IList list)
        {
            foreach (var disposable in list.Cast<IDisposable>().ToArray())
            {
                list.Remove(disposable);
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Transforms a <see cref="byte"/> into an hex dump formatted string
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="bytesPerLine"></param>
        /// <returns></returns>
        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }

        #endregion
    }
}
