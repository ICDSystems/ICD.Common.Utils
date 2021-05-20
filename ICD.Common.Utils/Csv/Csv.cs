/*
 * 2006 - 2018 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/csharp-csv-reader
 */

using System;
using System.Collections.Generic;
using System.Text;
using ICD.Common.Utils.IO;
#if SIMPLSHARP
using ICD.Common.Utils.Extensions;
#endif

namespace ICD.Common.Utils.Csv
{

    /// <summary>
    /// Root class that contains static functions for straightforward Csv parsing
    /// </summary>
    public static class Csv
    {
        /// <summary>
        /// The default Csv field delimiter.
        /// </summary>
        public const char DEFAULT_CSV_DELIMITER = ',';

        /// <summary>
        /// The default Csv text qualifier.  This is used to encode strings that contain the field delimiter.
        /// </summary>
        public const char DEFAULT_CSV_QUALIFIER = '"';

        /// <summary>
        /// The default TSV (tab delimited file) field delimiter.
        /// </summary>
        public const char DEFAULT_TSV_DELIMITER = '\t';

        /// <summary>
        /// The default TSV (tabe delimited file) text qualifier.  This is used to encode strings that contain the field delimiter.
        /// </summary>
        public const char DEFAULT_TSV_QUALIFIER = '"';


#region Methods to read Csv data
        /// <summary>
        /// Parse a Csv stream into IEnumerable&lt;string[]&gt;, while permitting embedded newlines
        /// </summary>
        /// <param name="inStream">The stream to read</param>
        /// <param name="settings">The Csv settings to use for this parsing operation (Default: Csv)</param>
        /// <returns>An enumerable object that can be examined to retrieve rows from the stream.</returns>
        public static IEnumerable<string[]> ParseStream(IcdStreamReader inStream, CsvReaderSettings settings)
        {
            string line = "";
            int i = -1;
            List<string> list = new List<string>();
            var work = new StringBuilder();

            // Ensure settings are non-null
            if (settings == null) {
                settings = CsvReaderSettings.CSV;
            }

            // Begin reading from the stream
            while (i < line.Length || !inStream.EndOfStream)
            {
                // Consume the next character of data
                i++;
                if (i >= line.Length) {
                    var newLine = inStream.ReadLine();
                    line += newLine + settings.LineSeparator;
                }
                char c = line[i];

                // Are we at a line separator? If so, yield our work and begin again
                if (String.Equals(line.Substring(i, settings.LineSeparator.Length), settings.LineSeparator)) {
                    list.Add(work.ToString());
                    yield return list.ToArray();
                    list.Clear();
                    work.Clear();
                    if (inStream.EndOfStream)
                    {
                        break;
                    }

                    // Read in next line
                    if (i + settings.LineSeparator.Length >= line.Length)
                    {
                        line = inStream.ReadLine() + settings.LineSeparator;
                    }
                    else
                    {
                        line = line.Substring(i + settings.LineSeparator.Length);
                    }
                    i = -1;

                    // While starting a field, do we detect a text qualifier?
                }
                else if ((c == settings.TextQualifier) && (work.Length == 0))
                {
                    // Our next task is to find the end of this qualified-text field
                    int p2 = -1;
                    while (p2 < 0) {

                        // If we don't see an end in sight, read more from the stream
                        p2 = line.IndexOf(settings.TextQualifier, i + 1);
                        if (p2 < 0) {

                            // No text qualifiers yet? Let's read more from the stream and continue
                            work.Append(line.Substring(i + 1));
                            i = -1;
                            var newLine = inStream.ReadLine();
                            if (String.IsNullOrEmpty(newLine) && inStream.EndOfStream)
                            {
                                break;
                            }
                            line = newLine + settings.LineSeparator;
                            continue;
                        }

                        // Append the text between the qualifiers
                        work.Append(line.Substring(i + 1, p2 - i - 1));
                        i = p2;
                        
                        // If the user put in a doubled-up qualifier, e.g. `""`, insert a single one and continue
                        if (((p2 + 1) < line.Length) && (line[p2 + 1] == settings.TextQualifier))
                        {
                            work.Append(settings.TextQualifier);
                            i++;
                            p2 = -1;
                        }
                    }

                    // Does this start a new field?
                }
                else if (c == settings.FieldDelimiter)
                {
                    // Is this a null token, and do we permit null tokens?
                    AddToken(list, work, settings);

                    // Test for special case: when the user has written a casual comma, space, and text qualifier, skip the space
                    // Checks if the second parameter of the if statement will pass through successfully
                    // e.g. `"bob", "mary", "bill"`
                    if (i + 2 <= line.Length - 1)
                    {
                        if (line[i + 1].Equals(' ') && line[i + 2].Equals(settings.TextQualifier))
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    work.Append(c);
                }
            }
        }

        /// <summary>
        /// Parse a single row of data from a Csv line into an array of objects, while permitting embedded newlines
        /// DEPRECATED - Please use ParseStream instead.
        /// </summary>
        /// <param name="inStream">The stream to read</param>
        /// <param name="settings">The Csv settings to use for this parsing operation (Default: Csv)</param>
        /// <returns>An array containing all fields in the next row of data, or null if it could not be parsed.</returns>
        public static string[] ParseMultiLine(IcdStreamReader inStream, CsvReaderSettings settings)
        {
            StringBuilder sb = new StringBuilder();
            string[] array = null;
            while (!inStream.EndOfStream)
            {
                // Read in a line
                sb.Append(inStream.ReadLine());

                // Does it parse?
                string s = sb.ToString();
                if (TryParseLine(s, out array, settings))
                {
                    return array;
                }

                // We didn't succeed on the first try - our text must have an embedded newline in it.
                // Let's assume that we were in the middle of parsing a field when we encountered a newline,
                // and continue parsing.
                sb.Append(settings.LineSeparator);
            }

            // Fails to parse - return the best array we were able to get
            return array;
        }

        /// <summary>
        /// Parse a line from a Csv file and return an array of fields, or null if 
        /// </summary>
        /// <param name="line">One line of text from a Csv file</param>
        /// <param name="settings">The Csv settings to use for this parsing operation (Default: Csv)</param>
        /// <returns>An array containing all fields in the next row of data, or null if it could not be parsed.</returns>
        public static string[] ParseLine(string line, CsvReaderSettings settings)
        {
	        string[] row;
            TryParseLine(line, out row, settings);
            return row;
        }

        /// <summary>
        /// Try to parse a line of Csv data.  Can only return false if an unterminated text qualifier is encountered.
        /// </summary>
        /// <returns>False if there was an unterminated text qualifier in the <paramref name="line"/></returns>
        /// <param name="line">The line of text to parse</param>
        /// <param name="settings">The Csv settings to use for this parsing operation (Default: Csv)</param>
        /// <param name="row">The array of fields found in the line</param>
        public static bool TryParseLine(string line, out string[] row, CsvReaderSettings settings)
        {
            // Ensure settings are non-null
            if (settings == null) settings = CsvReaderSettings.CSV;

            // Okay, let's begin parsing
            List<string> list = new List<string>();
            var work = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                // If we are starting a new field, is this field text qualified?
                if ((c == settings.TextQualifier) && (work.Length == 0))
                {
	                while (true)
                    {
                        int p2 = line.IndexOf(settings.TextQualifier, i + 1);

                        // If no closing qualifier is found, this string is broken; return failure.
                        if (p2 < 0)
                        {
                            work.Append(line.Substring(i + 1));
                            list.Add(work.ToString());
                            row = list.ToArray();
                            return false;
                        }

                        // Append this qualified string
                        work.Append(line.Substring(i + 1, p2 - i - 1));
                        i = p2;

                        // If this is a double quote, keep going!
                        if (((p2 + 1) < line.Length) && (line[p2 + 1] == settings.TextQualifier))
                        {
                            work.Append(settings.TextQualifier);
                            i++;

                            // otherwise, this is a single qualifier, we're done
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Does this start a new field?
                }
                else if (c == settings.FieldDelimiter)
                {
                    // Is this a null token, and do we permit null tokens?
                    AddToken(list, work, settings);

                    // Test for special case: when the user has written a casual comma, space, and text qualifier, skip the space
                    // Checks if the second parameter of the if statement will pass through successfully
                    // e.g. "bob", "mary", "bill"
                    if (i + 2 <= line.Length - 1)
                    {
                        if (line[i + 1].Equals(' ') && line[i + 2].Equals(settings.TextQualifier))
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    work.Append(c);
                }
            }

            // We always add the last work as an element.  That means `alice,bob,charlie,` will be four items long.
            AddToken(list, work, settings);
            row = list.ToArray();
            return true;
        }

        /// <summary>
        /// Add a single token to the list
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="work">Work.</param>
        /// <param name="settings">Settings.</param>
        private static void AddToken(List<string> list, StringBuilder work, CsvReaderSettings settings)
        {
            var s = work.ToString();
            if (settings.AllowNull && String.Equals(s, settings.NullToken, StringComparison.Ordinal))
            {
                list.Add(null);
            }
            else
            {
                list.Add(s);
            }
            work.Length = 0;
        }

#endregion

    }
}
