using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppSample
{
    public static class SortAlgorithm
    {
        public static void BubbleSort()
        {
            int[] iAry = { 64, 34, 25, 12, 22, 11, 90 };
            Console.WriteLine($"Before BubbleSort [{string.Join(',', iAry)}]");

            bool swapped = false;
            for (int i = 0; i < iAry.Length - 1; i++)
            {
                for (int j = 0; j < iAry.Length - i - 1; j++)
                {
                    if (iAry[j] > iAry[j + 1])
                    {
                        iAry[j] = iAry[j] + iAry[j + 1];
                        iAry[j + 1] = iAry[j] - iAry[j + 1];
                        iAry[j] = iAry[j] - iAry[j + 1];
                        swapped = true;
                    }
                }
                if (swapped == false)
                    break;
            }

            Console.WriteLine($"After BubbleSort [{string.Join(',', iAry)}]");
        }

        public static void SelectionSort()
        {
            int[] iAry = { 64, 34, 25, 12, 22, 11, 90 };

            Console.WriteLine($"Before SelectionSort [{string.Join(',', iAry)}]");
            for (int i = 0; i < iAry.Length - 1; i++)
            {
                int min = i;
                for (int j = i + 1; j < iAry.Length; j++)
                {
                    if (iAry[j] < iAry[min])
                    {
                        min = j;
                    }
                }

                if (min != i)
                {
                    iAry[min] = iAry[min] + iAry[i];
                    iAry[i] = iAry[min] - iAry[i];
                    iAry[min] = iAry[min] - iAry[i];
                }
            }

            Console.WriteLine($"After BubbleSort [{string.Join(',', iAry)}]");
        }
    }
}
