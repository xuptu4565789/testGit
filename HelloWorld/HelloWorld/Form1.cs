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
        private string portName = "";
        private int baudRate = 115200;
        private Parity parity = Parity.None;
        private StopBits stopBits = StopBits.One;
        private int indexOfCapabilities = 1;

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

        //private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    component11.SetLoad(true);
        //    for (int i = 0; i < progressBar1.Maximum; i++)
        //    {
        //        if (backgroundWorker1.CancellationPending)
        //        {
        //            e.Cancel = true;
        //            return;
        //        }
        //        if (i < 20)
        //            component11.SetVoltageIndexForTypeC(2, 1);
        //        else if (i < 40)
        //            component11.SetVoltageIndexForTypeC(2, 2);
        //        else if (i < 60)
        //            component11.SetVoltageIndexForTypeC(2, 3);
        //        else if (i < 80)
        //            component11.SetVoltageIndexForTypeC(2, 4);
        //        else
        //            component11.SetVoltageIndexForTypeC(2, 5);
        //        Thread.Sleep(50);

        //        if (i % (progressBar1.Maximum / progressBar1.Step) == 0)
        //        {
        //            try
        //            {
        //                backgroundWorker1.ReportProgress(i);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.Write(ex.Message);
        //            }
        //        }
        //    }
        //    component11.SetLoad(false);
        //    backgroundWorker1.ReportProgress(progressBar1.Maximum);
        //}

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int retry = 3;
            while (!(Math.Abs(component11.Capabilties[indexOfCapabilities - 1] - component11.VoltageArray[1]) < 1) && retry > 0)
            {
                retry--;
                component11.SetVoltageIndexForTypeC(2, indexOfCapabilities);
            }
                
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    return;
                    //backgroundWorker1.RunWorkerAsync();
                }
            }
            finally
            {
                //component11.SetVoltageIndexForTypeC(2, 1);
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
                try
                {
                    component11.AddParameter(new object[] { portName, baudRate , parity , stopBits });
                    component11.SwitchSerialPort();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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

                    //component11.ReadAllSettedTypeCVoltage();

                    label13.Invoke((MethodInvoker)(() => label13.Text = component11.SettedVoltageArray[0].ToString()));
                    label14.Invoke((MethodInvoker)(() => label14.Text = component11.SettedVoltageArray[1].ToString()));
                    label15.Invoke((MethodInvoker)(() => label15.Text = component11.SettedVoltageArray[2].ToString()));
                    label16.Invoke((MethodInvoker)(() => label16.Text = component11.SettedVoltageArray[3].ToString()));

                    //label1.Text = component11.CurrentArray[0].ToString();
                    //label2.Text = component11.CurrentArray[1].ToString();
                    //label3.Text = component11.CurrentArray[2].ToString();
                    //label4.Text = component11.CurrentArray[3].ToString();
                    //label5.Text = component11.VoltageArray[0].ToString();
                    //label6.Text = component11.VoltageArray[1].ToString();
                    //label7.Text = component11.VoltageArray[2].ToString();
                    //label8.Text = component11.VoltageArray[3].ToString();
                    Thread.Sleep(1000);
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
            cboPort.SelectedIndex = ports.Length-1;
            portName = cboPort.Text;
            comboBox1.SelectedIndex = 3;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;
            comboBox4.SelectedIndex = 0;
        }

        private void cboPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            portName = cboPort.Text;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            baudRate = Convert.ToInt32(comboBox1.Text);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            parity = (Parity)Enum.Parse(typeof(Parity), comboBox2.Text);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            stopBits = (StopBits)Enum.Parse(typeof(StopBits), comboBox3.Text);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            indexOfCapabilities = Convert.ToInt32(comboBox4.Text);
        }
    }
}
