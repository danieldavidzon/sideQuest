using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sideQuest
{
    internal class sideQuestsList
    {
        private string Name;
        private string Explanation;
        private int XP;
        private int coin;
        public sideQuestsList(string name, string explanation, int xP, int coin)
        {
            this.Name = name;
            this.Explanation = explanation;
            this.XP = xP;
            this.coin = coin;
        }
        public string GetName()
        {
            return this.Name;
        }
        public void SetName(string name)
        {
            this.Name = name;
        }
        public string GetExplanation()
        {
            return this.Explanation;
        }
        public void SetExplanation(string explanation)
        {
            this.Explanation = explanation;
        }
        public int GetXP()
        {
            return this.XP;
        }
        public void SetXP(int xp)
        {
            this.XP = xp;
        }
        public int GetCoin() 
        { 
            return this.coin; 
        }
        public void SetCoin(int coin)
        {
            this.coin = coin;
        }
        }
    }
