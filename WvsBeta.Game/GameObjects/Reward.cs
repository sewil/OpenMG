﻿using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using WvsBeta.Common;
using WvsBeta.Common.Enums;
using WvsBeta.Common.Extensions;
using WvsBeta.Common.Objects;
using WvsBeta.Common.Sessions;

namespace WvsBeta.Game
{
    public class Reward
    {
        public bool Mesos;
        public int Drop;
        public Item Data { get; private set; }

        public long DateExpire
        {
            get
            {
                long Result = 0;
                if (Data != null) Result = Data.Expiration;
                return (Result == 0) ? Item.NoItemExpiration : Result;
            }
        }

        public short Amount => Data?.Amount ?? -1;
        public int ItemID => (!Mesos) ? Drop : 0;

        // Server drop rate
        public static double ms_fIncDropRate => Server.Instance.RateDropChance;
        // Server 'event time' droprate (between 1pm and 6pm)
        public static double ms_fIncDropRate_WSE => 1.0;
        // Used for MC drops, map the MCType prop of the item to some table calculated in CField_MonsterCarnival::GetMCRewardRate
        public static double MonsterCarnivalRewardRate => 1.0;

        public static List<Reward> GetRewards(GameCharacter Owner, Map Field, int ID, char Type, bool PremiumMap, double Showdown)
        {
            double HourDropRateIncrease = 1.0;
            var curDate = MasterThread.CurrentDate;
            long cTime = MasterThread.CurrentTime;
            if (curDate.Hour >= 13 && curDate.Hour < 19)
            {
                HourDropRateIncrease = ms_fIncDropRate_WSE;
            }
            
            double dRegionalIncRate = Field.m_dIncRate_Drop;
            double dwOwnerDropRate = Owner.m_dIncDropRate;
            double dwOwnerDropRate_Ticket = Owner.m_dIncDropRate_Ticket;

            var Result = new List<Reward>();

            if (!GameDataProvider.Drops.TryGetValue($"{Type}{ID}", out var Rewards)) return Result;

            foreach (var Drop in Rewards)
            {
                if ((Drop.Premium && !PremiumMap))
                    continue;

                if (
                    GameDataProvider.QuestItems.TryGetValue(Drop.ItemID, out var itemQuests) && 
                    !itemQuests.Any(itemQuest => Owner.Quests.HasQuestState(itemQuest, QuestState.InProgress))
                ) continue;

                var itemDropRate = 1.0;
                if (Drop.Mesos == 0)
                    itemDropRate = dwOwnerDropRate_Ticket;

                var maxDropChance = (long)(1000000000.0
                                           / (ms_fIncDropRate * HourDropRateIncrease)
                                           / dRegionalIncRate
                                           / Showdown
                                           / dwOwnerDropRate
                                           / itemDropRate
                                           / MonsterCarnivalRewardRate);

                var luckyNumber = Rand32.Next() % maxDropChance;

                if (luckyNumber >= Drop.Chance) continue;

                // Don't care about items that are 'expired'
                if (Drop.Mesos != 0 && Drop.Expiration <= cTime) continue;

                var Reward = new Reward()
                {
                    Mesos = Drop.Mesos != 0,
                    Drop = Drop.Mesos != 0 ? Drop.Mesos : Drop.ItemID,
                    Data = Drop.Mesos != 0 ? null : Item.CreateFromItemID(Drop.ItemID, GetItemAmount(Drop.ItemID, Drop.Min, Drop.Max))
                };

                if (!Reward.Mesos)
                {
                    if (Reward.Data is EquipItem)
                        (Reward.Data as EquipItem).GiveStats(ItemVariation.Normal);
                    //if (drop.Quest > 0 && reward.Data.IsOnly && owner.Inventory.ItemCount(drop.ItemID) > 0) continue;
                    if (Drop.Period > 0)
                    {
                        Reward.Data.Expiration = cTime + Drop.Period * TimeExtensions.DayMillis;
                    }
                    else if (Drop.Expiration != Item.NoItemExpiration)
                    {
                        Reward.Data.Expiration = Drop.Expiration;
                    }
                }

                if (!Drop.Premium || PremiumMap)
                {
                    if (Reward.Mesos)
                    {
                        int minDrop = 4 * Reward.Drop / 5;
                        int maxDrop = 2 * Reward.Drop / 5 + 1;
                        int DroppedMesos = (int)(minDrop + Rand32.Next() % maxDrop);

                        if (DroppedMesos <= 1)
                            DroppedMesos = 1;

                        DroppedMesos = (int)(DroppedMesos * dwOwnerDropRate_Ticket);
                        Reward.Drop = DroppedMesos;
                    }
                }

                Result.Add(Reward);
            }

            return Result;
        }

        public static Reward Create(Item Item)
        {
            return new Reward()
            {
                Mesos = false,
                Data = Item,
                Drop = Item.ItemID
            };
        }

        public static Reward Create(double Mesos)
        {
            return new Reward()
            {
                Mesos = true,
                Drop = Convert.ToInt32(Mesos)
            };
        }

        private static short GetItemAmount(int ItemID, int Min, int Max)
        {
            var ItemType = ItemID / 1000000;
            if (Max > 0 && (ItemType == 2 || ItemType == 3 || ItemType == 4))
                return (short)(Min + Rand32.Next() % (Max - Min + 1));
            return 1;
        }

        public void EncodeForMigration(Packet pw)
        {
            pw.WriteBool(Mesos);
            pw.WriteInt(Drop);
            if (!Mesos)
            {
                Data.EncodeForMigration(pw);
            }
        }

        public static Reward DecodeForMigration(Packet pr)
        {
            var reward = new Reward();
            reward.Mesos = pr.ReadBool();
            reward.Drop = pr.ReadInt();
            if (!reward.Mesos)
            {
                reward.Data = Item.DecodeForMigration(pr);
            }
            return reward;
        }
    }
}
