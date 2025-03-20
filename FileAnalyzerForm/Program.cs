using System;
using System.Windows.Forms;

namespace FileAnalyzerForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            loginForm login = new loginForm();

            if (login.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new programForm());
            }
        }

    }
}
