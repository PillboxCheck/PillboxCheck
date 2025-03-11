using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pillbox
{
    public static class Utils
    {
        public static int getHeightText(Label lbl)
        {
            using (Graphics g = lbl.CreateGraphics())
            {
                SizeF size = g.MeasureString(lbl.Text, lbl.Font,lbl.Width);
                return (int)Math.Ceiling(size.Height);
            }
        }

        public static byte[] ReceiveAll(NetworkStream networkStream)
        {
            var buffer = new List<byte>();
            var tempBuffer = new byte[1];

            while (networkStream.DataAvailable)
            {
                int bytesRead = networkStream.Read(tempBuffer, 0, tempBuffer.Length);

                if (bytesRead > 0)
                {
                    buffer.Add(tempBuffer[0]);
                }
            }

            return buffer.ToArray();
        }

    }

}
