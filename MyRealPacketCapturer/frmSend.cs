using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyRealPacketCapturer
{
    public partial class frmSend : Form
    {
        public static int instantiations = 0;

        public frmSend()
        {
            InitializeComponent();
            instantiations++;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "text files|*.txt|all files|*.*";
            openFileDialog1.Title = "open the captured packets";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                txtPacket.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "text files|*.txt|all files|*.*";
            saveFileDialog1.Title = "save the captured packets";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, txtPacket.Text);
            }
        }

        private void frmSend_FormClosed(object sender, FormClosedEventArgs e)
        {
            instantiations--;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string stringBytes = "";
            foreach (string s in txtPacket.Lines) {
                string[] noComments = s.Split('#');
                string s1 = noComments[0];
                stringBytes += s1 + Environment.NewLine;
            }

            string[] sBytes = stringBytes.Split(new string[] { "\n", "\r\n", " " },
                StringSplitOptions.RemoveEmptyEntries);

            byte[] packet = new byte[sBytes.Length];
            int i= 0;
            
            foreach (string s in sBytes) { packet[i] = Convert.ToByte(s, 16);i++; }

            try
            {
                frmCapture.device.SendPacket(packet);
            }
            catch (Exception exp) { }
            
        }
    }
}
