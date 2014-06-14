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
        #region declarations
        //Connection
        public Socket socket;
        IPAddress dnsserver;
        IPEndPoint dnsendpoint;

        //Arrays for in and output
        byte[] Send;
        byte[] rebuffer;

        //Events
        event Receive OnReceive;
        public event Output ShowOutput;

        //Global variables
        string qtype;
        string host;
        string answer;
        List<string> answers;
        string ipversion;
        int length;
        string[] domains = { ".com", ".de", ".net" ,".org"};
        #endregion
        public DNS(string dnsip)
        {
            //Initialize declarations
            answers = new List<string>();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            
            dnsserver = IPAddress.Parse(dnsip);
            dnsendpoint = new IPEndPoint(dnsserver, 53);
            //Events
            OnReceive += DNS_OnReceive;
        }

        //=================================
        //Main methods
        //=================================

        /// <summary>
        /// Get and show the IPv4 of the host.
        /// </summary>
        public void GetIPv4(string host)
        {
            //Remove if it starts with "www."
            if (host.StartsWith("www."))
            {
                host = host.Remove(0, 4);
            }
            this.host = host;
            //Split into parts
            string[] addressparts = SplitAddress(host);
            Send = new byte[17 + GetTotalLength(addressparts)/*Frame data + length of host*/];
            //Write the host to the array
            for (int i = 12; i < Send.Length; )
            {
                int k = 13;
                for (int j = 0; j < addressparts.Length; j++)
                {
                    //Split the string of the addressparts to a char array
                    char[] help = addressparts[j].ToArray();
                    if (i < Send.Length)
                    {
                        Send[i] = Convert.ToByte(addressparts[j].Length);//Length of coming label
                        for (int l = 0; k < Send.Length && l < help.Length; k++, l++)
                        {
                            Send[k] = Convert.ToByte(help[l]);
                        }
                        //Set length label to begin of the next part
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
            //Set frame data
            SetFrame();
            SetType("A");
            SendToDNS();
        }

        /// <summary>
        /// Get and show the IPv6 of the host.
        /// </summary>
        public void GetIPv6(string host)
        {
            //Remove if it starts with "www."
            if (host.StartsWith("www."))
            {
                host = host.Remove(0, 4);
            }
            this.host = host;
            //Split into parts
            string[] addressparts = SplitAddress(host);
            Send = new byte[17 + GetTotalLength(addressparts)/*Frame data + length of host*/];
            //Write the host to the array
            for (int i = 12; i < Send.Length; )
            {
                int k = 13;
                for (int j = 0; j < addressparts.Length; j++)
                {
                    //Split the string of the addressparts to a char array
                    char[] help = addressparts[j].ToArray();
                    if (i < Send.Length)
                    {
                        Send[i] = Convert.ToByte(addressparts[j].Length);//Length of coming label
                        for (int l = 0; k < Send.Length && l < help.Length; k++, l++)
                        {
                            Send[k] = Convert.ToByte(help[l]);
                        }
                        //Set length label to begin of the next part
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
            //Set frame data
            SetFrame();
            SetType("AAAA");
            SendToDNS();
        }

        /// <summary>
        /// Get and show the address and IPv4 of the mailserver.
        /// </summary>
        public void GetMailServer(string host)
        {
            //Get the mailserver
            MailAddress server = new MailAddress(host);
            host = server.Host;

            this.host = host;
            Send = new byte[18 + host.Length/*Frame data + length of host*/];
            //Write the host to the array
            for (int i = 0; i < host.Length; i++)
            {
                if (host[i] == '.')
                {
                    //Writes length label + first char
                    Send[12] = Convert.ToByte(i);
                    Send[i + 13] = Convert.ToByte(host.Length - i - 1);
                }
                else
                {
                    //Writes char
                    Send[i + 13] = Convert.ToByte(host[i]);
                }
            }
            //Set frame data
            SetFrame();
            SetType("MX");
            SendToDNS();
            //Get the IPv4 of the mailserver
            string[] temp = host.Split('.');
            if(!answer.Contains(temp[0]))
            {
                GetIPv4(answer+'.'+host);
            }
            else
            {
                GetIPv4(answer);
            }
        }

        /// <summary>
        /// Get and show the webaddress of the host.
        /// </summary>
        public void GetWebaddress(string host)
        {
            this.host = host;
            //Split into parts
            string[] addressparts = SplitAddress(host);
            if (host.Contains("."))
            {
                ipversion = "v4";
                Send = new byte[30 + GetTotalLength(addressparts)/*Frame data + length of host*/];
                //Write the host to the array in reversed order
                for (int i = 12; i < Send.Length; )
                {
                    int k = 13;
                    for (int j = addressparts.Length - 1; j >= 0; j--)
                    {
                        //Split the string of the addressparts to a char array
                        char[] help = addressparts[j].ToArray();
                        if (i < Send.Length)
                        {
                            Send[i] = Convert.ToByte(addressparts[j].Length);//Length of coming label
                            for (int l = 0; k < Send.Length && l < help.Length; k++, l++)
                            {
                                Send[k] = Convert.ToByte(help[l]);
                            }
                            //Set length label to begin of the next part
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
                //Frame data for a PTR request (all IP's should end to "in-addr.arpa")
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
            }
            else if (host.Contains(":"))
            {
                ipversion = "v6";
                length = GetTotalLength(addressparts);
                Send = new byte[26 + GetTotalLength(addressparts)/*Frame data + length of host*/];
                for (int i = 12; i < Send.Length; )
                {
                    int k = 13;
                    for (int j = addressparts.Length - 1; j >= 0; j--)
                    {
                        //Split the string of the addressparts to a char array
                        char[] help = addressparts[j].ToArray();
                        if (i < Send.Length)
                        {
                            Send[i] = Convert.ToByte(addressparts[j].Length);//Length of coming label
                            for (int l = 0; k < Send.Length && l < help.Length; k++, l++)
                            {
                                Send[k] = Convert.ToByte(help[l]);
                            }
                            //Set length label to begin of the next part
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
                //Frame data for a PTR request (all IP's should end to "ip6.arpa")
                Send[Send.Length - 14] = 3;
                Send[Send.Length - 13] = Convert.ToByte('i');
                Send[Send.Length - 12] = Convert.ToByte('p');
                Send[Send.Length - 11] = Convert.ToByte('6');
                Send[Send.Length - 10] = 4;
                Send[Send.Length - 9] = Convert.ToByte('a');
                Send[Send.Length - 8] = Convert.ToByte('r');
                Send[Send.Length - 7] = Convert.ToByte('p');
                Send[Send.Length - 6] = Convert.ToByte('a');
            }
            //Set frame data
            SetFrame();
            SetType("PTR");
            SendToDNS();
        }

        //=================================
        //Private methods
        //=================================

        /// <summary>
        /// Sends the bufferarray to the DNS server
        /// </summary>
        private void SendToDNS()
        {
            //Connect to DNS
            socket.Connect(dnsendpoint);
            try
            {
                //Send and receive
                socket.SendTo(Send, dnsendpoint);
                rebuffer = new byte[170];
                socket.Receive(rebuffer, SocketFlags.None);
            }
            catch
            {
                throw new Exception();
            }
            //Give receive array to event
            OnReceive(rebuffer);
        }

        /// <summary>
        /// Sets the frame of the buffer array.
        /// </summary>
        private void SetFrame()
        {
            //ID
            Send[0] = Convert.ToByte('x');
            Send[1] = Convert.ToByte('y');
            //Status
            Send[2] = 1;
            Send[3] = 0;
            //Number of questions in request
            Send[4] = 0;
            Send[5] = 1;

            for (int i = 6; i < 12; i++)
            {
                Send[i] = 0;
            }
            //QCLASS = 0x0001 for internet
            Send[Send.Length - 1] = 1;
            Send[Send.Length - 2] = 0;
        }

        /// <summary>
        /// Set the type of the query.
        /// </summary>
        private void SetType(string shortcut)
        {
            switch (shortcut)
            {
                case "A":
                    //A = 1
                    Send[Send.Length - 3] = 1;
                    Send[Send.Length - 4] = 0;
                    qtype = "A";
                    break;
                case "AAAA":
                    //AAAA = 28
                    Send[Send.Length - 3] = 28;
                    Send[Send.Length - 4] = 0;
                    qtype = "AAAA";
                    break;
                case "MX":
                    //MX = 15
                    Send[Send.Length - 3] = 0xF;
                    Send[Send.Length - 4] = 0;
                    qtype = "MX";
                    break;
                case "PTR":
                    //PTR = 12
                    Send[Send.Length - 3] = 0xC;
                    Send[Send.Length - 4] = 0;
                    qtype = "PTR";
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Splits the webaddress at '.' or ':' and returns the string array.
        /// </summary>
        private string[] SplitAddress(string webaddress)
        {
            string[] addressparts = new string[webaddress.Length];
            //IPv4
            if (webaddress.Contains('.'))
            {
                 addressparts = webaddress.Split('.');
            }
            //IPv6
            else if(webaddress.Contains(':'))
            {
                List<string> stringlist = new List<string>();
                addressparts = webaddress.Split(':');
                foreach(string s in addressparts)
                {
                    char[] help = s.ToArray();
                    for (int i=0;i<help.Length;i++)
                    {
                        stringlist.Add(Convert.ToString(help[i]));
                    }
                }
                addressparts = stringlist.ToArray();
            }
            return addressparts;
        }

        /// <summary>
        /// Gets the total length of the array including the '.'.
        /// </summary>
        private int GetTotalLength(string[] array)
        {
            //Counter for the amount of chars
            int counter = 0;
            for (int i = 0; i < array.Length; i++)
            {
                char[] help = array[i].ToArray();
                foreach (char c in help)
                {
                    
                    counter += 1;
                }
            }
            //Adds the '.'
            counter += array.Length;
            return counter;
        }

        /// <summary>
        /// Handles all incoming messages
        /// </summary>
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
                            answers.Add("IPv4: " + answer.TrimEnd('.'));
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
                    answer = "";
                    //Set startcounter to offset
                    startcounter = 30 + host.Length;
                    for (int i = startcounter; i <= startcounter+16/*length of IPv6 address*/; i++)
                    {
                        //Inserts leading 0
                        if (receive[i] < 16)
                        {
                            answer += "0" + receive[i].ToString("X");
                        }
                        else
                        {
                            answer += receive[i].ToString("X");
                        }
                    }
                    //Insert ':'
                    string answernew = "";
                    for (int j = 1,k=0; j < answer.Length&&k<answer.Length; j++,k++)
                    {
                        if(j%4==0)
                        {
                            answernew += answer[k] + ":";
                        }
                        else
                        {
                            answernew += answer[k];
                        }
                    }
                    //Remove spare symbols
                    answernew = answernew.Remove(answernew.Length - 2, 2);
                    ShowOutput("IPv6: " + answernew);
                    break;
                case "MX":
                    answer = "";
                    //Set startcounter to offset
                    startcounter = host.Length + 33;
                    for (int i = startcounter; i < receive.Length; i++)
                    {
                        if (receive[i] != 0)
                        {
                            //Dont read empty bytes
                            if (receive[i] < 20)
                            {
                                answer += '.';
                            }
                            else if (receive[i] == 192)
                            {
                                break;
                            }
                            else
                            {
                                answer += Convert.ToChar(receive[i]);
                            }
                        }
                    }
                    ShowOutput("Address: " + answer);
                    break;
                case "PTR":
                    answer = "";
                    //Set startcounter to offset depending on used IP version
                    if (ipversion == "v4")
                    {
                        startcounter = host.Length + 44;//in-addr.arpa
                    }
                    else if (ipversion == "v6")
                    {
                        startcounter = length + 13 + 22 + 8;//ip6.arpa
                    }
                    for (int i = startcounter; i < receive.Length; i++)
                    {
                        //Dont read empty bytes
                        if (receive[i] != 0)
                        {
                            //Sort out counter to labels
                            if (receive[i] < 20)
                            {
                                answer += '.';
                            }
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
    }
}

