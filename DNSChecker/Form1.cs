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
            //Fill the combobox and set to first item
            cboType.Items.Add("A");
            cboType.Items.Add("AAAA");
            cboType.Items.Add("MX");
            cboType.Items.Add("PTR");
            cboType.SelectedIndex = 0;
        }

        //=================================
        //Events
        //=================================

        /// <summary>
        /// Handles the click event of the send button.
        /// </summary>
        private void btnSend_Click(object sender, EventArgs e)
        {
            txtResult.Clear();
            //Initialize new DNS
            dns = new DNS(txtServer.Text);
            dns.ShowOutput += dns_ShowOutput;
            //Check if no fields are empty
            if (txtHost.Text != "" && txtServer.Text != "")
            {
                try
                {
                    switch (Convert.ToString(cboType.SelectedItem))
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
                            dns.GetWebaddress(txtHost.Text);
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    //Evades the exceeptions when too many queries are queued
                    MessageBox.Show("Try again in a few moments and check your entries.");
                }
            }
            else
            {
                //Not all fields are written into
                MessageBox.Show("Please fill all fields.");
            }
        }

        /// <summary>
        /// Handles all the Output.
        /// </summary>
        void dns_ShowOutput(string output)
        {
            //Updates the resultbox after the Ouput event
            Action update = () => txtResult.AppendText(output);
            txtResult.BeginInvoke(update);
            Action newline = () => txtResult.AppendText("\n");
            txtResult.BeginInvoke(newline);
        }

        /// <summary>
        /// Handles when field of the combobox is changed.
        /// </summary>
        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtHost.Clear();
        }

        /// <summary>
        /// Handles the action while closing the program.
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //Closes the socket so it does not have to time out
            if (dns != null)
            {
                dns.socket.Close();
            }
        }
    }
}
