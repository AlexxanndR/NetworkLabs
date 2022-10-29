using COM_Ports_CRC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM_Ports_CRC.Core
{
    internal class CheckSum
    {
        public static Dictionary<int, string> StandartPolynoms = new Dictionary<int, string>()
        {
            {4, "C"}, {8, "9B"}, {16, "A001"}
        };

        public static string CRC(string message, int degree)
        {
            if (degree % 2 != 0 && degree > 16)
                throw new ArgumentException("Polynom degree is incorrect.");

            List<byte> polynom = StandartPolynoms[degree].HexToBitList();
            List<byte> crc = message.HexToBitList();

            for (int i = 0; crc.Count > polynom.Count; i = 0)
            {
                if (crc[i] == 0) 
                {
                    crc.RemoveAt(i);
                    continue;
                }
                while (i < polynom.Count)
                {
                    crc[i] = (byte)(crc[i] ^ polynom[i]);
                    i++;
                }
            }


            return String.Join(String.Empty, crc);
        }
    }
}
