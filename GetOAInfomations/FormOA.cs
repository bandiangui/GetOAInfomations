using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NNWebFlow.DBContexts.OA;

namespace GetOAInfomations
{
    public partial class FormOA : Form
    {
        public FormOA()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 导数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

            this.button1.Enabled = false;

            int beginYear = this.dateTimePicker1.Value.Year;
            int type = (this.comboBox1.SelectedItem as ComboxItem).Valud;

            List<ReceiveData> receiveList = null;
            List<SendData> sendList = null;
            int count = 0;
            this.label4.Text = string.Format("开始：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Application.DoEvents();
            var handle = new OaInfoHandle();
            switch (type)
            {
                case 2:
                    sendList = new List<SendData>();

                    var sends = handle.GetSends(beginYear);
                    count = sends.Count;

                    sendList.AddRange(handle.GetSends(sends));
                    break;
                default:
                    receiveList = new List<ReceiveData>();

                    var receives = handle.GetReceiveses(beginYear, type);
                    count = receives.Count;

                    receiveList.AddRange(handle.GetReceiveses(receives));
                    break;
            }

            this.label3.Text = string.Format("共{0}条数据。", count);
            Application.DoEvents();

            handle.ExcelImport(beginYear, type, sendList, receiveList);
            this.label5.Text = string.Format("完成：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            this.button1.Enabled = true;
        }
        /// <summary>
        /// 导附件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.button2.Enabled = false;

            int beginYear = this.dateTimePicker1.Value.Year;
            int type = (this.comboBox1.SelectedItem as ComboxItem).Valud;

            this.progressBar1.Minimum = 0;
            this.progressBar1.BackColor = Color.Green;

            List<AnnexItem> list = new List<AnnexItem>();
            int count = 0;
            int annexCount = 0;
            this.label4.Text = string.Format("开始：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Application.DoEvents();
            var handle = new OaInfoHandle();


            switch (type)
            {
                case 2:
                    var sendList = handle.GetSends(beginYear);
                    count = sendList.Count;

                    list.AddRange(sendList.Select(p => new AnnexItem()
                    {
                        Index = string.Format("{0:D8}", p.ID),
                        ID = p.ID,
                        Title = p.Title,
                        SerialNumber = p.SerialNumber,
                        FolderName = string.Format("{0}年发文附件", p.SerialNumber.Substring(p.SerialNumber.IndexOf("[") + 1, 4))
                    }));
                    break;
                default:
                    var receiveList = handle.GetReceiveses(beginYear, type);
                    count = receiveList.Count;

                    list.AddRange(receiveList.Select(p => new AnnexItem()
                    {
                        Index = string.Format("{0:D8}", p.ID),
                        ID = p.ID,
                        Title = p.Title,
                        SerialNumber = p.SerialNumber,
                        FolderName = string.Format("{0}年{1}附件", p.GetDate.HasValue ? p.GetDate.Value.Year : 0, type == 1 ? "收文" : "信访")
                    }));
                    break;
            }
            this.label3.Text = string.Format("数据{0}条 / ", count);
            this.progressBar1.Maximum = count;


            foreach (var r in list)
            {
                string ret = handle.GetAnnex(r);
                if (!string.IsNullOrEmpty(ret))
                {
                    this.richTextBox1.Text += string.Format("{0}{1}", Environment.NewLine, ret);
                }
                else
                {
                    annexCount++;
                }
                this.progressBar1.Value++;
                Application.DoEvents();
            }

            this.label3.Text += string.Format("数据{0}条 / 附件{1}条", count, annexCount);
            Application.DoEvents();

            this.label5.Text = string.Format("完成：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            this.button2.Enabled = true;
        }
        /// <summary>
        /// 数据与附件的检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.button3.Enabled = false;

            int beginYear = this.dateTimePicker1.Value.Year;
            int type = (this.comboBox1.SelectedItem as ComboxItem).Valud;

            this.label4.Text = string.Format("开始：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Application.DoEvents();

            List<AnnexItem> list = new List<AnnexItem>();
            var handle = new OaInfoHandle();
            int count = 0;
            int annexCount = 0;
            List<string> exList = new List<string>();

            switch (type)
            {
                case 2:
                    var sendList = handle.GetSends(beginYear);

                    list.AddRange(sendList.Select(p => new AnnexItem()
                    {
                        Index = string.Format("{0:D8}", p.ID),
                        ID = p.ID,
                        Title = p.Title,
                        SerialNumber = p.SerialNumber,
                        FolderName = string.Format("{0}年发文附件", p.SerialNumber.Substring(p.SerialNumber.IndexOf("[") + 1, 4))
                    }));
                    break;
                default:
                    var receiveList = handle.GetReceiveses(beginYear, type);

                    list.AddRange(receiveList.Select(p => new AnnexItem()
                    {
                        Index = string.Format("{0:D8}", p.ID),
                        ID = p.ID,
                        Title = p.Title,
                        SerialNumber = p.SerialNumber,
                        FolderName = string.Format("{0}年{1}附件", p.GetDate.HasValue ? p.GetDate.Value.Year : 0, type == 1 ? "收文" : "信访")
                    }));
                    break;
            }
            count = list.Count;
            this.richTextBox1.Text = string.Format("公文条目：{0}", count);
            Application.DoEvents();

            foreach (var r in list)
            {
                var aList = handle.GetAnnexList(r).Any();
                var doc = handle.GetDocument(r);

                if (aList || doc != null)
                {
                    annexCount++;
                }

                if (!aList && doc == null)
                {
                    exList.Add(string.Format("{0} {1} {2}", r.ID, r.SerialNumber, r.Title));
                }
            }

            this.richTextBox1.Text += string.Format("           附件条目：{0}", annexCount);
            Application.DoEvents();

            this.richTextBox1.Text += string.Format("{0}没有附件的公文：{1}", Environment.NewLine, annexCount);
            this.richTextBox1.Text += string.Format("{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, exList));
            Application.DoEvents();

            this.label5.Text = string.Format("完成：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            this.button3.Enabled = true;
        }
    }
}
