using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNSChecker
{
    public partial class Form1 : Form
    {
        DNS dns;
        public Form1()
        {
            InitializeComponent();
            cboAbfrageart.Items.Add("A");
            cboAbfrageart.Items.Add("AAAA");
            cboAbfrageart.Items.Add("MX");
            cboAbfrageart.Items.Add("PTR");
            cboAbfrageart.SelectedIndex = 0;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            dns.socket.Close();
        }
        void dns_ShowOutput(string output)
        {
            Action update = () => txtResult.AppendText(output);
            txtResult.BeginInvoke(update);
            Action newline = () => txtResult.AppendText("\n");
            txtResult.BeginInvoke(newline);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            txtResult.Clear();
            dns = new DNS(txtServer.Text);
            dns.ShowOutput += dns_ShowOutput;
            if (txtHost.Text != "" && txtServer.Text != "")
            {
                try
                {
                    switch (Convert.ToString(cboAbfrageart.SelectedItem))
                    {
                        case "A":
                            dns.GetIPv4(txtHost.Text);
                            break;
                        case "AAAA":
                            dns.GetIPv6(txtHost.Text);
                            break;
                        case "MX":
                            dns.GetMailServer(txtHost.Text);
                            break;
                        case "PTR":
                            dns.GetWebadress(txtHost.Text);
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    MessageBox.Show("Try again later.");
                }
            }
            else
            {
                MessageBox.Show("Please fill all fields.");
            }

        }
    }
}
