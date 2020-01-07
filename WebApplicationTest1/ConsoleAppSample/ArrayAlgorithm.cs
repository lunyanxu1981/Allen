using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConsoleAppSample
{
    public static class ArrayAlgorithm
    {
        /// <summary>
        /// Greatest sum of non joining element value in an integer array
        /// </summary>
        public static void GreatestSumOfNonAdjacentElements(int[] iAry)
        {
            SumResult result = CalcSumRecursive(iAry, iAry.Length - 1);

            StringBuilder strResult = new StringBuilder();

            foreach (int idx in result.Indices)
            {
                strResult.Append($"Array[{idx}]:={iAry[idx]} +");
            }

            strResult.Remove(strResult.ToString().LastIndexOf('+'), 1);

            Console.WriteLine($"{strResult.ToString()}= {result.Sum}");
        }

        private static SumResult CalcSumRecursive(int[] iAry, int idx)
        {
            if (idx == 0)
            {
                return new SumResult(iAry[idx], idx);
            }
            else if (idx == 1)
            {
                return iAry[idx] >= iAry[idx - 1] ? new SumResult(iAry[idx], idx) : new SumResult(iAry[idx - 1], -1);
            }
            else
            {
                SumResult aResult = CalcSumRecursive(iAry, idx - 2).CompareNum(iAry[idx], idx);
                SumResult bResult = CalcSumRecursive(iAry, idx - 1);
                return aResult.Sum >= bResult.Sum ? aResult : bResult;
            }
        }

        /// <summary>
        /// The greatest sum of (contigous sub-array) in an integer array
        /// </summary>
        /// <param name="iAry">The integer array</param>
        /// <returns></returns>
        public static int GreatestSum(int[] iAry)
        {
            int max_so_far = iAry[0];
            int curr_max = iAry[0];

            for (int i = 1; i < iAry.Length; i++)
            {
                curr_max = Math.Max(iAry[i], curr_max + iAry[i]);
                max_so_far = Math.Max(max_so_far, curr_max);
            }

            Console.WriteLine($"The greatest sum of Array [{string.Join(',', iAry)}] is: {max_so_far}");
            return max_so_far;
        }

        public static void GreatestSumOfNElementsRecur(int[] iAry, int Max)
        {
            int result = Int32.MinValue;
            int maxIdx = Int32.MinValue;
            for (int i = 0; i <= iAry.Length - Max; i++)
            {
                int tempResult = iAry.Skip(i).Take(Max).Sum();
                if (tempResult > result)
                {
                    result = tempResult;
                    maxIdx = i;
                }
            }
            if (result > Int32.MinValue)
            {
                while (Max > 0)
                {
                    Console.Write($"Index:{maxIdx} ");
                    Max--;
                    maxIdx++;
                }
                Console.WriteLine($"Max Sum:{result}");
            }
            else
            {
                Console.WriteLine("No Answer!");
            }
        }

        /// <summary>
        /// The greatest sum of (contigous N-element sub-array) in an integer array (Recursive)
        /// </summary>
        /// <param name="iAry"></param>
        /// <param name="curIdx"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        public static int GreatestSumOfNElementsRecur(int[] iAry, int curIdx, int Max)
        {
            if ((curIdx + Max) > iAry.Length)
                return Int32.MinValue;
            if ((curIdx + Max) == iAry.Length)
            {
                return iAry.Skip(curIdx).Take(Max).Sum();
            }
            else
            {
                return Math.Max(iAry.Skip(curIdx).Take(Max).Sum(),
                    GreatestSumOfNElementsRecur(iAry, curIdx + 1, Max));
            }
        }


        public static void LongestCommonSequence(string str1 = "asassaabcdssxxbsscdedd", string str2 = "faabcdsbcdeff")
        {
            string result = string.Empty;            

            Tuple<string, string> compare(string s1, string s2)
            {
                return (s1.Length > s2.Length) ? Tuple.Create(s2, s1) : Tuple.Create(s1, str2);
            }

            (string sourceStr, string targetStr) = compare(str1, str2);

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
                if (tempResult.Length > result.Length)
                {
                    result = tempResult;
                }
            }
            Console.WriteLine($"Source String: {sourceStr} Target String: {targetStr} Longest Common Sequence: {result}");
        }

        /// <summary>
        /// Find longest common substring
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        public static void LongestCommonSubstring(string str1 = "aaaabcdssbcdedd", string str2 = "faabcdsbcdeff")
        {
            string result = string.Empty;

            Tuple<string, string> compare(string s1, string s2)
            {
                return (s1.Length > s2.Length) ? Tuple.Create(s2, s1) : Tuple.Create(s1, str2);
            }

            (string sourceStr, string targetStr) = compare(str1, str2);

            for (int i = 0; i < sourceStr.Length; i++)
            {
                int offset = 1;
                while ((offset + i) <= sourceStr.Length)
                {
                    string subStr = sourceStr.Substring(i, offset);
                    if (targetStr.IndexOf(subStr) >= 0)
                    {
                        if (result.Length <= subStr.Length)
                        {
                            result = subStr;
                        }
                        offset++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            Console.WriteLine($"Source String: {sourceStr} Target String: {targetStr} Longest Common Substring: {result}");

        }
    }

    public class SumResult
    {
        public int Sum = Int32.MinValue;
        public List<int> Indices = new List<int>();

        public SumResult(int num, int numIdx)
        {
            CompareNum(num, numIdx);
        }

        public SumResult CompareNum(int num, int numIdx)
        {
            if ((Sum + num) > Sum && !Indices.Exists(idx => (idx == numIdx || Math.Abs(numIdx - idx) == 1)))
            {
                Indices.Add(numIdx);
                Sum = Sum.Equals(Int32.MinValue) ? num : Sum + num;
            }

            return this;
        }
    }
}
