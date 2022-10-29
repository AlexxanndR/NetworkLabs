using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM_Ports_CRC.Helpers
{
    public static class MessageHelper
    {
        public static string HexToBin(this string message)
        {
            var binMessage = Enumerable.Range(0, message.Length / 2)
                                       .Select(i => message.Substring(i * 2, 2))
                                       .Select(i => Convert.ToByte(i, 16))
                                       .Select(i => Convert.ToString(i, 2).PadLeft(8, '0'))
                                       .ToArray();
            return String.Join(String.Empty, binMessage);
        }

        public static string BinToHex(this string message)
        {
            var bytes = Enumerable.Range(0, message.Length % 8 == 0 ? message.Length / 8 : (message.Length / 8) + 1)
                                  .Select(i => Convert.ToByte(message.PadLeft(8, '0').Substring(i * 8, 8), 2))
                                  .ToArray();
            return BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();
        } 

        public static List<byte> HexToBitList(this string message)
        {
            var binMessage = Enumerable.Range(0, message.Length % 2 == 0 ? message.Length / 2 : (message.Length / 2) + 1)
                                       .Select(i => message.PadLeft(2, '0').Substring(i * 2, 2))
                                       .Select(i => Convert.ToByte(i, 16))
                                       .Select(i => Convert.ToString(i, 2).PadLeft(8, '0'))
                                       .ToArray();
            return String.Join(String.Empty, binMessage).Select(i => i == '1' ? (byte)1 : (byte)0).ToList();
        }
    }
}
