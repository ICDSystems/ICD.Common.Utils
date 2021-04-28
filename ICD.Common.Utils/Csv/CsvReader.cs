/*
 * 2006 - 2018 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/csharp-csv-reader
 */

using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.IO;

namespace ICD.Common.Utils.Csv
{
    public sealed class CsvReader : IEnumerable<string[]>, IDisposable
    {
        private readonly CsvReaderSettings m_Settings;
        private readonly IcdStreamReader m_Instream;

        #region Public Variables

        /// <summary>
        /// If the first row in the file is a header row, this will be populated
        /// </summary>
        public string[] Headers = null;

        #endregion

        #region Constructors
        /// <summary>
        /// Construct a new Csv reader off a streamed source
        /// </summary>
        /// <param name="source">The stream source</param>
        /// <param name="settings">The Csv settings to use for this reader (Default: Csv)</param>
        public CsvReader(IcdStreamReader source, [CanBeNull] CsvReaderSettings settings)
        {
            m_Instream = source;
            m_Settings = settings ?? CsvReaderSettings.CSV;

            // Do we need to parse headers?
            if (m_Settings.HeaderRowIncluded)
            {
                Headers = NextLine();
            }
            else
            {
                Headers = m_Settings.AssumedHeaders != null ? m_Settings.AssumedHeaders.ToArray() : null;
            }
        }
        #endregion

        #region Iterate through a Csv File
        /// <summary>
        /// Iterate through all lines in this Csv file
        /// </summary>
        /// <returns>An array of all data columns in the line</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Lines().GetEnumerator();
        }

        /// <summary>
        /// Iterate through all lines in this Csv file
        /// </summary>
        /// <returns>An array of all data columns in the line</returns>
        IEnumerator<string[]> System.Collections.Generic.IEnumerable<string[]>.GetEnumerator()
        {
            return Lines().GetEnumerator();
        }

        /// <summary>
        /// Iterate through all lines in this Csv file
        /// </summary>
        /// <returns>An array of all data columns in the line</returns>
        public IEnumerable<string[]> Lines()
        {
            return Csv.ParseStream(m_Instream, m_Settings);
        }

        /// <summary>
        /// Retrieve the next line from the file.
        /// DEPRECATED - 
        /// </summary>
        /// <returns>One line from the file.</returns>
        public string[] NextLine()
        {
            return Csv.ParseMultiLine(m_Instream, m_Settings);
        }
        #endregion

		#region Disposal
        /// <summary>
        /// Close our resources - specifically, the stream reader
        /// </summary>
        public void Dispose()
        {
            m_Instream.Dispose();
        }
		#endregion
    }
}
