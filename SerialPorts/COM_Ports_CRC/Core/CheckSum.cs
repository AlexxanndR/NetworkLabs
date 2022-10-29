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
            {4, "0C"}, {8, "AB"}, {16, "A001"}
        };

        public static string CRC(string message, int degree)
        {
            if (degree % 2 != 0 && degree > 16)
                throw new ArgumentException("Polynom degree is incorrect.");

            List<byte> polynom = StandartPolynoms[degree].HexToBitList();
            List<byte> crc = message.HexToBitList();

            while (crc.Count > polynom.Count)
            {
                for (int i = 0; i < crc.Count && i < polynom.Count; i++)
                {
                    if (crc[i] == 0)
                        crc.RemoveAt(i);
                    else
                        crc[i] = (byte)(crc[i] ^ polynom[i]);              
                }
            }

            return String.Join(String.Empty, crc);
        }
    }
}
