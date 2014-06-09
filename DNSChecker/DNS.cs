using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Net.Mail;

namespace DNSChecker
{
    class DNS
    {
        public Socket socket;
        IPAddress dnsserver;
        IPEndPoint dnsendpoint;

        byte[] Send;
        byte[] rebuffer;

        event Receive OnReceive;
        public event Output ShowOutput;

        string qtype;
        string host;
        string answer;
        List<string> answers;

        string[] domains = { ".com", ".de", ".net"};

        public DNS(string dnsip)
        {
            answers = new List<string>();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            dnsserver = IPAddress.Parse(dnsip);
            dnsendpoint = new IPEndPoint(dnsserver, 53);
            OnReceive += DNS_OnReceive;
        }
        void DNS_OnReceive(byte[] receive)
        {
            int startcounter = host.Length;
            switch (qtype)
            {
                case "A":
                    answer = "";
                    //Set startcounter to offset
                    startcounter = 30 + host.Length;
                    for (int i = startcounter; i < receive.Length; )
                    {
                        for (int j = 0; j < 4 && i < receive.Length; j++, i++)
                        {
                            answer += Convert.ToString(receive[i]) + ".";
                        }
                        if (!answer.StartsWith("0"))
                        {
                            answers.Add("IP: "+answer.TrimEnd('.'));
                        }
                        answer = "";
                        i += 12;//Bytes to next IP address
                    }
                    foreach (string s in answers)
                    {
                        ShowOutput(s);
                    }
                    break;
                case "AAAA":
                    //Offset
                    answer = "";
                    startcounter = 12 + host.Length;
                    for (int i = startcounter; i < receive.Length;i++)
                    {
                        if (receive[i] != 0)
                        {
                            answer += Convert.ToChar(receive[i]);
                        }
                    }
                    ShowOutput(answer);
                    break;
                case "MX":
                    answer = "";
                    //Set startcounter to offset
                    startcounter = host.Length + 33;
                    for (int i = startcounter; i < receive.Length; i++)
                    {
                        if (receive[i] != 0)
                        {
                            if (receive[i] < 20)
                            {
                                answer += '.';
                            }
                            else if(receive[i]==192)
                            {
                                break;
                            }
                            else
                            {
                                answer += Convert.ToChar(receive[i]);
                            }
                        }
                    }
                    ShowOutput("Address: "+answer);
                    break;
                case "PTR":
                    //Set startcounter to offset
                    startcounter = host.Length+18+13+13/*in-addr.arpa*/;
                    answer = "";
                    for (int i = startcounter; i < receive.Length; i++)
                    {
                        if (receive[i] != 0)
                        {
                            if (receive[i] < 20)
                            {
                                answer += '.';
                            }
                            /*else if (receive[i] == 0 | receive[i] == 192)
                                break;*/
                            else
                            {
                                answer += Convert.ToChar(receive[i]);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    ShowOutput("Address: " + answer);
                    break;
                default:
                    break;
            }
        }
        public void GetIPv4(string host)
        {
            if (host.StartsWith("www."))
            {
                host = host.Remove(0, 4);
            }
            this.host = host;
            string[] addressparts = SplitAddress(host);
            Send = new byte[17 + GetTotalLength(addressparts)];
            SetHeader();
            for (int i = 12; i < Send.Length; )
            {
                int k = 13;
                for (int j = 0; j < addressparts.Length; j++)
                {
                    char[] help = addressparts[j].ToArray();
                    if (i < Send.Length)
                    {
                        Send[i] = Convert.ToByte(addressparts[j].Length);
                        for (int l = 0; k < Send.Length && l < help.Length; k++, l++)
                        {
                            Send[k] = Convert.ToByte(help[l]);
                        }
                        i += addressparts[j].Length + 1;
                        k++;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            }
            SetQType("A");
            SendToDNS();
        }
        public void GetMailServer(string host)
        {
            MailAddress server = new MailAddress(host);
            host = server.Host;
            this.host = host;
            Send = new byte[18 + host.Length];
            SetHeader();
            for (int i = 0; i < host.Length; i++)
            {
                if (host[i] == '.')
                {
                    Send[12] = Convert.ToByte(i);
                    Send[i + 13] = Convert.ToByte(host.Length - i - 1);
                }
                else
                {
                    Send[i + 13] = Convert.ToByte(host[i]);
                }
            }
            SetQType("MX");
            SendToDNS();
            try
            {
                GetIPv4(answer);
            }
            catch
            {
                try
                {
                    foreach (string s in domains)
                    {
                        GetIPv4(answer + s);
                    }
                }
                catch
                {
                    try
                    {
                        GetIPv4(answer + host);
                    }
                    catch
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }
        public void GetIPv6(string host)
        {
            if (host.StartsWith("www."))
            {
                host = host.Remove(0, 4);
            }
            this.host = host;
            string[] addressparts = SplitAddress(host);
            Send = new byte[17 + GetTotalLength(addressparts)];
            for (int i = 12; i < Send.Length; )
            {
                int k = 13;
                for (int j = 0; j < addressparts.Length; j++)
                {
                    char[] help = addressparts[j].ToArray();
                    if (i < Send.Length)
                    {
                        Send[i] = Convert.ToByte(addressparts[j].Length);
                        for (int l = 0; k < Send.Length && l < help.Length; k++, l++)
                        {
                            Send[k] = Convert.ToByte(help[l]);
                        }
                        i += addressparts[j].Length + 1;
                        k++;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            }
            SetHeader();
            SetQType("AAAA");
            SendToDNS();
        }
        public void GetWebadress(string host)
        {
            this.host = host;
            string[] addressparts = SplitAddress(host);
            Send = new byte[17 + GetTotalLength(addressparts)+13];
            byte[] reverse = new byte[Send.Length];
            for (int i = 12; i < Send.Length; )
            {
                int k = 13;
                for (int j = addressparts.Length-1; j >= 0; j--)
                {
                    char[] help = addressparts[j].ToArray();
                    if (i < Send.Length)
                    {
                        Send[i] = Convert.ToByte(addressparts[j].Length);
                        for (int l = 0; k < Send.Length && l < help.Length; k++, l++)
                        {
                            Send[k] = Convert.ToByte(help[l]);
                        }
                        i += addressparts[j].Length + 1;
                        k++;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            }
            Send[Send.Length - 18] = 7;
            Send[Send.Length - 17] = Convert.ToByte('i');
            Send[Send.Length - 16] = Convert.ToByte('n');
            Send[Send.Length - 15] = Convert.ToByte('-');
            Send[Send.Length - 14] = Convert.ToByte('a');
            Send[Send.Length - 13] = Convert.ToByte('d');
            Send[Send.Length - 12] = Convert.ToByte('d');
            Send[Send.Length - 11] = Convert.ToByte('r');
            Send[Send.Length - 10] = 4;
            Send[Send.Length - 9] = Convert.ToByte('a');
            Send[Send.Length - 8] = Convert.ToByte('r');
            Send[Send.Length - 7] = Convert.ToByte('p');
            Send[Send.Length - 6] = Convert.ToByte('a');
            SetHeader();
            SetQType("PTR");
            SendToDNS();
        }
        public void SendToDNS()
        {
            socket.Connect(dnsendpoint);
            try
            {
                socket.SendTo(Send, dnsendpoint);
                rebuffer = new byte[170];
                socket.Receive(rebuffer, SocketFlags.None);
            }
            catch
            {
                throw new Exception();
            }
            OnReceive(rebuffer);
            //try catch on null response
            //socket.Close();
        }
        public void SetHeader()
        {
            Send[0] = Convert.ToByte('x');
            Send[1] = Convert.ToByte('y');
            Send[2] = 1;
            Send[3] = 0;
            Send[4] = 0;
            Send[5] = 1;
            for (int i = 6; i < 12; i++)
            {
                Send[i] = 0;
            }
            Send[Send.Length - 1] = 1;
            Send[Send.Length - 2] = 0;
        }
        public void SetQType(string shortcut)
        {
            switch (shortcut)
            {
                case "A":
                    Send[Send.Length - 3] = 1;
                    Send[Send.Length - 4] = 0;
                    qtype = "A";
                    break;
                case "AAAA":
                    Send[Send.Length - 3] = 0xC;
                    Send[Send.Length - 4] = 1;
                    qtype = "AAAA";
                    break;
                case "MX":
                    Send[Send.Length - 3] = 0xF;
                    Send[Send.Length - 4] = 0;
                    qtype = "MX";
                    break;
                case "PTR":
                    Send[Send.Length - 3] = 0xC;
                    Send[Send.Length - 4] = 0;
                    qtype = "PTR";
                    break;
                default:
                    Send[Send.Length - 3] = 1;
                    Send[Send.Length - 4] = 0;
                    break;

            }

        }
        public string[] SplitAddress(string webadress)
        {
            string[] addressparts = webadress.Split('.');
            return addressparts;
        }


        public int GetTotalLength(string[] array)
        {
            int counter = 0;
            for (int i = 0; i < array.Length; i++)
            {
                char[] help = array[i].ToArray();
                foreach (char c in help)
                {
                    counter += 1;
                }
            }
            counter += array.Length;
            return counter;
        }

    }
}

