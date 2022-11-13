using COM_Ports_CRC.Helpers;
using System;
using System.Collections.Generic;

namespace COM_Ports_CRC.Core
{
    internal class CheckSum
    {
        public const string polynom16 = "A001";

        public static string CRC16(string message)
        {
            List<byte> polynom = polynom16.HexToBinList();
            List<byte> crc = message.HexToBinList();

            Int32 polynomDec = Convert.ToInt32(polynom16, 16);
            Int64 crcDec = Convert.ToInt64(message, 16);

            for (int i = 0; crcDec > polynomDec || crc.Count > polynom.Count; i = 0)
            {
                if (crc[i] == 0)
                    crc.RemoveAt(i); 
                else
                {
                    while (i < polynom.Count)
                    {
                        crc[i] = (byte)(crc[i] ^ polynom[i]);
                        i++;
                    }
                }
                crcDec = Convert.ToInt64(String.Join(String.Empty, crc), 2);
            }

            return String.Join(String.Empty, crc);
        }
    }
}
