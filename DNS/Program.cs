using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using System.Threading;//网络类的命名空间

using System.Text.RegularExpressions;//正则表达式

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

        static void Main(string[] args)
        {
            while(true){
                Console.WriteLine("请输入要解析的域名，若输入exit则结束本程序：");
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
                    string DNS = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(DNS))
                    {
                        Console.WriteLine("请先输入内容。");
                        continue;
                    }

                    if (!Regex.IsMatch(DNS, @"((25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))"))
                    {
                        Console.WriteLine("输入的IP地址有误。");
                        continue;
                    }

                    //解析
                    Console.WriteLine("本次解析已经完成，请按任意键继续...");
                    Console.ReadKey();
                    break;
                }
            }
        }

        /// <summary>
        /// 发送UDP数据包
        /// </summary>
        /// <param name="Data"></param>
        private static void SendMessage(Object Data)
        {
            //UdpSend.Send();
        }
    }
}
