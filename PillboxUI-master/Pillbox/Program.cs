using Microsoft.VisualBasic.ApplicationServices;
using static System.Net.WebRequestMethods;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace Pillbox
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new mainForm());
            Application.Exit();
        }
    }
}