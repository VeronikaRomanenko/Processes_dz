using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listView1.Columns.Add("ProcessName");
            listView1.Columns.Add("Id");
            listView1.Columns.Add("ThreadsCount");
            listView1.Columns.Add("HandleCount");
            listView1.View = View.Details;
            listView2.Columns.Add("Id");
            listView2.Columns.Add("PriorityLevel");
            listView2.Columns.Add("StartTime");
            listView2.View = View.Details;
            listView3.Columns.Add("ModuleName");
            listView3.Columns.Add("FileName");
            listView3.View = View.Details;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            foreach (Process proc in Process.GetProcesses())
            {
                ListViewItem item = new ListViewItem(proc.ProcessName);
                item.SubItems.Add(proc.Id.ToString());
                item.SubItems.Add(proc.Threads.Count.ToString());
                item.SubItems.Add(proc.HandleCount.ToString());
                listView1.Items.Add(item);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "calc.exe";
            proc.Start();
            MessageBox.Show($"Запущен процесс {proc.ProcessName}");
            MessageBox.Show($"Процесс завершился с кодом {proc.ExitCode}");
            MessageBox.Show($"Текущий процесс имеет имя {Process.GetCurrentProcess().ProcessName}");
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            listView2.Items.Clear();
            listView3.Items.Clear();
            Process proc = Process.GetProcessById(int.Parse(listView1.SelectedItems[0].SubItems[1].Text));
            try
            {
                foreach (ProcessThread thread in proc.Threads)
                {
                    ListViewItem item = new ListViewItem(thread.Id.ToString());
                    item.SubItems.Add(thread.PriorityLevel.ToString());
                    item.SubItems.Add(thread.StartTime.ToString());
                    listView2.Items.Add(item);
                }
                foreach (ProcessModule modul in proc.Modules)
                {
                    ListViewItem item = new ListViewItem(modul.ModuleName);
                    item.SubItems.Add(modul.FileName);
                    listView3.Items.Add(item);
                }
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            button1_Click(button1, new EventArgs());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (XmlTextWriter wr = new XmlTextWriter(dialog.FileName, Encoding.Unicode))
                {
                    wr.Formatting = Formatting.Indented;
                    wr.WriteStartDocument();
                    wr.WriteStartElement("Processes");
                    foreach (ListViewItem proc in listView1.Items)
                    {
                        wr.WriteStartElement("Process");
                        wr.WriteElementString("ProcessName", proc.SubItems[0].Text.ToString());
                        wr.WriteElementString("Id", proc.SubItems[1].Text.ToString());
                        wr.WriteElementString("ThreadsCount", proc.SubItems[2].Text.ToString());
                        wr.WriteElementString("HandleCount", proc.SubItems[3].Text.ToString());
                        wr.WriteEndElement();
                    }
                    wr.WriteEndElement();
                    wr.WriteEndDocument();
                }
            }
        }
    }
}
