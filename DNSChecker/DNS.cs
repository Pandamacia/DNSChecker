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
        public DNS()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 100000);
            dnsserver = IPAddress.Parse("8.8.8.8");
            dnsendpoint = new IPEndPoint(dnsserver, 53);
            GetIP("www.google.de");
        }
        public void GetIP(string webaddress)
        {

            byte[] Send = new byte[13 + webaddress.Length+5];
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
            Send[12] = Convert.ToByte(webaddress.Length);
            char[] charaddress = webaddress.ToArray();
            List<byte> toconv = new List<byte>();
            foreach (Char c in charaddress)
            {
                toconv.Add(Convert.ToByte(c));
            }
            byte[] conv_address = toconv.ToArray();
            for (int i = 13, j = 0; i < Send.Length && j < conv_address.Length; i++, j++)
            {
                Send[i] = conv_address[j];
            }
            Send[Send.Length - 1] = 1;
            Send[Send.Length - 2] = 0;
            Send[Send.Length - 3] = 0x1;
            Send[Send.Length - 4] = 0;
            Send[Send.Length - 5] = 0;
            socket.Connect(dnsendpoint);
            socket.SendTo(Send, dnsendpoint);
            byte[] rebuffer = new byte[170];
            //List<byte> rebuffer = new List<byte>();

            socket.Receive(rebuffer, SocketFlags.None);
            string answer = "";
            foreach (Byte b in rebuffer)
            {
                answer += Convert.ToChar(b);
            }
            Console.WriteLine(answer);

            Console.ReadLine();
        }


    }
}
