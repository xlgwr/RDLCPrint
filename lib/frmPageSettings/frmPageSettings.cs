using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RDLCReport
{
    public partial class frmPageSettings : Form
    {
        public frmPageSettings(string ReportName)
        {
            InitializeComponent();
            this.m_ReportName = ReportName;
        }
        
        /// <summary>
        /// �������ƣ������ļ���һ��������ʹ�õ�����������ı���
        /// </summary>
        private string m_ReportName = string.Empty;

        /// <summary>
        /// ���ڶ�ȡ��д��XML�����ļ���DataSet
        /// </summary>
        private System.Data.DataSet dsReportSetting = new DataSet();

        /// <summary>
        /// ��ǰm_ReportName�������ñ���ı�
        /// </summary>
        private System.Data.DataTable dtReportSetting = new DataTable();

        /// <summary>
        /// ���������ʱ������Ϣ�������
        /// </summary>
        private System.Data.DataRow drReportSetting = null;


        #region �Զ��庯��


        /// <summary>
        /// ��ȡ�����ô�ӡ���б�
        /// </summary>
        private void GetPrinters()
        {
            System.Collections.ArrayList alPrinters = Printer.GetPrinterList();
            for (int i = 0; i < alPrinters.Count; i++)
                this.cmb��ӡ��.Items.Add(alPrinters[i].ToString());
            alPrinters.Clear();
            alPrinters = null;
        }

        /// <summary>
        /// ͨ��ѡ�еı�����ҳ��ĸ߶ȺͿ��
        /// </summary>
        /// <param name="FormName">ѡ�еı�</param>
        private void GetForm(string FormName)
        {
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();

            pd.PrinterSettings.PrinterName = this.cmb��ӡ��.Text;

            foreach (System.Drawing.Printing.PaperSize ps in pd.PrinterSettings.PaperSizes)
            {
                if (ps.PaperName == FormName)
                {
                    //inת��Ϊ����
                    this.txt���߶�.Text = Printer.FromInchToCM(System.Convert.ToDecimal(ps.Height.ToString())).ToString("#.00");
                    this.txt�����.Text = Printer.FromInchToCM(System.Convert.ToDecimal(ps.Width.ToString())).ToString("#.00");
                }
            }            

            pd.Dispose();
        }
        
        /// <summary>
        /// ��ȡָ����ӡ���ı��б�
        /// </summary>
        private void GetForms()
        {
            this.cmb��.Items.Clear();

            if (this.cmb��ӡ��.Text.Trim() == string.Empty)
                return;

            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();

            pd.PrinterSettings.PrinterName = this.cmb��ӡ��.Text;

            foreach (System.Drawing.Printing.PaperSize ps in pd.PrinterSettings.PaperSizes)
                this.cmb��.Items.Add(ps.PaperName);

            pd.Dispose();
        }

        /// <summary>
        /// �������ļ����ж������ļ���ϵͳ�е������Ƿ�һ��
        /// </summary>
        private void ReadSetting()
        {

            this.dsReportSetting.ReadXml(System.Windows.Forms.Application.StartupPath + @"\ReportSettings.xml");

            this.GetPrinters();

            foreach( System.Data.DataTable dt in this.dsReportSetting.Tables )
            {
                if (dt.TableName == this.m_ReportName)
                {
                    this.dtReportSetting = dt;
                    break;
                }
            }

            if (this.dtReportSetting.Rows.Count != 0)
            {
                this.drReportSetting = this.dtReportSetting.Rows[0];

                if (! Printer.PrinterInList(this.drReportSetting["PrinterName"].ToString()))
                {
                    this.epError.SetError(this.cmb��ӡ��, "ϵͳ�в������Ѿ������������ļ��еĴ�ӡ�������������ã�");
                    return;
                }
                else
                {
                    this.cmb��ӡ��.Text = this.drReportSetting["PrinterName"].ToString();
                    this.GetForms();

                    if (!Printer.FormInPrinter(this.cmb��ӡ��.Text.Trim(), this.drReportSetting["PaperName"].ToString()))
                    {
                        this.epError.SetError(this.cmb��, "��ǰ��ӡ���в����ڱ����������ļ��еı������������ã�");
                        return;
                    }
                    else
                    {
                        this.cmb��.Text = this.drReportSetting["PaperName"].ToString();
                        this.GetForm(this.cmb��.Text);

                        if (!Printer.FormSameSize(this.cmb��ӡ��.Text.Trim(), this.cmb��.Text.Trim(), System.Convert.ToDecimal(this.txt�����.Text.Trim()), System.Convert.ToDecimal(this.txt���߶�.Text.Trim())))
                        {
                            this.epError.SetError(this.txt���߶�, "���汨������ʱ������д�ı���߶�Ӧ�ú͵�ǰ��ѡ��ı��߶�һ�£�");
                            this.epError.SetError(this.txt�����, "���汨������ʱ������д�ı�����Ӧ�ú͵�ǰ��ѡ��ı����һ�£�");
                        }
                    }

                }

                if (dtReportSetting.Rows[0]["Orientation"].ToString() == "����")
                    this.rad����.Checked = true;
                else if (dtReportSetting.Rows[0]["Orientation"].ToString() == "����")
                    this.rad����.Checked = true;

                this.txt��߾�.Text = this.drReportSetting["MarginLeft"].ToString();
                this.txt�ұ߾�.Text = this.drReportSetting["MarginRight"].ToString();
                this.txt�����߾�.Text = this.drReportSetting["MarginTop"].ToString();
                this.txt�ײ��߾�.Text = this.drReportSetting["MarginBottom"].ToString();

            }
            else
            {
                this.dtReportSetting.TableName = this.m_ReportName;
                this.dtReportSetting.Columns.Add("ReportName", System.Type.GetType("System.String"));
                this.dtReportSetting.Columns.Add("PrinterName", System.Type.GetType("System.String"));
                this.dtReportSetting.Columns.Add("PaperName", System.Type.GetType("System.String"));
                this.dtReportSetting.Columns.Add("PageWidth", System.Type.GetType("System.String"));
                this.dtReportSetting.Columns.Add("PageHeight", System.Type.GetType("System.String"));
                this.dtReportSetting.Columns.Add("MarginTop", System.Type.GetType("System.String"));
                this.dtReportSetting.Columns.Add("MarginBottom", System.Type.GetType("System.String"));
                this.dtReportSetting.Columns.Add("MarginLeft", System.Type.GetType("System.String"));
                this.dtReportSetting.Columns.Add("MarginRight", System.Type.GetType("System.String"));
                this.dtReportSetting.Columns.Add("Orientation", System.Type.GetType("System.String"));
                this.drReportSetting = this.dtReportSetting.NewRow();                
            }

        }

        /// <summary>
        /// д�����ļ����ڴ�֮ǰ�ж����ݵ���Ч��
        /// </summary>
        private bool WriteSetting()
        {

            bool bolRet = false;

            if (this.cmb��ӡ��.Text.Trim() == string.Empty)
            {
                this.epError.SetError(this.cmb��ӡ��, "���汨������ʱ����ѡ��һ����ӡ����");
                return false;
            }

            if (!Printer.PrinterInList(this.cmb��ӡ��.Text.Trim()))
            {
                this.epError.SetError(this.cmb��ӡ��, "���汨������ʱ����ѡ��ӡ��Ӧ���ڿ��ô�ӡ���б��У�");
                return false;
            }

            if (this.cmb��.Text.Trim() == string.Empty)
            {
                this.epError.SetError(this.cmb��, "���汨������ʱ����ѡ��һ������");
                return false;
            }

            if (!Printer.FormInPrinter(this.cmb��ӡ��.Text.Trim(), this.cmb��.Text.Trim()))
            {
                this.epError.SetError(this.cmb��, "���汨������ʱ����ѡ��Ӧ���ڵ�ǰ��ӡ�����õı��б��У�");
                return false;
            }

            decimal decOut = 0;

            if (this.txt�����.Text.Trim() != string.Empty)
            {
                if (!decimal.TryParse(this.txt�����.Text, out decOut))
                {
                    this.epError.SetError(this.txt�����, "���������ʱ������д�ı����Ӧ����һ������λС�������֣�");
                    return false;
                }
            }
            else
            {
                this.epError.SetError(this.txt�����, "���������ʱ������д�ı���Ȳ���Ϊ�գ�");
                return false;
            }

            if (this.txt���߶�.Text.Trim() != string.Empty)
            {
                if (!decimal.TryParse(this.txt���߶�.Text, out decOut))
                {
                    this.epError.SetError(this.txt���߶�, "���������ʱ������д�ı��߶�Ӧ����һ������λС�������֣�");
                    return false;
                }
            }
            else
            {
                this.epError.SetError(this.txt���߶�, "���������ʱ������д�ı��߶Ȳ���Ϊ�գ�");
                return false;
            }

            if (!Printer.FormSameSize(this.cmb��ӡ��.Text.Trim(), this.cmb��.Text.Trim(), System.Convert.ToDecimal(this.txt�����.Text.Trim()), System.Convert.ToDecimal(this.txt���߶�.Text.Trim())))
            {
                this.epError.SetError(this.txt���߶�, "���汨������ʱ������д�ı���߶�Ӧ�ú͵�ǰ��ѡ��ı��߶�һ�£�");
                this.epError.SetError(this.txt�����, "���汨������ʱ������д�ı�����Ӧ�ú͵�ǰ��ѡ��ı����һ�£�");
                return false;
            }

            if (!decimal.TryParse(this.txt��߾�.Text, out decOut))
            {
                this.epError.SetError(this.txt��߾�, "���������ʱ������д����߾�Ӧ����һ������λС�������֣�");
                return false;
            }

            if (!decimal.TryParse(this.txt�ұ߾�.Text, out decOut))
            {
                this.epError.SetError(this.txt�ұ߾�, "���������ʱ������д���ұ߾�Ӧ����һ������λС�������֣�");
                return false;
            }

            if (!decimal.TryParse(this.txt�����߾�.Text, out decOut))
            {
                this.epError.SetError(this.txt�����߾�, "���������ʱ������д�Ķ����߾�Ӧ����һ������λС�������֣�");
                return false;
            }

            if (!decimal.TryParse(this.txt�ײ��߾�.Text, out decOut))
            {
                this.epError.SetError(this.txt�ײ��߾�, "���������ʱ������д�ĵײ��߾�Ӧ����һ������λС�������֣�");
                return false;
            }
            
            this.drReportSetting["ReportName"] = this.m_ReportName;
            this.drReportSetting["PrinterName"] = this.cmb��ӡ��.Text.Trim();
            this.drReportSetting["PaperName"] = this.cmb��.Text.Trim();
            this.drReportSetting["PageWidth"] = this.txt�����.Text.Trim();
            this.drReportSetting["PageHeight"] = this.txt���߶�.Text.Trim();

            if (this.txt�����߾�.Text.Trim() != string.Empty)
                this.drReportSetting["MarginTop"] = this.txt�����߾�.Text.Trim();
            else
                this.drReportSetting["MarginTop"] = "0";

            if (this.txt�ײ��߾�.Text.Trim() != string.Empty)
                this.drReportSetting["MarginBottom"] = this.txt�ײ��߾�.Text.Trim();
            else
                this.drReportSetting["MarginBottom"] = "0";

            if (this.txt��߾�.Text.Trim() != string.Empty)
                this.drReportSetting["MarginLeft"] = this.txt��߾�.Text.Trim();
            else
                this.drReportSetting["MarginLeft"] = "0";

            if (this.txt�ұ߾�.Text.Trim() != string.Empty)
                this.drReportSetting["MarginRight"] = this.txt�ұ߾�.Text.Trim();
            else
                this.drReportSetting["MarginRight"] = "0";

            if (this.rad����.Checked)
                this.drReportSetting["Orientation"] = "����";
            else
                this.drReportSetting["Orientation"] = "����";

            if (this.dtReportSetting.Rows.IndexOf(this.drReportSetting) == -1)
                this.dtReportSetting.Rows.Add(this.drReportSetting);

            if (this.dsReportSetting.Tables.IndexOf(this.dtReportSetting) == -1)
                this.dsReportSetting.Tables.Add(this.dtReportSetting);

            this.dsReportSetting.WriteXml(System.Windows.Forms.Application.StartupPath + @"\ReportSettings.xml");

            bolRet = true;

            return bolRet;

        }
        

        #endregion


        #region ϵͳ�¼�


        private void frmPageSettings_Load(object sender, EventArgs e)
        {
            this.ReadSetting();
        }
        
        private void cmb��ӡ��_SelectedValueChanged(object sender, EventArgs e)
        {
            //Pub.WinForm.Msg.Information(this.cmb��ӡ��.Text);
            this.GetForms();
        }

        private void btn״̬_Click(object sender, EventArgs e)
        {
            Pub.WinForm.Msg.Information("��ӡ����" + this.cmb��ӡ��.Text + "����ǰ״̬�� \r\n    " + Printer.GetPrinterStatus(this.cmb��ӡ��.Text));
        }

        private void btnĬ�ϴ�ӡ��_Click(object sender, EventArgs e)
        {
            this.cmb��ӡ��.Text = Printer.GetDeaultPrinterName();
            this.GetForms();
            this.cmb��.Text = string.Empty;
            this.txt�����.Text = string.Empty;
            this.txt���߶�.Text = string.Empty;
        }

        private void btn��ӱ�_Click(object sender, EventArgs e)
        {
            if (this.cmb��ӡ��.Text.Trim() != string.Empty && this.txt��.Text.Trim() != string.Empty && this.txt�����.Text.Trim() != string.Empty && this.txt���߶�.Text.Trim() != string.Empty)
            {

                System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();

                pd.PrinterSettings.PrinterName = this.cmb��ӡ��.Text.Trim();

                bool bolExist = false;

                foreach (System.Drawing.Printing.PaperSize ps in pd.PrinterSettings.PaperSizes)
                {
                    if (ps.PaperName == this.txt��.Text.Trim())
                    {
                        bolExist = true;
                        break;
                    }
                }

                pd.Dispose();

                if (bolExist)
                {
                    Pub.WinForm.Msg.Warning("Ҫ��ӵı��Ѿ����ڣ���ɾ���Ժ�����ӣ�");
                    return;
                }

                Printer.AddCustomPaperSize(this.cmb��ӡ��.Text, this.txt��.Text.Trim(), System.Convert.ToSingle(this.txt�����.Text.Trim()) * 10, System.Convert.ToSingle(this.txt���߶�.Text.Trim()) * 10);
                this.GetForms();
                this.cmb��.Text = this.txt��.Text.Trim();
            }
            else
            {
                decimal decOut = 0;
                if (this.cmb��ӡ��.Text.Trim() == string.Empty)
                    this.epError.SetError(this.cmb��ӡ��, "����Զ����ʱ����ѡ��һ�����õĴ�ӡ����");
                if (this.txt��.Text.Trim() == string.Empty)
                    this.epError.SetError(this.txt��, "����Զ����ʱ������дһ�����ʵı�����");
                if (this.txt���߶�.Text.Trim() == string.Empty || !System.Decimal.TryParse(this.txt���߶�.Text.Trim(), out decOut))
                    this.epError.SetError(this.txt���߶�, "����Զ����ʱ����ָ��һ�������͵ı��߶ȣ�");
                if (this.txt�����.Text.Trim() == string.Empty || !System.Decimal.TryParse(this.txt�����.Text.Trim(), out decOut))
                    this.epError.SetError(this.txt�����, "����Զ����ʱ����ָ��һ�������͵ı���ȣ�");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.cmb��.Text.Trim() != string.Empty && this.cmb��ӡ��.Text.Trim() != string.Empty)
            {
                if (Pub.WinForm.Msg.Question("ȷ��ɾ��ѡ�еı���") == DialogResult.Yes)
                {
                    Printer.DeleteCustomPaperSize(this.cmb��ӡ��.Text.Trim(), this.cmb��.Text.Trim());
                    this.GetForms();
                    this.cmb��.Text = string.Empty;
                }
            }
            else
            {
                if (this.cmb��.Text.Trim() == string.Empty)
                    this.epError.SetError(this.cmb��, "ɾ���Զ����ʱ����ѡ��һ���Զ������");
                if (this.cmb��ӡ��.Text.Trim() == string.Empty)
                    this.epError.SetError(this.cmb��ӡ��, "ɾ���Զ����ʱ����ѡ��һ����ӡ����");
            }
        }

        private void cmb��_SelectedValueChanged(object sender, EventArgs e)
        {
            this.GetForm(this.cmb��.Text);
        }

        private void btnȷ��_Click(object sender, EventArgs e)
        {
            if (!this.WriteSetting())
                return;
            
            //���ʹ�õĶ��������ռ�õ���Դ
            this.dtReportSetting.Dispose();

            this.dsReportSetting.Dispose();

            this.Close();

        }

        private void btnȡ��_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        #endregion

    }
}