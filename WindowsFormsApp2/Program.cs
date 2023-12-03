using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace WindowsFormsApp2
{

    internal static class Program
    {
        [DllImport("C:\\Users\\krzyw\\Source\\Repos\\vigilant-succotash\\WindowsFormsApp2\\x64\\Debug\\Prewitt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ExampleFunction(int a,int b);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
 
        static void Main()
        {
            Console.WriteLine(ExampleFunction(2,9));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
           


        }
    }
}
