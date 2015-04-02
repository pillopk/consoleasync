using System;
using System.Collections.Generic;

namespace AsyncronousConsole.Support
{
    public static class Extensions
    {
        // ------------------------------------------------------------
        // STRING EXTENSIONS
        // ------------------------------------------------------------

        /// <summary>Return the first characters of a string specified by length parameter </summary>
        /// <param name="source">Source string</param>
        /// <param name="length">Number of characters to retrieve</param>
        /// <returns>Left subset of original string</returns>
        public static string Left(this string source, int length)
        {
            if (source.Length < length) return source;
            return source.Substring(0, length);
        }

        /// <summary>Return the last characters of a string without the first characters specified by length parameter </summary>
        /// <param name="source">Source string</param>
        /// <param name="length">Number of characters to ignore</param>
        /// <returns>Right subset of original string</returns>
        public static string LeftRest(this string source, int length)
        {
            if (source.Length < length) return string.Empty;
            return source.Substring(length, source.Length - length);
        }

        /// <summary>Return the last characters of a string specified by length parameter </summary>
        /// <param name="source">Source string</param>
        /// <param name="length">Number of characters to retrieve</param>
        /// <returns>Right subset of original string</returns>
        public static string Right(this string source, int length)
        {
            if (source.Length < length) return source;
            return source.Substring(source.Length - length, length);
        }

        /// <summary>Return the first characters of a string without the last characters specified by length parameter </summary>
        /// <param name="source">Source string</param>
        /// <param name="length">Number of characters to ignore</param>
        /// <returns>Left subset of original string</returns>
        public static string RightRest(this string source, int length)
        {
            if (source.Length < length) return string.Empty;
            return source.Substring(0, source.Length - length);
        }

        /// <summary>Return subset of string specified by index parameters </summary>
        /// <param name="source">Source string</param>
        /// <param name="startIndex">Index of first character to take</param>
        /// <param name="endIndex">Index of last character to take</param>
        /// <returns>Subset of original string</returns>
        public static string Chunk(this string source, int startIndex, int endIndex)
        {
            if (endIndex < startIndex)
                throw new ArgumentException("End index must be greater then start index");

            return source.Substring(startIndex, endIndex - startIndex + 1);
        }

        /// <summary>Return a string with length specified by totalLength parameter, eventually filled or truncated </summary>
        /// <param name="source">Source string</param>
        /// <param name="totalLength">Requested length of string</param>
        /// <param name="filler">Character used to fill string if is too short</param>
        /// <returns>String of fixed length</returns>
        public static string Fit(this string source, int totalLength, char filler = ' ')
        {
            if (totalLength >= source.Length)
                return source.PadRight(totalLength, filler);

            return (totalLength < source.Length) ? source.Substring(0, totalLength) : source;
        }

        /// <summary>Truncate string at the specified length and insert ellipsis character if necessary </summary>
        /// <param name="source">Source string</param>
        /// <param name="totalLength">Max string length</param>
        /// <param name="ellipsis">Ellipsis character</param>
        /// <returns>String with max length</returns>
        public static string Truncate(this string source, int totalLength, string ellipsis = "_")
        {
            if (totalLength >= source.Length) return source;

            return string.Concat(source.Left(totalLength - ellipsis.Length), ellipsis);
        }

        /// <summary>Return string without a specified character </summary>
        /// <param name="source">Source string</param>
        /// <param name="charIndex">Index of character to remove</param>
        /// <returns>String</returns>
        public static string RemoveChar(this string source, int charIndex)
        {
            if ((charIndex < 0) || (charIndex > source.Length - 1))
                throw new IndexOutOfRangeException("Char index must be into bounds of string");

            if (charIndex == 0)
            {
                return source.LeftRest(1);
            }

            if (charIndex == source.Length - 1)
            {
                return source.RightRest(1);
            }

            string left = source.Left(charIndex);
            string right = source.Right(source.Length - charIndex - 1);
            return string.Concat(left, right);
        }

        /// <summary>Cut source string in multiple lines with a length specified by lineLength parameter </summary>
        /// <param name="source">Source string</param>
        /// <param name="lineLength">Length of line</param>
        /// <returns>Array of strings</returns>
        public static string[] FitMultiline(this string source, int lineLength)
        {
            string temp = source;
            List<string> results = new List<string>();

            while (true)
            {
                if (temp.Length > lineLength)
                {
                    results.Add(temp.Left(lineLength));
                    temp = temp.LeftRest(lineLength);
                }

                if (temp.Length <= lineLength)
                {
                    results.Add(temp);
                    break;
                }
            }

            return results.ToArray();
        }

        // ------------------------------------------------------------
        // LONG EXTENSIONS
        // ------------------------------------------------------------

        /// <summary>Return a string with file formatted size</summary>
        /// <param name="source">Dimension in byte</param>
        /// <returns>String</returns>
        public static string ToFileSize(this long source)
        {
            const long kiloByte = 1024;
            const long megaByte = kiloByte * 1024;
            const long gigaByte = megaByte * 1024;

            string suffix;
            if (source > gigaByte)
            {
                source /= gigaByte;
                suffix = "GB";
            }
            else if (source > megaByte)
            {
                source /= megaByte;
                suffix = "MB";
            }
            else if (source > kiloByte)
            {
                source /= kiloByte;
                suffix = "kB";
            }
            else
            {
                suffix = "Bt";
            }

            return string.Format("{0} {1}", source, suffix);
        }

    }
}
