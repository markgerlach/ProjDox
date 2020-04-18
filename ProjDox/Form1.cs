using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjDox
{
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //WindowsFormsSettings.AllowRibbonFormGlass = 
            //    DevExpress.Utils.DefaultBoolean.False;

            //ribbonMain.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Show;

            //ribbonMain.tab
        }

        private void ribbonMain_TabIndexChanged(object sender, EventArgs e)
        {
            // When the page changes, change the text
            //ribbonMain.SelectPage(ribbonMain.Pages[1]);
            //if (ribbonMain.SelectedPage.PageIndex == 1)
            //{
            //    return;
            //}
        }

        private void ribbonMain_SelectedPageChanged(object sender, EventArgs e)
        {
            if (ribbonMain.SelectedPage != ribbonPage3)
            {
                // Go to the second page
                ribbonMain.SelectPage(ribbonPage3);
            }
            else
            {
                // Select the first one
                //ribbonMain;
            }
            //ribbonMain.SelectPage(ribbonMain.Pages[1]);
            //ribbonMain
        }
    }
}
