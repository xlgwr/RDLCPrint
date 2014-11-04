using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RDLCPrint
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            System.Data.DataSet ds = new DataSet();
            ds.ReadXml(System.Windows.Forms.Application.StartupPath + @"\TestData.xml");
            ReportViewer frmRPT = new ReportViewer();
            frmRPT.MainDataSet = ds;
            frmRPT.ReportName = "Test";
            frmRPT.MainDataSourceName = "dsTest_Test";
            frmRPT.ReportPath = System.Windows.Forms.Application.StartupPath + @"\rdlcTest.rdlc";
            frmRPT.ShowDialog();
        }

    }
}