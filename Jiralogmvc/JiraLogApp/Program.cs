using System;
using System.Collections.Generic;
using System.Windows.Forms;

using JiralogMVC.Controller;
using JiralogMVC.View;
//Test for sonarqube
namespace JiralogMVC
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            JiralogView view = new JiralogView();
            JiralogModel model = new JiralogModel();
            view.Visible = false;

            JiralogController controller = new JiralogController( view , model);
            controller.LoadView();
            view.ShowDialog();
        }
    }
}
