using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleAppSample
{
    public class CoinChange
    {
        private List<int> Changes = new List<int>();
        private List<int> Coins = null;
        private bool IsChanged = false;

        public CoinChange(List<int> coins)
        {
            if (coins != null && coins.Count > 0)
            {
                coins.Sort();
                coins.Reverse();
                Coins = coins;
            }
        }

        public void Change(int val)
        {
            if (val == 0)
            {
                IsChanged = true;
                return;
            }
            else
            {
                IsChanged = false;
            }
            for (int i = 0; i < Coins.Count; i++)
            {
                if (Coins[i] <= val)
                {
                    val = val - Coins[i];
                    Changes.Add(Coins[i]);
                    Change(val);
                    break;
                }
            }
        }
    }

}
