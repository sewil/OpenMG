﻿using System;
using WvsBeta.Common;
using WvsBeta.Common.Enums;
using WvsBeta.Common.Objects;
using WvsBeta.Game;
using WvsBeta.Game.Scripting;

namespace WvsBeta.Scripts
{
    internal class Easter : INpcScript
    {
        public void Run(IHost self, GameCharacter target, byte State, byte Answer, string StringAnswer, int IntegerAnswer)
        {
            if(target.Level < 8)
            {
                self.SendOK(" Sup. I'm Roy, but my boys call me 'The Mad Bunny'. I hate Easter and I'm going to bring it down this year. I don't think you can help us out right now, cuz you seem weak. Go get yourself stronger, and maybe we'll talk business. ");
                return;
            }
            var quests = target.Quests.Quests;
            int questID = 8876;
            if (!target.Quests.HasQuest(questID))
                target.Quests.AddNewQuest(questID, "uns");
            var quest = quests[questID];
            var inventory = target.Inventory;
            var cTime = MasterThread.CurrentDate;
            var esTime = cTime.CompareTo(new DateTime(cTime.Year, 3, 11, 0, 0, 0));
            var eeTime = cTime.CompareTo(new DateTime(cTime.Year, 4, 2, 23, 59, 59));
            if (esTime < 0)
            {
                self.SendOK("Quit buggin me. When Easter comes along, I'll have plenty for you to do...");
                self.Stop();
                return;
            }
            else if (eeTime > 0)
            {
                self.SendOK("Sup. Easter is over, but I still don't like it one bit, and now the people are aware of my plans. That's alright, though; my gang will do our best to bring it down next year, too. I'll be counting on you to help us take away more Easter Eggs from others. Peace..");
                self.Stop();
                return;
            }

            if (quest.Data == "end") // if a user already finished the quest
            {
                if (State == 0)
                {
                    self.AskYesNo("Hey, nice to see you again! How have you been? So, have you found more #b#t4031284##k for me?");
                    return;
                }
                else if (Answer == 0)
                {
                    self.SendOK("Oh really? That's alright. I'll be here for a bit, so if you find more #b#t4031284##k, you know where to find me.");
                }
                else
                {
                    self.SendOK("Sweet. That sounds like good news. I'll be here waiting.");
                    quests[questID].Data = "ing";
                }
                self.Stop();
            }
            else if (quest.Data == "ing")
            {
                if (inventory.GetItemAmount(Inventory.Etc, 4031284) >= 1)
                {
                    if (State == 0)
                    {
                        self.AskYesNo("Wow! You found the Golden Egg? That's the rarest of the Easter Eggs! What are you going to do with it? If you give it to me, I'll give you some EXP! Now, I'm not used to giving out EXP, so I can't guarantee how much you're going to get. But life's a gamble, right? Otherwise, I've got a deal with all the store owners, and they'll give you some good money for it. (These EXP points may level you up as much as 'almost' two levels, depending on your level.) ");
                        return;
                    }
                    else if (State == 1 && Answer != 0)
                    {
                        self.SendNext("Good deal! Hope you get some good EXP!");
                        return;
                    }
                    else if (State == 2)
                    {
                        inventory.TakeInventoryItem(4031284, 1);
                        var exp = 0;
                        var rn1 = Rand32.NextBetween(1, 10000);
                        if (1 <= rn1 && rn1 <= 5000) exp = 100;
                        if (5000 < rn1 && rn1 <= 8500) exp = 1000;
                        if (8500 < rn1 && rn1 <= 9999) exp = 10000;
                        if (9999 < rn1 && rn1 <= 10000) exp = 100000;
                        target.AddEXP(exp);
                        self.SendOK("I'm giving you " + exp + " EXP. Peace out!");
                        quests[questID].Data = "end";
                    }
                    self.Stop();
                }
                else
                {
                    self.SendOK("You don't have the Golden Egg!! Man... if you ever find one, then please see me, alright?");
                    self.Stop();
                }
            }
            else
            {
                if (State == 0)
                {
                    self.AskYesNo("I see, so does that mean you can help us out by giving us the Golden Egg?");
                    return;
                }
                else if (Answer != 0)
                {
                    //user accepts the quest
                    self.SendOK("Okay... Good luck!!!");
                    quests[questID].Data = "ing";
                }
                else //user doesn't accept the quest
                {
                    self.SendOK("You just wasted my breath. Get lost!");
                }
                self.Stop();
            }
        }
    }
}