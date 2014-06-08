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

        public Form1()
        {
            InitializeComponent();
            DNS dns = new DNS();
            dns.ShowOutput += dns_ShowOutput;
        }


        void dns_ShowOutput(string output)
        {
            Action update = () => lblTest.Text = output;
            lblTest.BeginInvoke(update);
        }




    }
}
