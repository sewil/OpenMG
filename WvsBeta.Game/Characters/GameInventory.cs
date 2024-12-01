﻿using System;
using System.Collections.Generic;
using System.Linq;
using WvsBeta.Common;
using WvsBeta.Common.Characters;
using WvsBeta.Common.DataProviders;
using WvsBeta.Common.Enums;
using WvsBeta.Common.Extensions;
using WvsBeta.Common.Objects;
using WvsBeta.Game.Packets;
using static WvsBeta.Common.Constants;
using static WvsBeta.Game.GameInventory;

namespace WvsBeta.Game
{
    public class GameInventory : Common.Characters.Inventory
    {
        private GameCharacter Character { get; set; }

        public GameInventory(GameCharacter character) : base(character.UserID, character.ID)
        {
            Character = character;
        }

        public void SaveInventory()
        {
            base.SaveInventory(Program.MainForm.LogAppend);
        }

        public new void LoadInventory()
        {
            base.LoadInventory();

            UpdateChocoCount(false);
        }

        public override void AddItem(InventoryType inventory, short slot, Item item, bool isLoading)
        {
            base.AddItem(inventory, slot, item, isLoading);

            if (slot < 0 && item is EquipItem equipItem)
            {
                slot = Math.Abs(slot);
                Character.PrimaryStats.UpdateEquipStats((sbyte)slot, equipItem, isLoading);
            }

            if (!isLoading)
                UpdateChocoCount();
        }

        public override void SetItem(Common.Enums.InventoryType inventory, short slot, Item item)
        {
            base.SetItem(inventory, slot, item);
            if (Character.IsOnline)
            {
                if (slot < 0 && Math.Abs(slot) <= 100)
                {
                    Character.PrimaryStats.UpdateEquipStats((sbyte)slot, item as EquipItem, false);
                }
                UpdateChocoCount();
            }
        }

        public Item GetEquippedItem(int itemid, out EquippedType type)
        {
            type = (EquippedType)(-1);
            foreach (var equips in Equipped)
            {
                foreach (var item in equips.Value)
                {
                    if (item.Value.ItemID == itemid)
                    {
                        type = equips.Key;
                        return item.Value;
                    }
                }
            }
            return null;
        }

        public void UpdateChocoCount(bool sendPacket = true)
        {

            int prevChocoCount = ChocoCount;
            ChocoCount = Items[Constants.getInventory(Constants.Items.Choco)].Count(x => x?.ItemID == Constants.Items.Choco);
            ActiveItemID = ChocoCount > 0 ? Constants.Items.Choco : 0;

            if (sendPacket && prevChocoCount != ChocoCount)
            {
                MapPacket.SendAvatarModified(Character, MapPacket.AvatarModFlag.ItemEffects);
            }
        }

        public IList<OperationOut> AddItemOperations(Item item, out short amountLeft, bool sendOperations = true, bool isSelf = true)
        {
            Common.Enums.InventoryType inventory = Constants.getInventory(item.ItemID);
            short freeSlot = 0;
            IList<OperationOut> operations = new List<OperationOut>();
            bool stackable = Constants.isStackable(item.ItemID);
            short maxSlots = 1;
            amountLeft = item.Amount;

            if (GameDataProvider.Items.TryGetValue(item.ItemID, out ItemData itemData))
            {
                maxSlots = (short)itemData.MaxSlot;
                if (maxSlots == 0) maxSlots = 100;
            }

            for (short slot = 1; slot <= MaxSlots[inventory]; slot++)
            {
                Item slotItem = GetItem(inventory, slot);

                if (freeSlot == 0 && slotItem == null) freeSlot = slot;
                if (!stackable)
                {
                    if (freeSlot == 0) continue;
                    else break;
                }
                if (slotItem == null) continue;
                if (item.ItemID != slotItem.ItemID) continue;
                if (slotItem.Amount == maxSlots) continue;

                if (item.Amount + slotItem.Amount > maxSlots) // Bundle overflow
                {
                    item.Amount -= (short)(maxSlots - slotItem.Amount);
                    amountLeft = item.Amount;
                    slotItem.Amount = maxSlots;
                    operations.Add(InventoryOperationPacket.ChangeAmountOperation(inventory, slot, maxSlots));
                }
                else // Update slot amount
                {
                    slotItem.Amount += item.Amount;
                    amountLeft = 0;
                    operations.Add(InventoryOperationPacket.ChangeAmountOperation(inventory, slot, slotItem.Amount));
                    break;
                }
            }
            if (amountLeft > 0)
            {
                if (freeSlot != 0)
                {
                    SetItem(inventory, freeSlot, item);
                    operations.Add(InventoryOperationPacket.AddOperation(item));
                    amountLeft = 0;
                }
                else
                {
                    amountLeft = item.Amount;
                }
            }

            if (sendOperations)
            {
                InventoryOperationPacket.Run(Character, isSelf, operations.ToArray());
            }

            return operations;
        }

        public override short AddNewItem(int id, short amount) // Only normal items!
        {
            if (!GameDataProvider.Items.ContainsKey(id) &&
                !GameDataProvider.Equips.ContainsKey(id) &&
                !GameDataProvider.Pets.ContainsKey(id))
            {
                return 0;
            }

            short max = 1;
            if (!Constants.isEquip(id) && !Constants.isCash(id))
            {
                max = (short)GameDataProvider.Items[id].MaxSlot;
                if (max == 0)
                {
                    max = 100;
                }
            }
            short thisAmount = 0, givenAmount = 0;

            if (Constants.isRechargeable(id))
            {
                thisAmount = (short)(max + Character.Skills.GetRechargeableBonus());
                amount -= 1;
            }
            else if (Constants.isEquip(id) || Constants.isCash(id))
            {
                thisAmount = 1;
                amount -= 1;
            }
            else if (amount > max)
            {
                thisAmount = max;
                amount -= max;
            }
            else
            {
                thisAmount = amount;
                amount = 0;
            }

            if (Constants.isCash(id))
            {
                givenAmount = 0;
            }
            else
            {
                var item = Item.CreateFromItemID(id);
                item.Amount = thisAmount;
                givenAmount += thisAmount;
                AddItemOperations(item, out short amountLeft);
                if (amountLeft == 0 && amount > 0)
                {
                    givenAmount += AddNewItem(id, amount);
                }
            }

            return givenAmount;
        }

        public bool HasEquipped(int itemId, EquippedType type, params Constants.EquipSlots.Slots[] checkSlots)
        {
            foreach (var slot in checkSlots)
            {
                if (Equipped[type].TryGetValue(slot, out EquipItem item) && item.ItemID == itemId) return true;
            }
            return false;
        }

        public override bool HasSlotsFreeForItem(int itemid, short amount)
        {
            short slotsRequired = 0;
            Common.Enums.InventoryType inventory = Constants.getInventory(itemid);
            bool stackable = Constants.isStackable(itemid);
            if (Constants.isStar(itemid))
            {
                slotsRequired = 1;
            }
            else if (!stackable)
            {
                slotsRequired = amount;
            }
            else
            {
                short maxPerSlot = (short)GameDataProvider.Items[itemid].MaxSlot;
                if (maxPerSlot == 0) maxPerSlot = 100; // default 100 O.o >_>
                short hasAmounts = (short)(ItemAmounts.ContainsKey(itemid) ? ItemAmounts[itemid] : 0);
                if (stackable && hasAmounts > 0)
                {
                    // We should try to see which slots we can fill, and determine how much new slots are left

                    short amountLeft = amount;
                    Common.Enums.InventoryType inv = Constants.getInventory(itemid);
                    foreach (var item in Items[inv].ToList().FindAll(x => x != null && x.ItemID == itemid && x.Amount < maxPerSlot))
                    {
                        amountLeft -= (short)(maxPerSlot - item.Amount); // Substract the amount of 'slots' left for this slot
                        if (amountLeft <= 0)
                        {
                            amountLeft = 0;
                            break;
                        }
                    }

                    // Okay, so we need to figure out where to keep these stackable items.

                    // Apparently we've got space left on slots
                    if (amountLeft == 0) return true;

                    // Hmm, still need to get more slots
                    amount = amountLeft;
                }

                slotsRequired = (short)(amount / maxPerSlot);
                // Leftover slots to handle
                if ((amount % maxPerSlot) > 0)
                    slotsRequired++;

            }
            return GetOpenSlotsInInventory(inventory) >= slotsRequired;
        }

        public override int ItemAmountAvailable(int itemid)
        {
            Common.Enums.InventoryType inv = Constants.getInventory(itemid);
            int available = 0;
            short maxPerSlot = (short)(GameDataProvider.Items.ContainsKey(itemid) ? GameDataProvider.Items[itemid].MaxSlot : 1); // equip
            if (maxPerSlot == 0) maxPerSlot = 100; // default 100 O.o >_>

            short openSlots = GetOpenSlotsInInventory(inv);
            available += (openSlots * maxPerSlot);

            Item temp = null;

            for (short i = 1; i <= MaxSlots[inv]; i++)
            {
                temp = GetItem(inv, i);
                if (temp != null && temp.ItemID == itemid)
                    available += (maxPerSlot - temp.Amount);
            }

            return available;
        }

        public override short GetNextFreeSlotInInventory(Common.Enums.InventoryType inventory)
        {
            for (short i = 1; i <= MaxSlots[inventory]; i++)
            {
                if (GetItem(inventory, i) == null)
                    return i;
            }
            return -1;
        }

        public override short DeleteFirstItemInInventory(Common.Enums.InventoryType inv)
        {
            for (short i = 1; i <= MaxSlots[inv]; i++)
            {
                if (Items[inv][i] != null)
                {
                    Items[inv][i] = null;
                    UpdateChocoCount();
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Set the MaxSlots for <param name="inventory"/> to <param name="slots" />.
        /// If the Items array is already initialized, it will either expand the array,
        /// or, when <param name="slots" /> is less, will remove items and shrink it.
        /// </summary>
        /// <param name="inventory">Inventory ID</param>
        /// <param name="slots">Amount of slots</param>
        public override void SetInventorySlots(InventoryType inventory, byte slots, bool sendPacket = true)
        {
            base.SetInventorySlots(inventory, slots, sendPacket);

            if (sendPacket)
                InventoryPacket.IncreaseSlots(Character, inventory, slots);
        }

        public override void TakeItem(Item item, InventoryType inventory, short slot, short amount)
        {
            var isRechargeable = Constants.isRechargeable(item.ItemID);
            if (amount > item.Amount) return;
            item.Amount -= amount;
            if (item.Amount == 0 && !isRechargeable)
            {
                // Your item. Gone.
                SetItem(inventory, slot, null);
                TryRemoveCashItem(item);
                InventoryOperationPacket.SwitchSlots(Character, inventory, slot, 0);
            }
            else
            {
                // Update item with new amount
                InventoryOperationPacket.ChangeAmount(Character, item);
            }
        }

        public bool HasItemAmount(int itemId, short amount)
        {
            if (amount <= 0) return false;

            short actualAmount = 0;
            var isRechargeable = Constants.isRechargeable(itemId);
            var inv = Constants.getInventory(itemId);
            for (short slot = 1; slot <= MaxSlots[inv]; slot++)
            {
                var item = GetItem(inv, slot);
                if (item == null || item.ItemID != itemId) continue;

                actualAmount += isRechargeable ? (short)1 : item.Amount;
                if (actualAmount >= amount) return true;
            }
            return false;
        }

        public void AddMesos(int value, bool isSelf = false)
        {
            AddMesos(value, isSelf, out int _);
        }
        public void AddMesos(int value, bool isSelf, out int mesosDiff)
        {
            int newMesos = Math.Max(0, Math.Min(int.MaxValue, Mesos + value));
            mesosDiff = newMesos - Mesos;
            Mesos = newMesos;
            CharacterStatsPacket.SendStatChanged(Character, StatFlags.Mesos, isSelf);
        }
        /// <summary>
        /// Try to remove <paramref name="amount"/> amount of itemid <paramref name="itemid"/>.
        /// Does not 'remove' stacks, keeps them as-is (with 0 items).
        /// </summary>
        /// <param name="itemid">The Item ID</param>
        /// <param name="amount">Amount</param>
        /// <param name="sendOperations">Send operations</param>
        /// <returns>Amount of items that were _not_ taken away</returns>
        public IList<OperationOut> TakeItemOperations(int itemid, short amount, bool sendOperations = true)
        {
            var operations = new List<OperationOut>();
            if (amount == 0) return operations;

            var isRechargeable = Constants.isRechargeable(itemid);
            Common.Enums.InventoryType inventory = Constants.getInventory(itemid);
            short amountLeft = (short)amount;
            for (short slot = 1; slot <= MaxSlots[inventory]; slot++)
            {
                if (amountLeft == 0) break; // Nothing more to take
                Item item = GetItem(inventory, slot);
                if (item == null || item.ItemID != itemid) continue;

                short maxTake = Math.Min(item.Amount, amountLeft);
                item.Amount -= (short)maxTake;
                amountLeft -= maxTake;
                if (item.Amount == 0 && !isRechargeable)
                {
                    // Your item. Gone.
                    SetItem(inventory, slot, null);
                    TryRemoveCashItem(item);
                    operations.Add(InventoryOperationPacket.SwitchSlotsOperation(inventory, slot, 0));
                }
                else
                {
                    // Update item with new amount
                    operations.Add(InventoryOperationPacket.ChangeAmountOperation(item.Inventory, item.InventorySlot, item.Amount));
                }
            }

            if (sendOperations)
            {
                InventoryOperationPacket.Run(Character, true, operations.ToArray());
            }

            return operations;
        }

        public override Item TakeItemAmountFromSlot(Common.Enums.InventoryType inventory, short slot, short amount, bool takeStars)
        {
            var item = GetItem(inventory, slot);

            if (item == null) return null;

            if (!takeStars)
            {
                if (item.Amount - amount < 0) return null;
            }

            bool removeItem = false;
            Item newItem;
            if (takeStars && Constants.isStar(item.ItemID))
            {
                // Take the whole item
                newItem = item;
                removeItem = true;
            }
            else
            {
                newItem = item.SplitInTwo(amount);
                removeItem = item.Amount == 0 && !Constants.isStar(item.ItemID);
            }

            if (removeItem)
            {
                SetItem(inventory, slot, null);
                TryRemoveCashItem(item);
                InventoryOperationPacket.SwitchSlots(Character, inventory, slot, 0);
            }
            else
            {
                // Update item
                InventoryOperationPacket.ChangeAmount(Character, item);
            }

            return newItem;
        }

        public Dictionary<byte, int> GetVisibleEquips()
        {
            Dictionary<byte, int> shown = new Dictionary<byte, int>();

            foreach (var item in Equipped[EquippedType.Normal])
            {
                shown[(byte)item.Key] = item.Value.ItemID;
            }

            // Cash overrides normal
            foreach (var item in Equipped[EquippedType.Cash])
            {
                shown[(byte)item.Key] = item.Value.ItemID;
            }
            return shown;
        }

        public int GetTotalWAttackInEquips(int bulletID = 0)
        {
            int totalWat = Equipped[EquippedType.Normal].Sum(i => i.Value.Watk);

            if (bulletID > 0 && DataProvider.Items.TryGetValue(bulletID, out var bullet))
            {
                totalWat += bullet.IncWAtk;
            }

            return totalWat;
        }

        public double GetExtraExpRate()
        {
            // Holiday stuff here.
            double rate = 1;

            foreach (Item item in Items[Common.Enums.InventoryType.Etc])
            {
                if (item == null || item.ItemID < 4100000 || item.ItemID >= 4200000) continue; // ???
                ItemData id = GameDataProvider.Items[item.ItemID];
                if (ItemData.RateCardEnabled(id, false))
                {
                    if (rate < id.Rate) rate = id.Rate;
                }
            }
            return rate;
        }


        private long lastCheck = 0;
        public void GetExpiredItems(long time, Action<List<Item>> callback)
        {
            if (time - lastCheck < 45000) return;
            lastCheck = time;

            var expiredItems = Equipped.SelectMany(i => i.Value).Where(x => x.Value.HasExpired(time)).Select(i => i.Value as Item)
                .Concat(Items.SelectMany(i => i.Value).Where(i => i != null).Where(i => i.HasExpired(time)))
                .ToList();

            if (expiredItems.Count == 0) return;

            callback(expiredItems);
        }

        public void CheckExpired()
        {
            var currentTime = MasterThread.CurrentTime;
            _cashItems.GetExpiredItems(currentTime, expiredItems =>
            {
                var dict = new Dictionary<Common.Enums.InventoryType, List<short>>();
                expiredItems.ForEach(x =>
                {
                    InventoryPacket.SendCashItemExpired(Character, x.ItemId);
                    Common.Enums.InventoryType inventory = Constants.getInventory(x.ItemId);
                    var baseItem = GetItemByCashID(x.CashId, inventory);

                    if (baseItem != null)
                    {
                        if (dict.TryGetValue(inventory, out var curList)) curList.Add(baseItem.InventorySlot);
                        else
                        {
                            dict[inventory] = new List<short> { baseItem.InventorySlot };
                        }
                    }
                    RemoveLockerItem(x, baseItem, true);
                });

                dict.ForEach(x => InventoryOperationPacket.MultiDelete(Character, x.Key, x.Value.ToArray()));
            });

            GetExpiredItems(currentTime, expiredItems =>
            {
                var dict = new Dictionary<Common.Enums.InventoryType, List<short>>();
                var itemIds = new List<int>();
                Server.Instance.SpawnedPets.TryGetValue(Character.ID, out PetItem spawnedPet);
                expiredItems.ForEach(x =>
                {
                    if (x is PetItem pi)
                    {
                        if (spawnedPet != null && spawnedPet.CashId == pi.CashId)
                        {
                            PetsPacket.RemovePet(Character, PetRemoveReason.Expire, true);
                        }
                        pi.DeadDate = Item.NoItemExpiration;
                        pi.SpawnedDate = 0;
                        Pet.UpdatePet(Character, pi);
                    }
                    else
                    {
                        Common.Enums.InventoryType inventory = Constants.getInventory(x.ItemID);
                        if (x.CashId != 0)
                        {
                            var baseItem = GetItemByCashID(x.CashId, inventory);
                            if (dict.TryGetValue(inventory, out var curList)) curList.Add(baseItem.InventorySlot);
                            else
                            {
                                dict[inventory] = new List<short> { baseItem.InventorySlot };
                            }
                            TryRemoveCashItem(x);
                        }
                        SetItem(inventory, x.InventorySlot, null);
                        itemIds.Add(x.ItemID);
                    }
                });

                InventoryPacket.SendItemsExpired(Character, itemIds);
                dict.ForEach(x => InventoryOperationPacket.MultiDelete(Character, x.Key, x.Value.ToArray()));
            });
        }

        public IList<LockerItem> GetCoupleRings()
        {
            return _cashItems.Items.Where(c => c != null && Constants.isCoupleRing(c.ItemId)).ToList();
        }
        public EquipItem GetEquippedCoupleRing()
        {
            return Equipped[EquippedType.Cash].Select(i => i.Value).FirstOrDefault(c => Constants.isCoupleRing(c.ItemID));
        }
        #region Script helpers
        public int SlotCount(byte inventory)
        {
            return SlotCount((Common.Enums.InventoryType)inventory);
        }
        public int SlotCount(Common.Enums.InventoryType inventory)
        {
            return MaxSlots[inventory];
        }
        public int HoldCount(byte inventory)
        {
            return HoldCount((Common.Enums.InventoryType)inventory);
        }
        public int HoldCount(Common.Enums.InventoryType inventory)
        {
            int holdCount = 0;
            for (short slot = 1; slot <= MaxSlots[inventory]; slot++)
            {
                if (GetItem(inventory, slot) != null) holdCount++;
            }
            return holdCount;
        }
        public int ItemCount(int itemid)
        {
            return GetItemAmount(itemid);
        }
        public bool SetPetLife(long petCashID, params (int itemid, short amount)[] exchangeItems)
        {
            if (!TryGetPet(petCashID, out PetItem petItem)) return false;
            if (!MassExchange(0, exchangeItems)) return false;

            petItem.DeadDate = MasterThread.CurrentTime + 90 * TimeExtensions.DayMillis;
            petItem.Fullness = 100;
            Pet.UpdatePet(Character, petItem);

            return true;
        }
        public enum MovePetStatResult
        {
            Success = 0,
            MissingItem = 1,
            WrongCloseness = 2,
        }
        public MovePetStatResult MovePetStat(long petcashidfrom, long petcashidto, int exchangeItem)
        {
            if (!TryGetPet(petcashidfrom, out PetItem petFrom) || !TryGetPet(petcashidto, out PetItem petTo)) return MovePetStatResult.MissingItem;
            if (!HasItemAmount(exchangeItem, 1)) return MovePetStatResult.MissingItem;
            if (petTo.Closeness > petFrom.Closeness) return MovePetStatResult.WrongCloseness;

            MassExchange(0, (exchangeItem, 1));

            petTo.Closeness = petFrom.Closeness;
            petTo.Level = petFrom.Level;
            petFrom.Level = 1;
            petFrom.Closeness = 0;

            Pet.UpdatePet(Character, petFrom);
            Pet.UpdatePet(Character, petTo);
            return MovePetStatResult.Success;
        }
        public bool CanExchange(int mesos, params (int itemid, short amount)[] items)
        {
            var itemsList = items.Select(i => new ExchangeItem(i.itemid, i.amount));
            return CanExchange(mesos, itemsList);
        }
        public bool CanExchange(int mesos, IEnumerable<ExchangeItem> items)
        {
            if (mesos != 0)
            {
                long newM = (long)Mesos + (long)mesos;
                bool canExchangeMesos = 0 <= newM && newM <= int.MaxValue;
                if (!canExchangeMesos) return false;
            }
            if (items != null)
            {
                foreach (var item in items)
                {
                    short amount = item.amount;
                    int itemid = item.itemID;
                    if (amount == 0) continue;
                    if (amount < 0 && !HasItemAmount(itemid, Math.Abs(amount))) // Take item
                    {
                        return false;
                    }
                    else if (amount > 0 && !HasSlotsFreeForItem(itemid, amount)) // Give item
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Mass exchange, used by scripts when exchanging with NPCs
        /// </summary>
        /// <param name="mesos">mesos to exchange, can be positive or negative</param>
        /// <param name="items">items to exchange, alternating between itemid and amount: itemid,amount,itemid,amount...</param>
        /// <returns>Status code: 1 = success, 0 = error</returns>
        public int Exchange(int mesos, params int[] items)
        {
            if (items.Length % 2 != 0)
            {
                Program.MainForm.LogDebug("Invalid exchange args {0} {1}", mesos, items);
                return 0;
            }
            // Parse
            var parsedItems = new List<(int itemid, short amount)>();
            for (int i = 0; i < items.Length; i++)
            {
                if (i % 2 != 0) continue;
                short amount = (short)items[i + 1];
                if (amount == 0) continue;
                parsedItems.Add((items[i], amount));
            }
            return MassExchange(mesos, parsedItems.ToArray()) ? 1 : 0;
        }
        public bool MassExchange(int mesos, params (int itemid, short amount)[] items)
        {
            var itemsList = items.Select(i => new ExchangeItem(i.itemid, i.amount)).ToList();
            return MassExchange(mesos, itemsList);
        }
        public bool MassExchange(int mesos, IList<ExchangeItem> exchangeItems, MessageAppearType appearType = MessageAppearType.ChatGrey)
        {
            if (!CanExchange(mesos, exchangeItems)) return false;

            AddMesos(mesos);
            if (mesos != 0)
            {
                if (appearType == MessageAppearType.SideWhite)
                    Character.SendPacket(MessagePacket.DropPickup(true, mesos, 0));
                else if (appearType == MessageAppearType.ChatGrey)
                    Character.SendPacket(MessagePacket.GainMesos(mesos));

            }
            if (exchangeItems != null)
            {
                var operations = new List<OperationOut>();
                foreach(var exchangeItem in exchangeItems)
                {
                    short amount = exchangeItem.amount;
                    int itemid = exchangeItem.itemID;
                    if (amount < 0) // Take item
                    {
                        operations.AddRange(TakeItemOperations(itemid, (short)-amount, false));
                    }
                    else // Give item
                    {
                        var createdItem = Item.CreateFromItemID(exchangeItem.itemID, exchangeItem.amount, exchangeItem.periodMinutes, exchangeItem.variation);
                        operations.AddRange(AddItemOperations(createdItem, out short _, false));
                    }
                }
                InventoryOperationPacket.Run(Character, false, operations.ToArray());
                if (appearType == MessageAppearType.ChatGrey)
                    PlayerEffectPacket.SendInventoryChanged(Character, exchangeItems);
            }
            return true;
        }
        public class ExchangeItem
        {
            public int itemID;
            public short amount;
            public int periodMinutes;
            public ItemVariation variation;
            public ExchangeItem() { }
            public ExchangeItem(int itemID, short amount)
            {
                this.itemID = itemID;
                this.amount = amount;
            }
        }
        /// <summary>
        /// Mass exchange with query:
        /// ExchangeEx(0, "2070006,Count:800", 800, "2070006,Count:800", 800, "2070006,Count:800", 800, "2070006,Count:800", 800, "2070006,Count:800", 800)
        /// </summary>
        /// <returns>Status code: 1 = success, 0 = error</returns>
        /// <exception cref="ArgumentException"></exception>
        public int ExchangeEx(int mesos, params object[] items)
        {
            var exchangeItems = new List<ExchangeItem>();
            for (int i = 0; i < items.Length; i++)
            {
                object item = items[i];
                if (!(item is string query)) continue;

                string[] queries = query.Split(',');
                int itemid = int.Parse(queries[0]);
                var exchangeItem = new ExchangeItem
                {
                    itemID = itemid
                };

                for (int queryIdx = 1; queryIdx < queries.Length; queryIdx++)
                {
                    var kv = queries[queryIdx].Split(':');
                    string key = kv[0];
                    int value = int.Parse(kv[1]);
                    if (key == "Period")
                    {
                        exchangeItem.periodMinutes = value;
                        break;
                    }
                    else if (key == "Variation")
                    {
                        exchangeItem.variation = (ItemVariation)value;
                        break;
                    }
                }

                short amount = Convert.ToInt16(items[i + 1]);
                exchangeItem.amount = amount;
                exchangeItems.Add(exchangeItem);
            }
            return MassExchange(mesos, exchangeItems) ? 1 : 0;
        }

        public void IncSlotCount(byte inventory, byte slots)
        {
            var inventoryType = (InventoryType)inventory;
            var newSlots = (byte)(MaxSlots[inventoryType] + slots);
            SetInventorySlots(inventoryType, newSlots);
        }
        /// <summary>
        /// Remove equipped item
        /// </summary>
        /// <param name="itemid"></param>
        /// <returns>-1 = item not found, 0 = success, 1 = failed to unequip</returns>
        public int RemoveEquippedItem(int itemid)
        {
            var item = GetEquippedItem(itemid, out EquippedType _);
            if (item == null) return -1;
            InventoryPacket.Unequip(Character, item, 0);
            return 0;
        }

        public int HasThisItemInHisParty(int itemid)
        {
            var party = Character.Party;
            if (party == null) return 0;
            foreach (var m in party.Characters)
            {
                if (m.Inventory.HasItemAmount(itemid, 1))
                {
                    return 1;
                }
            }
            return 0;
        }
        #endregion
    }
}