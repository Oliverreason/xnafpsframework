#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#endregion

namespace FPSFramework.Core
{
    public class Log
    {
        #region Fields
        /// <summary>
        /// File error output
        /// </summary>
        private static StreamWriter writer = null;

        /// <summary>
        /// Filename
        /// </summary>
        private const string LogFilename = "LogOutput.txt";
        #endregion

        #region Static Ctor
        static Log()
        {
            // Open file
            FileStream file = new FileStream(
              LogFilename, FileMode.OpenOrCreate,
              FileAccess.Write, FileShare.ReadWrite);
            writer = new StreamWriter(file);
            // Go to end of file
            writer.BaseStream.Seek(0, SeekOrigin.End);
            // Enable auto flush (always be up to date when reading!)
            writer.AutoFlush = true;

            // Add some info about this session
            DateTime ct = DateTime.Now;
            writer.WriteLine("/// Session started at: " +
              ct.Day.ToString("00") + "/" +
              ct.Month.ToString("00") + "/" +
              ct.Year.ToString("0000") + " - " +
              ct.Hour.ToString("00") + ":" +
              ct.Minute.ToString("00") + ":" +
              ct.Second.ToString("00"));
        } // Log()
        #endregion

        #region Methods
        
        /// <summary>
        /// Write log entry
        /// </summary>
        /// <param name="message"></param>
        static public void Write(string message)
        {
            DateTime ct = DateTime.Now;
            string s = "[" + ct.Hour.ToString("00") + ":" +
              ct.Minute.ToString("00") + ":" +
              ct.Second.ToString("00") + "] " +
              message;
            writer.WriteLine(s);

#if DEBUG
            // In debug mode write that message to the console as well!
            System.Console.WriteLine(s);
#endif
        } // Write(message)
        #endregion

    }
}
