using System;
using System.Linq;
namespace ConsoleAlgorithm
{
    public static class ArrayAlgorithm
    {
        public static void GreatestSumOfContiguousElementsInArray()
        {
            int[] iAry = { 2, 3, -7, 9, -9, 1, 6 };
            int maxSofar, max;
            max = maxSofar = iAry[0];
            for (int i = 1; i < iAry.Length; i++)
            {
                max = Math.Max(iAry[i], iAry[i] + max);

                maxSofar = Math.Max(max, maxSofar);
            }
            Console.WriteLine($"Array:[{String.Join(',', iAry)}]");
            Console.WriteLine($"The greatest sum of contiguous elements is: {maxSofar}");

        }

        public static int GreatestSumOfNonContiguousElementsInArray(int[] iAry, int idx)
        {
            if (idx == 0)
            {
                return iAry[idx];
            }
            else if (idx == 1)
            {
                return Math.Max(iAry[idx], iAry[idx - 1]);
            }
            else
            {
                int a = Math.Max(GreatestSumOfNonContiguousElementsInArray(iAry, idx - 2) + iAry[idx], iAry[idx]);
                int b = GreatestSumOfNonContiguousElementsInArray(iAry, idx - 1);
                return Math.Max(a, b);
            }

        }

        public static int SmallestSumOfNonContiguousElementsInArray(int[] iAry, int idx)
        {
            if (idx == 0)
            {
                return iAry[idx];
            }
            else if (idx == 1)
            {
                return Math.Min(iAry[idx], iAry[idx - 1]);
            }
            else
            {
                int a = Math.Min(SmallestSumOfNonContiguousElementsInArray(iAry, idx - 2) + iAry[idx], iAry[idx]);
                int b = SmallestSumOfNonContiguousElementsInArray(iAry, idx - 1);
                return Math.Min(a, b);
            }

        }

        public static string LongestCommmonString(string sourceStr, string targetStr)
        {
            string result = string.Empty;
            for (int i = 0; i < sourceStr.Length; i++)
            {
                int offset = 1;

                while ((i + offset) <= sourceStr.Length)
                {
                    string tempResult = sourceStr.Substring(i, offset);
                    if (targetStr.IndexOf(tempResult) >= 0)
                    {
                        offset++;
                        if (result.Length <= tempResult.Length)
                        {
                            result = tempResult;
                        }
                    }
                    else
                        break;
                }
            }
            return result;
        }

        public static string LongestCommonSequence(string sourceStr, string targetStr)
        {
            string result = string.Empty;
            for (int i = 0; i < sourceStr.Length; i++)
            {
                string tempResult = string.Empty;
                string tempTarget = targetStr;
                for (int j = i; j < sourceStr.Length && tempTarget.Length > 0; j++)
                {
                    if (tempTarget.IndexOf(sourceStr[j]) >= 0)
                    {
                        tempResult += sourceStr[j];
                        tempTarget = tempTarget.Substring(tempTarget.IndexOf(sourceStr[j]) + 1);
                    }
                }
                if(tempResult.Length > result.Length)
                {
                    result = tempResult;
                }
            }
            return result;
        }
    }
}