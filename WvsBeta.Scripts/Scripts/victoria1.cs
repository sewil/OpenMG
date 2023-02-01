﻿using System;
using WvsBeta.Common;
using WvsBeta.Common.Enums;
using WvsBeta.Common.Extensions;
using WvsBeta.Game;
using WvsBeta.Game.Scripting;

namespace WvsBeta.Scripts.Scripts
{
    // a pile of flowers
    [Script("bush1")]
    class Bush1 : INpcScript
    {
        public void Run(INpcHost self, GameCharacter target, byte state, byte nRet, string stringAnswer, int nRetNum)
        {
            var qr = target.Quests;
            var val = qr.GetState(2050);

            var inventory = target.Inventory;

            if (target.Level > 24)
            {
                if (val == 1)
                {
                    if (state == 0)
                    {
                        self.AskYesNo("Are you sure you want to take #b#t4031020##k with you?");
                        return;
                    }
                    else if (nRet != 0)
                    {
                        if (!target.TryExchange(0, 4031020, 1)) self.SendNext("Your etc inventory seems to be full. Please make some room to get the item.");
                        else target.ChangeMap(101000000);
                    }
                }
                else if (val == 2)
                {
                    if (inventory.GetOpenSlotsInInventory(Inventory.Etc) >= 1)
                    {
                        var nNewItemID = 0;
                        var rnum = Rand32.NextBetween(0, 12);
                        if (rnum == 0) nNewItemID = 4010000;
                        else if (rnum == 1) nNewItemID = 4010001;
                        else if (rnum == 2) nNewItemID = 4010002;
                        else if (rnum == 3) nNewItemID = 4010003;
                        else if (rnum == 4) nNewItemID = 4010004;
                        else if (rnum == 5) nNewItemID = 4010005;
                        else if (rnum == 6) nNewItemID = 4020000;
                        else if (rnum == 7) nNewItemID = 4020001;
                        else if (rnum == 8) nNewItemID = 4020002;
                        else if (rnum == 9) nNewItemID = 4020003;
                        else if (rnum == 10) nNewItemID = 4020004;
                        else if (rnum == 11) nNewItemID = 4020005;
                        else if (rnum == 12) nNewItemID = 4020006;

                        var ret = target.TryExchange(0, nNewItemID, 2);
                        if (!ret) self.SendNext("Your etc inventory seems to be full. Please make some room to take the item.");
                        else target.ChangeMap(101000000);
                    }
                    else self.SendNext("You must have at least one empty slot in your etc inventory to store the item you found among the flowers. Free up some space and then try again.");
                }
                else self.SendNext("In the middle of all these flowers you will find some with a mysterious aura around them, but you won't be able to pick them because you still haven't heard the explanation from #b#p1061005##k about which ones to pick...");
            }
            else self.SendNext("Among these flowers you will find some with a mysterious aura around them. You won't be able to pick them up because of the aura that surrounds them.");
            self.Stop();
        }
    }

    // herbal bush
    [Script("bush2")]
    class Bush2 : INpcScript
    {
        public void Run(INpcHost self, GameCharacter target, byte state, byte nRet, string stringAnswer, int nRetNum)
        {
            var qr = target.Quests;
            var val = qr.GetState(2051);

            var inventory = target.Inventory;

            if (target.Level > 49)
            {
                if (val == 1)
                {
                    if (state == 0)
                    {
                        self.AskYesNo("Are you sure you want to take #b#t4031032##k with you?");
                        return;
                    }
                    else if (nRet != 0)
                    {
                        var ret = target.TryExchange(0, 4031032, 1);
                        if (!ret) self.SendNext("Your etc inventory seems to be full. Please make some room to take the item.");
                        else target.ChangeMap(101000000);
                    }
                }
                else if (val == 2)
                {
                    if (inventory.GetOpenSlotsInInventory(Inventory.Etc) >= 1 && inventory.GetOpenSlotsInInventory(Inventory.Equip) >= 1)
                    {
                        int nNewItemID = 0;
                        int rnum = Rand32.NextBetween(1, 30);
                        short itemNumber = 0;
                        if (rnum > 0 && rnum < 11) {
                            nNewItemID = 4020007;
                            itemNumber = 2;
                        }
                        else if (rnum > 10 && rnum < 21) {
                            nNewItemID = 4020008;
                            itemNumber = 2;
                        }
                        else if (rnum > 20 && rnum < 30) {
                            nNewItemID = 4010006;
                            itemNumber = 2;
                        }
                        else if (rnum == 30)
                        {
                            nNewItemID = 1032013;
                            itemNumber = 1;
                        }

                        var ret = target.TryExchange(0, nNewItemID, itemNumber);
                        if (!ret) self.SendNext("Your equipment and etc inventory are full, which does not allow you to accept more items. You need to free up space in your inventory of etc.");
                        else target.ChangeMap(101000000);
                    }
                    else self.SendNext("You need to free up some space in your equipment and etc inventories, so that you can place the items you found in the herb bushes. Please check again after adjusting.");
                }
                else self.SendNext("In the middle of the herb bushes you will find roots with a mysterious energy, but since there was no explanation from #b#p1061005##k about them, there's no way to know which root to take...");
            }
            else self.SendNext("In the middle of the herb bushes, you'll find roots with a mysterious energy, but a strange aura around them makes it impossible for us to pick them up.");
            self.Stop();
        }
    }

    // taking the character out of the map of herbs
    [Script("herb_out")]
    class HerbOut : INpcScript
    {
        public void Run(INpcHost self, GameCharacter target, byte state, byte nRet, string stringAnswer, int nRetNum)
        {
            if (state == 0)
            {
                self.AskYesNo("Do you want to get out of here? Well... this place can really wear you out... I'm used to it, I'm fine. Anyway, remember that if you get out of here through me, you'll have to start the quest all over again. Still want to go?");
                return;
            }
            else if (nRet == 0) self.SendNext("Isn't it awful that you have to restart the whole thing? Keep trying... the more you explore, the better you get to know this whole place. Soon you'll be able to walk around here with your eyes closed, hehe.");
            else target.ChangeMap(101000000);
            self.Stop();
        }
    }

    // letting the character into the map of herbs
    [Script("herb_in")]
    class HerbIn : INpcScript
    {
        public void Run(INpcHost self, GameCharacter target, byte state, byte nRet, string stringAnswer, int nRetNum)
        {
            var qr = target.Quests;
            var val = qr.GetState(2050);
            var val2 = qr.GetState(2051);

            if (val2 == 0)
            {
                if (val == 1)
                {
                    var nPrice = target.Level * 100;
                    if (state == 0)
                    {
                        self.AskYesNo("So you came here on #b#p1061005##k's request to pick up medicinal herbs? Well... I inherited this land from my father, and I can't leave some stranger here for nothing... But, with #r" + nPrice.ToFormattedString() + "#k mesos, it's a totally different story... So, you want to pay the entrance fee?");
                        return;
                    }
                    else if (nRet == 0) self.SendNext("I understand... but understand my side too, you can't stay here for free.");
                    else
                    {
                        var ret = target.TryExchange(-nPrice);
                        if (!ret) self.SendNext("Are you running out of money? See if you have more than #r" + nPrice.ToFormattedString() + "#k mesos on hand. Don't expect me to give any discount.");
                        else target.ChangeMap(101000100);
                    }
                }
                else if (val == 2)
                {
                    self.SendNext("Is that you from the other day... Is #p1061005# working hard on the diet medicine? Anyway, I was kind of surprised you got around this place without difficulty. As a reward, I'll let you stay a while without paying. You might even find some cool items there along the way.");
                    target.ChangeMap(101000100);
                }
                else self.SendNext("Want to come in? You must have heard that there are precious medicinal herbs here, huh? But I can't leave a stranger like you around, who doesn't even know I own this land. I'm sorry, but that's all.");
            }
            else if (val2 == 1)
            {
                var nPrice = target.Level * 200;
                if (state == 0)
                {
                    self.AskYesNo("Is that you from the other day... #b#p1061005##k made another order for you? What? Do you need to stay longer? Hmm... it's very dangerous there, but... that's ok, with #r" + nPrice.ToFormattedString() + "#k mesos I'll let you rummage around. So, will you pay for admission?");
                }
                else if (nRet == 0) self.SendNext("I understand... but understand my side too, you can't stay here for free.");
                else
                {
                    var ret = target.TryExchange(-nPrice);
                    if (!ret) self.SendNext("Are you running out of money? See if you have more than #r" + nPrice.ToFormattedString() + "#k mesos in hand. Don't expect me to give any discount.");
                    else target.ChangeMap(101000102);
                }
            }
            else if (val2 == 2)
            {
                if (state == 0)
                {
                    self.SendNext("Is that you from the other day... Is #p1061005# working hard on the anti-aging medicine? Anyway, I was kind of surprised you got around this place without difficulty. As a reward, I'll let you stay a while without paying. You might even find some cool items there along the way.");
                    return;
                }
                else if (state == 1)
                {
                    self.SendBackNext("Oh yea... #p1032100#, from this very town, tried to sneak in. I caught him, and in the process #p1032100# dropped something over there. I tried looking, but I have no idea where it is. What do you think about looking?");
                    return;
                }
                else target.ChangeMap(101000102);
            }
            else self.SendNext("Want to come in? You must have heard that there are precious medicinal herbs here, huh? But I can't leave a stranger here who doesn't even know I own this land. Sorry, but I think that's about it.");
            self.Stop();
        }
    }
}