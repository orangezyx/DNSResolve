using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;//正则表达式

using System.Net;
using System.Net.Sockets;//Socket

using System.Threading;//多线程

namespace DNS
{
    class Program
    {
        /// <summary>
        /// 用于发送UDP数据包的对象
        /// </summary>
        private static UdpClient UdpSend;

        /// <summary>
        /// 用于接收UDP数据包的对象
        /// </summary>
        private static UdpClient UdpRecv;

        /// <summary>
        /// 线程：不断监听UDP报文
        /// </summary>
        static Thread thrRecv;

        static CountdownEvent latch = new CountdownEvent(1);

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("请输入要解析的域名，若输入exit则结束本程序：");
                string Address = Console.ReadLine();
                if (Address == "exit")
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(Address))
                {
                    Console.WriteLine("请先输入内容。");
                    continue;
                }

                if (!Regex.IsMatch(Address, @"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+\.?"))
                {
                    Console.WriteLine("输入的域名有误。");
                    continue;
                }


                while (true)
                {
                    Console.Write("请输入DNS服务器地址：");
                    string DNSInput = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(DNSInput))
                    {
                        Console.WriteLine("请先输入内容。");
                        continue;
                    }

                    if (!Regex.IsMatch(DNSInput, @"((25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))"))
                    {
                        Console.WriteLine("输入的IP地址有误。");
                        continue;
                    }

                    //---解析开始---
                    UdpSend = new UdpClient(12345);
                    IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(DNSInput), 53); // 发送到的IP地址和端口号
                    byte[] DNSData = GenerateDNSPack(Address);
                    UdpSend.Send(DNSData, DNSData.Length, remoteIpep);//发送DNS报文
                    UdpSend.Close();//关闭
                    
                    IPEndPoint localIpepRcv = new IPEndPoint(IPAddress.Any, 12345); // 本机IP和监听端口号
                    UdpRecv = new UdpClient(localIpepRcv);
                    thrRecv = new Thread(ReceiveMessage);
                    thrRecv.Start();
                    latch.Wait();
                    //---解析结束---
                    break;
                }
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private static void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 12345);
            while (true)
            {
                try
                {
                    byte[] bytRecv = UdpRecv.Receive(ref remoteIpep);
                    //---------------
                    string pack = "";
                    foreach (byte tmp in bytRecv)
                    {
                        pack += tmp.ToString() + " ";
                    }
                    Console.WriteLine(pack); //输出接收的消息
                    //---------------
                    UdpRecv.Close();
                    Console.Write("本次解析已经完成，请按任意键继续...");
                    Console.Read();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.ToString());
                    break;
                }
            }

        }

        private static byte[] GenerateDNSPack(string HostName)
        {
            byte[] pack = new byte[] { 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0 };
            string[] Hosts = HostName.Split('.');
            for (int i = 0; i < Hosts.Length; i++)
            {
                pack = CombineBytes(pack,new byte[] { Convert.ToByte(Hosts[i].Length) });
                pack = CombineBytes(pack, Encoding.ASCII.GetBytes(Hosts[i]));
            }
            pack = CombineBytes(pack, new byte[] { 0, 0, 1, 0, 1 });
            return pack;
        }

        private static byte[] CombineBytes(byte[] source,byte[] newbytes)
        {
            byte[] c = new byte[source.Length + newbytes.Length];
            source.CopyTo(c, 0);
            newbytes.CopyTo(c, source.Length);
            return c;
        }
    }
}