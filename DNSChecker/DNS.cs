using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DNSChecker
{
    class DNS
    {
        Socket socket;
        IPAddress dnsserver;
        IPEndPoint dnsendpoint;
        byte[] Send;
        public DNS()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 100000);
            dnsserver = IPAddress.Parse("8.8.8.8");
            dnsendpoint = new IPEndPoint(dnsserver, 53);
            GetIPv4("www.google.de");
        }
        public void GetIPv4(string webaddress)
        {
            string[] addressparts = SplitAddress(webaddress);
            Send = new byte[13 + GetTotalLength(addressparts) + 4];
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
            Send[Send.Length - 1] = 1;
            Send[Send.Length - 2] = 0;
            Send[Send.Length - 3] = 1;
            Send[Send.Length - 4] = 0;
            socket.Connect(dnsendpoint);
            socket.SendTo(Send, dnsendpoint);
            byte[] rebuffer = new byte[170];

            socket.Receive(rebuffer, SocketFlags.None);
            string answer = "";
            foreach (Byte b in rebuffer)
            {
                answer += Convert.ToChar(b);
            }
            Console.WriteLine(answer);

            Console.ReadLine();
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

