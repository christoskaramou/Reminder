using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reminder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\Reminder", false);
            if (rk == null)
                checkBox1.Checked = false;
            else
                checkBox1.Checked = true;

            string[] binFilesInDir = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.bin");
            foreach (string file in binFilesInDir)
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(file));
            notifyIcon1.Visible = true;
            //int fileNo = 0;
            //string fileName = "reminder" + fileNo + ".bin";
            //while (File.Exists(Directory.GetCurrentDirectory() + "\\" + fileName))
            //{
            //    fileNo++;
            //    fileName = "reminder" + fileNo + ".bin";
            //}
            //textBox2.Text = Path.GetFileNameWithoutExtension(fileName);

            button1.Enabled = true;
            button2.Enabled = true;

            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd - MMM - yyyy,  HH : mm";
            DateTime dt = dateTimePicker1.Value;
            dateTimePicker1.Value = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
            timer1.Interval = 60000;
            timer1.Start();
        }

        private void dateTimePicker1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.dateTimePicker1, true, true, true, true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "New")
            {
                int fileNo = 0;
                string fileName = "reminder" + fileNo + ".bin";
                while (File.Exists(Directory.GetCurrentDirectory() + "\\" + fileName))
                {
                    fileNo++;
                    fileName = "reminder" + fileNo + ".bin";
                }
                textBox2.Text = Path.GetFileNameWithoutExtension(fileName);
                textBox2.Focus();
                button1.Text = "Save";
                button1.Enabled = true;
                button2.Enabled = false;
                listBox1.Enabled = false;
                //timer1.Stop();
                //Formatter f = new Formatter();
                //f.dateAndTimePick = dateTimePicker1.Value;
                //f.text = textBox1.Text;
                //f.enabled = true;
                //IFormatter formatter = new BinaryFormatter();
                //Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                //formatter.Serialize(stream, f);
                //stream.Close();
                //timer1.Start();
                ////MessageBox.Show("Reminder Created");
                //textBox1.Text = "";
                //listBox1.Items.Add(Path.GetFileNameWithoutExtension(fileName));

            }
            else if (button1.Text == "Save")
            {
                if (textBox2.Text != "" && textBox1.Text != "")
                {
                    string file = Directory.GetCurrentDirectory() + "\\" + textBox2.Text + ".bin";
                    if (File.Exists(file))
                    {
                        if (MessageBox.Show("You are going to overwite the file.", "Are you sure?", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            textBox2.Focus();
                            textBox1.Text = "";
                            button1.Text = "New";
                            listBox1.Enabled = true;
                            button1.Enabled = true;
                            button2.Enabled = true;
                            return;
                        }
                    }
                    timer1.Stop();
                    Formatter f = new Formatter();
                    f.dateAndTimePick = dateTimePicker1.Value;
                    f.text = textBox1.Text;
                    f.enabled = true;
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, f);
                    stream.Close();
                    timer1.Start();
                    textBox1.Text = "";
                    button1.Text = "New";
                    listBox1.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;

                    listBox1.Items.Clear();
                    string[] binFilesInDir = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.bin");
                    foreach (string fileN in binFilesInDir)
                        listBox1.Items.Add(Path.GetFileNameWithoutExtension(fileN));
                    //MessageBox.Show("Done");
                }
                else
                {
                    if (textBox2.Text == "")
                    {
                        MessageBox.Show("You can not save to emty file");
                        textBox2.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Please type some text");
                        textBox1.Focus();
                    }
                }  
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            listBox1.Enabled = false;
            button1.Text = "Save";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] binFilesInDir = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.bin");
            foreach (string file in binFilesInDir)
            {
                IFormatter formatter1 = new BinaryFormatter();
                Stream stream1 = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                Formatter f1 = (Formatter)formatter1.Deserialize(stream1);
                stream1.Close();
                if (f1.enabled && f1.dateAndTimePick <= DateTime.Now)
                {
                    // write back to the file for assign the false value to enabled
                    f1.enabled = false;
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, f1);
                    stream.Close();
                    f1.text = f1.dateAndTimePick.ToString() + "\n\n" + f1.text;
                    MessageBox.Show(f1.text, Path.GetFileNameWithoutExtension(file));
                }
            }
        }

        [Serializable]
        public class Formatter
        {
            public DateTime dateAndTimePick;
            public string text;
            public bool enabled;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = listBox1.Text;
            string file = Directory.GetCurrentDirectory() + "\\" + listBox1.Text + ".bin";
            if (File.Exists(file))
            {
                IFormatter formatter1 = new BinaryFormatter();
                Stream stream1 = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                Formatter f1 = (Formatter)formatter1.Deserialize(stream1);
                stream1.Close();
                textBox1.Text = f1.text;
                dateTimePicker1.Value = f1.dateAndTimePick;
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && MessageBox.Show("Delete the selected reminder?", "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                string file = Directory.GetCurrentDirectory() + "\\" + listBox1.Text + ".bin";
                if (File.Exists(file))
                {
                    File.Delete(file);
                    string[] binFilesInDir = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.bin");
                    listBox1.Items.Clear();
                    foreach (string f in binFilesInDir)
                        listBox1.Items.Add(Path.GetFileNameWithoutExtension(f));
                }
                else
                    MessageBox.Show("File not found");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (MessageBox.Show("Closing this application will stop all reminders, continue?", "Closing", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            //    e.Cancel = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (checkBox1.Checked)
            {
                rk.SetValue("Reminder", Application.ExecutablePath.ToString());
                MessageBox.Show("Enabled");
            }
            else
            {
                rk.DeleteValue("Reminder", false);
                MessageBox.Show("Disabled");
            }
        }
    }
}
