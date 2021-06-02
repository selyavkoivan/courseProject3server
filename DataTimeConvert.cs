using System;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    static class MyConvert
    {
        public static DateTime getDate(Socket socket)
        {

            byte[] bytes = new byte[128];
            int size = socket.Receive(bytes);
            string receiveStr = Encoding.Unicode.GetString(bytes, 0, size);
            string[] strings = receiveStr.Split('\t');
            DateTime date = new DateTime(Convert.ToInt32(strings[0]), Convert.ToInt32(strings[1]), Convert.ToInt32(strings[2]));
            return date;
        }

        static public void sendDate(DateTime date, Socket socket)
        {
            string sendStr = date.Year + "\t" + date.Month + "\t" + date.Day;
            socket.Send(Encoding.Unicode.GetBytes(sendStr));
        }
        public static TimeSpan[] getTime(Socket socket)
        {

            byte[] bytes = new byte[256];
            int size = socket.Receive(bytes);
            string receiveStr = Encoding.Unicode.GetString(bytes, 0, size);
            string[] strings = receiveStr.Split('\t');
            
            TimeSpan start = new TimeSpan(Convert.ToInt32(strings[0]), Convert.ToInt32(strings[1]), 0);
            TimeSpan end = new TimeSpan(Convert.ToInt32(strings[2]), Convert.ToInt32(strings[3]), 0);
            TimeSpan[] time = { start, end };
           
            return time;
        }

        static public void sendTime(TimeSpan start, TimeSpan end, Socket socket)
        {
            string sendStr = start.Hours + "\t" + start.Minutes + "\t" + end.Hours + "\t" + end.Minutes;
            socket.Send(Encoding.Unicode.GetBytes(sendStr));
        }

    }
}
