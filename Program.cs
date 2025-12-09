using System;
using System.Windows.Forms;

namespace CSharpFileScanner
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SolutionScanner());
        }
    }
}