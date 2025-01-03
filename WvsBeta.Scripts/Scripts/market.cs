﻿using WvsBeta.Common;
using WvsBeta.Game;
using WvsBeta.Game.Packets;
using WvsBeta.Game.Scripting;

namespace WvsBeta.Scripts.Scripts
{
    class Market
    {
        public static readonly short QUEST_ID = 7600;
    }
    [Script("market00")]
    class Market00 : Market, IPortalScript
    {
        public void Run(IPortalHost host, GameCharacter target)
        {
            var qr = target.Quests;
            var val = qr.Get(QUEST_ID);

            target.PlayPortalSE();

            if (val == "0") target.ChangeMap(100000100, "st00");
            else if (val == "1") target.ChangeMap(220000000, "st00");
            else if (val == "2") target.ChangeMap(211000100, "st00");
            else if (val == "4") target.ChangeMap(230000000, "st00");
            else target.ChangeMap(102000000, "st00");
        }
    }
    // Henesys Market
    [Script("market01")]
    class Market01 : Market, IPortalScript
    {
        public void Run(IPortalHost host, GameCharacter target)
        {
            var qr = target.Quests;
            qr.Set(QUEST_ID, "0");
            target.PlayPortalSE();
            target.ChangeMap(910000000, "st00");
        }
    }
    // Ludibrium
    [Script("market02")]
    class Market02 : Market, IPortalScript
    {
        public void Run(IPortalHost host, GameCharacter target)
        {
            var qr = target.Quests;
            qr.Set(QUEST_ID, "1");
            target.PlayPortalSE();
            target.ChangeMap(910000000, "st00");
        }
    }
    // El Nath Market
    [Script("market03")]
    class Market03 : Market, IPortalScript
    {
        public void Run(IPortalHost host, GameCharacter target)
        {
            var qr = target.Quests;
            qr.Set(QUEST_ID, "2");
            target.PlayPortalSE();
            target.ChangeMap(910000000, "st00");
        }
    }
    // Perion
    [Script("market04")]
    class Market04 : Market, IPortalScript
    {
        public void Run(IPortalHost host, GameCharacter target)
        {
            var qr = target.Quests;
            qr.Set(QUEST_ID, "3");
            target.PlayPortalSE();
            target.ChangeMap(910000000, "st00");
        }
    }
    // Aquarium
    [Script("market05")]
    class Market05 : Market, IPortalScript
    {
        public void Run(IPortalHost host, GameCharacter target)
        {
            var qr = target.Quests;
            qr.Set(QUEST_ID, "4");
            target.PlayPortalSE();
            target.ChangeMap(910000000, "st00");
        }
    }
}
