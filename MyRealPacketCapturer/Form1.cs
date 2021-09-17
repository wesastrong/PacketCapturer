using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketDotNet;
using SharpPcap;

namespace MyRealPacketCapturer
{
    public partial class Form1 : Form
    {
        CaptureDeviceList devices;
        public static ICaptureDevice device;
        public static string stringPackets = "";
        static int numPackets = 0;

        public Form1()
        {
            InitializeComponent();

            devices = CaptureDeviceList.Instance;

            if (devices.Count < 1)
            {
                MessageBox.Show("No Capture Devices Found!");
                Application.Exit();
            }

            foreach (ICaptureDevice dev in devices)
            {
                cmbDevices.Items.Add(dev.Description);
            }

            device = devices[0];
            cmbDevices.Text = device.Description;


            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
        }

        private static void device_OnPacketArrival(object sender, CaptureEventArgs packet)
        {
            numPackets++;

            stringPackets += "Packet no. " + Convert.ToString(numPackets);
            stringPackets += Environment.NewLine;

            byte[] data = packet.Packet.Data;

            int byteCounter = 0;


            stringPackets += "Destination MAC address: ";
            foreach (byte b in data)
            {
                if (byteCounter <= 13) stringPackets += b.ToString("X2") + " ";
                byteCounter++;

                switch (byteCounter)
                {
                    case 6: stringPackets += Environment.NewLine;
                        stringPackets += "Source MAC address: ";
                        break;
                    case 12: stringPackets += Environment.NewLine;
                        stringPackets += "Ethernet type: ";
                        break;
                    case 14: if (data[12] == 8)
                        {
                            if (data[13] == 0) stringPackets += "(IP)";
                            if (data[13] == 6) stringPackets += "(ARP)";
                        }
                        break;
                }

            }

            stringPackets += Environment.NewLine;
            byteCounter = 0;
            stringPackets += "Raw data" + Environment.NewLine;
            foreach (byte b in data)
            {
                stringPackets += b.ToString("X2") + " ";
                byteCounter++;

                if (byteCounter == 16)
                {
                    byteCounter = 0;
                    stringPackets += Environment.NewLine;
                }
            }
            stringPackets += Environment.NewLine;
            stringPackets += Environment.NewLine;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textCapturedData.AppendText(stringPackets);
            stringPackets = "";
            txtNumPackets.Text = Convert.ToString(numPackets);
        }

        private void btnStartStop_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (btnStartStop.Text == "Start")
                {
                    device.StartCapture();
                    timer1.Enabled = true;
                    btnStartStop.Text = "Stop";
                }
                else
                {
                    device.StopCapture();
                    timer1.Enabled = false;
                    btnStartStop.Text = "Start";
                }
            }
            catch (Exception exp) { }
        }

        private void cmbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            device = devices[cmbDevices.SelectedIndex];
            cmbDevices.Text = device.Description;

            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "text files|*.txt|all files|*.*";
            saveFileDialog1.Title = "save the captured packets";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, textCapturedData.Text);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "text files|*.txt|all files|*.*";
            openFileDialog1.Title = "open the captured packets";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                textCapturedData.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
