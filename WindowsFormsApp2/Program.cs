﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace WindowsFormsApp2
{

    internal static class Program
    {
        //[DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\Prewitt.dll")]
        //public static extern int ExampleFunction(byte[] imageData, int width, int height);
        //[DllImport("C:\\Users\\krzyw\\Source\\Repos\\PrewittFilter\\x64\\Debug\\PrewittAsm.dll")]
        //public static extern int Myproc();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
 
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
