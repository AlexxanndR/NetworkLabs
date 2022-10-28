using System;
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
            return String.Join(" ", binMessage);
        }
    }
}
