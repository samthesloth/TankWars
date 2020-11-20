// Authors: Nicholas Vaskelis and Sam Peters

using System;
using System.Windows.Forms;

namespace TankWars
{
    internal static class Program
    {
        /// <summary>
        /// Begins application by making a new game controller and then passing that into the form constructor.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GameController gc = new GameController();
            Form1 form = new Form1(gc);
            Application.Run(form);
        }
    }
}