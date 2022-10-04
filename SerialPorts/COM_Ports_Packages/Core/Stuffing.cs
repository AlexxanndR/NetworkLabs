using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM_Ports_Packages.Core
{
    internal static class Stuffing
    {
        private static string GetMaxSubsequence(string flag)
        {
            var repetingBits = new string(flag.Select((c, index) => flag.Substring(index).TakeWhile((x, i) => x == c && i < 5))
                                              .OrderByDescending(x => x.Count())
                                              .First()
                                              .ToArray());
            return flag.Substring(0, flag.IndexOf(repetingBits) + repetingBits.Length);
        }

        public static string BitStuffing(string package)
        {
            var binaryPackage = Enumerable.Range(0, package.Length / 2)
                                          .Select(i => package.Substring(i * 2, 2))
                                          .Select(i => Convert.ToByte(i, 16))
                                          .Select(i => Convert.ToString(i, 2).PadLeft(8, '0'))
                                          .ToArray();

            string binaryFlag    = binaryPackage[0];
            string maxSubsequnce = GetMaxSubsequence(binaryFlag);

            for (var field = 1; field < binaryPackage.Length; field++)
            {
                if (binaryPackage[field].Contains(maxSubsequnce))
                {
                    int startIndex   = binaryPackage[field].IndexOf(maxSubsequnce);
                    char additionBit = maxSubsequnce[maxSubsequnce.Length - 1] == '0' ? '1' : '0';
                    binaryPackage[field] = binaryPackage[field].Substring(0, startIndex + maxSubsequnce.Length) + additionBit + binaryPackage[field].Substring(startIndex + maxSubsequnce.Length);
                }
            }

            return String.Join(String.Empty, binaryPackage);
        }

        public static string BitDestuffing(string package)
        {
            string binaryFlag    = package.Substring(0, 8);
            string maxSubsequnce = GetMaxSubsequence(binaryFlag);

            for (int i = 8; i < package.Length - maxSubsequnce.Length + 1; i += 8)
            {
                var selectedByte = package.Substring(i, 8);
                if (selectedByte.Contains(maxSubsequnce))
                    package = package.Remove(i + selectedByte.IndexOf(maxSubsequnce) + maxSubsequnce.Length, 1);
            }

            var hexPackage = Enumerable.Range(0, package.Length / 8)
                                       .Select(i => package.Substring(i * 8, 8))
                                       .Select(i => Convert.ToByte(i, 2))
                                       .ToArray();

            return BitConverter.ToString(hexPackage).Replace("-", String.Empty).ToLower();
        }
    }
}
