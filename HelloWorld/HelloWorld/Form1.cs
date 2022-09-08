using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWorld
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            backgroundWorker3.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
            else
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            component11.SetIOut(2, 2);
            Thread.Sleep(50);
            component11.SetLoad(true);
            for (int i = 0; i < progressBar1.Maximum; i++)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                
                component11.SetVoltageIndexForTypeC(2,5);
                Thread.Sleep(50);

                if (i % (progressBar1.Maximum / progressBar1.Step) == 0)
                {
                    try
                    {
                        backgroundWorker1.ReportProgress(i);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            component11.SetLoad(false);
            backgroundWorker1.ReportProgress(progressBar1.Maximum);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
                //backgroundWorker1.RunWorkerAsync();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker2.IsBusy)
            {
                backgroundWorker2.CancelAsync();
            }
            else
            {
                backgroundWorker2.RunWorkerAsync();
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                string[] ports = SerialPort.GetPortNames();
                cboPort.Invoke((MethodInvoker)(() => 
                {
                    cboPort.Items.Clear();
                    cboPort.Items.AddRange(ports);
                    cboPort.SelectedIndex = 0;
                } ));
                

                component11.AddParameter();
                component11.SwitchSerialPort();
            }

            if (component11.IsComOpen)
            {
                button2.BackColor = Color.LightGray;
                button2.Text = "Disconnect";
            }

            else
            {
                button2.BackColor = Color.White;
                button2.Text = "Connect";
            }
                
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            //int numChannel = 4;
            //for(int i = 0; i < numChannel; i++)
            //    label1.Text = component11.CurrentArray[i].ToString();
            while (true)
            {
                try
                {
                    component11.ReadAllVoltageAndCurrent();
                    label1.Invoke((MethodInvoker)(() => label1.Text = component11.CurrentArray[0].ToString()));
                    label2.Invoke((MethodInvoker)(() => label2.Text = component11.CurrentArray[1].ToString()));
                    label3.Invoke((MethodInvoker)(() => label3.Text = component11.CurrentArray[2].ToString()));
                    label4.Invoke((MethodInvoker)(() => label4.Text = component11.CurrentArray[3].ToString()));

                    label5.Invoke((MethodInvoker)(() => label5.Text = component11.VoltageArray[0].ToString()));
                    label6.Invoke((MethodInvoker)(() => label6.Text = component11.VoltageArray[1].ToString()));
                    label7.Invoke((MethodInvoker)(() => label7.Text = component11.VoltageArray[2].ToString()));
                    label8.Invoke((MethodInvoker)(() => label8.Text = component11.VoltageArray[3].ToString()));

                    //label1.Text = component11.CurrentArray[0].ToString();
                    //label2.Text = component11.CurrentArray[1].ToString();
                    //label3.Text = component11.CurrentArray[2].ToString();
                    //label4.Text = component11.CurrentArray[3].ToString();
                    //label5.Text = component11.VoltageArray[0].ToString();
                    //label6.Text = component11.VoltageArray[1].ToString();
                    //label7.Text = component11.VoltageArray[2].ToString();
                    //label8.Text = component11.VoltageArray[3].ToString();
                }
                catch(Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cboPort.Items.AddRange(ports);
            cboPort.SelectedIndex = 0;
        }
    }
}
