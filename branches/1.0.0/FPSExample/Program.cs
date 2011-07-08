#region Using Statements
using System;
#endregion

namespace FPSExample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            {
                //Need for conversion between string and float/double formats
                System.Globalization.CultureInfo oldCI;
                oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture =
                        new System.Globalization.CultureInfo("en-US");
            }
            using (MyFPSGame game = new MyFPSGame())
            {
                game.Run();
            }
        }
    }
}

