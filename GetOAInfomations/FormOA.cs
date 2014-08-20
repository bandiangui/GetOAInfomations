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
        /// 导附件+数据
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
            List<ReceiveData> receiveList = null;
            List<SendData> sendList = null;
            List<int> idList = new List<int>();
            int count = 0;
            int annexCount = 0;

            this.label4.Text = string.Format("开始：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Application.DoEvents();

            var handle = new OaInfoHandle();
            switch (type)
            {
                case 2:
                    sendList = new List<SendData>();
                    var sends = handle.GetSends(beginYear);
                    count = sends.Count;

                    list.AddRange(sends.Select(p => new AnnexItem()
                    {
                        Index = string.Format("{0:D8}", p.ID),
                        ID = p.ID,
                        Title = p.Title,
                        SerialNumber = p.SerialNumber,
                        FolderName = string.Format("{0}年发文附件", p.SerialNumber.Substring(p.SerialNumber.IndexOf("[") + 1, 4))
                    }));

                    sendList.AddRange(handle.GetSends(sends));

                    break;
                default:
                    receiveList = new List<ReceiveData>();

                    var receives = handle.GetReceiveses(beginYear, type);
                    count = receives.Count;

                    list.AddRange(receives.Select(p => new AnnexItem()
                    {
                        Index = string.Format("{0:D8}", p.ID),
                        ID = p.ID,
                        Title = p.Title,
                        SerialNumber = p.SerialNumber,
                        FolderName = string.Format("{0}年{1}附件", p.GetDate.HasValue ? p.GetDate.Value.Year : 0, type == 1 ? "收文" : "信访")
                    }));


                    receiveList.AddRange(handle.GetReceiveses(receives));
                    break;
            }
            this.label3.Text = string.Format("数据{0}条 / ", count);
            this.progressBar1.Maximum = count;


            foreach (var r in list)
            {
                string ret = handle.GetAnnex(r);
                if (ret == "none")
                {
                    this.richTextBox1.Text += string.Format("{0}5  {1}({2} {3})  没有附件。", Environment.NewLine, r.ID, r.SerialNumber, r.Title);
                    Application.DoEvents();

                    if (receiveList != null) receiveList.Remove(receiveList.Find(p => p.Receive.ID == r.ID));
                    if (sendList != null) sendList.Remove(sendList.Find(p => p.Send.ID == r.ID));
                }
                else if (!string.IsNullOrEmpty(ret))
                {
                    this.richTextBox1.Text += string.Format("{0}{1}", Environment.NewLine, ret);
                    Application.DoEvents();

                    if (receiveList != null) receiveList.Remove(receiveList.Find(p => p.Receive.ID == r.ID));
                    if (sendList != null) sendList.Remove(sendList.Find(p => p.Send.ID == r.ID));
                }
                else
                {
                    annexCount++;
                }
                this.progressBar1.Value++;
                Application.DoEvents();
            }

            this.label3.Text = string.Format("数据{0}条 / 附件{1}条", count, annexCount);
            Application.DoEvents();


            handle.ExcelImport(beginYear, type, sendList, receiveList);

            this.label5.Text = string.Format("完成：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            this.button2.Enabled = true;
        }
        /// <summary>
        /// 会签文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.button3.Enabled = false;

            this.progressBar1.Minimum = 0;
            this.progressBar1.BackColor = Color.Green;

            List<AnnexItem> list = new List<AnnexItem>();
            List<SendData> sendList = null;
            List<int> idList = new List<int>();
            int count = 0;
            int annexCount = 0;

            this.label4.Text = string.Format("开始：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Application.DoEvents();

            var handle = new OaInfoHandle();
            sendList = new List<SendData>();
            var sends = handle.GetSendLHFW();
            count = sends.Count;

            list.AddRange(sends.Select(p => new AnnexItem()
            {
                Index = string.Format("{0:D8}", p.ID),
                ID = p.ID,
                Title = p.Title,
                SerialNumber = p.SerialNumber,
                FolderName = "会签发文附件"
            }));

            sendList.AddRange(handle.GetSends(sends));

            this.label3.Text = string.Format("数据{0}条 / ", count);
            this.progressBar1.Maximum = count;


            foreach (var r in list)
            {
                string ret = handle.GetAnnex(r);
                if (ret == "none")
                {
                    this.richTextBox1.Text += string.Format("{0}5  {1}({2} {3})  没有附件。", Environment.NewLine, r.ID, r.SerialNumber, r.Title);
                    Application.DoEvents();
                }
                else if (!string.IsNullOrEmpty(ret))
                {
                    this.richTextBox1.Text += string.Format("{0}{1}", Environment.NewLine, ret);
                    Application.DoEvents();
                }
                else
                {
                    annexCount++;
                }
                this.progressBar1.Value++;
                Application.DoEvents();
            }

            this.label3.Text = string.Format("数据{0}条 / 附件{1}条", count, annexCount);
            Application.DoEvents();


            handle.ExcelImport(DateTime.Now.Year, 2, sendList, null);

            this.label5.Text = string.Format("完成：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            this.button3.Enabled = true;
        }
    }
}
