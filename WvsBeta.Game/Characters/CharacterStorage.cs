﻿using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using WvsBeta.Common;
using WvsBeta.Common.Enums;
using WvsBeta.Common.Objects;

namespace WvsBeta.Game
{
    public class CharacterStorage
    {
        public GameCharacter Character { get; set; }
        private Dictionary<InventoryType, Item[]> _items { get; set; }

        public byte MaxSlots { get; set; }
        public byte TotalSlotsUsed { get; set; } = 0;
        public int Mesos { get; set; }


        public CharacterStorage(GameCharacter chr)
        {
            Character = chr;
        }

        public void Load()
        {
            bool initStorage = false;
            using (var data = (MySqlDataReader)Server.Instance.CharacterDatabase.RunQuery(
                    "SELECT slots, mesos FROM storage WHERE userid = @userid AND world_id = @worldid",
                    "@userid", Character.UserID,
                    "@worldid", Server.Instance.WorldID
            ))
            {

                if (data.Read() == false)
                {
                    MaxSlots = 4;
                    Mesos = 0;
                    initStorage = true;
                }
                else
                {
                    MaxSlots = (byte)data.GetInt16(0);
                    Mesos = data.GetInt32(1);
                }
            }

            if (initStorage)
            {
                Server.Instance.CharacterDatabase.RunQuery(
                    "INSERT INTO storage (userid, world_id) VALUES (@userid, @worldid)",
                    "@userid", Character.UserID,
                    "@worldid", Server.Instance.WorldID
                );
            }

            _items = new Dictionary<InventoryType, Item[]>();
            SetSlots(MaxSlots);

            SplitDBInventory.Load(
                Server.Instance.CharacterDatabase,
                "storage",
                $"userid = {Character.UserID} AND world_id = {Server.Instance.WorldID}",
                (type, inventory, slot, item) =>
                {
                    AddItem(item);
                }
           );

        }

        public void Save()
        {
            int userId = Character.UserID;
            int worldId = Server.Instance.WorldID;

            Server.Instance.CharacterDatabase.RunQuery(
                "UPDATE storage SET " +
                "slots = " + MaxSlots + ", " +
                "mesos = " + Mesos + " " +
                "WHERE " +
                "userid = " + userId + " AND " +
                "world_id = " + worldId);

            short slot = 0;
            foreach (InventoryType inventory in Enum.GetValues(typeof(InventoryType)))
            {
                for (var j = 0; j < MaxSlots; j++)
                {
                    var x = _items[inventory][j];
                    if (x != null) x.InventorySlot = slot++;
                }
            }

            SplitDBInventory.Save(
                Server.Instance.CharacterDatabase,
                "storage",
                $"{userId}, {worldId},",
                $"userid = {userId} AND world_id = {worldId}",
                (type, inventory) =>
                {
                    InventoryType inv = (InventoryType)inventory;
                    if (inv == InventoryType.Cash) return new List<Item>();
                    return GetInventoryItems(inv);
                },
                Program.MainForm.LogAppend
            );

        }

        public bool AddItem(Item item)
        {
            InventoryType inv = Constants.getInventory(item.ItemID);
            var items = _items[inv];
            // Find first empty slot
            for (var i = 0; i < MaxSlots; i++)
            {
                if (items[i] != null) continue;
                TotalSlotsUsed++;
                items[i] = item;
                return true;
            }

            // No slot found
            return false;
        }

        public IEnumerable<Item> GetInventoryItems(InventoryType inv)
        {
            return _items[inv].Where(x => x != null && Constants.getInventory(x.ItemID) == inv);
        }

        public void TakeItemOut(InventoryType inv, byte slot)
        {
            var items = _items[inv];
            var tmp = new Item[MaxSlots];
            var tmpOffset = 0;

            for (var i = 0; i < MaxSlots; i++)
            {
                if (i == slot)
                {
                    TotalSlotsUsed--;
                    continue;
                }

                tmp[tmpOffset++] = items[i];
            }

            _items[inv] = tmp;
        }

        public Item GetItem(InventoryType inv, byte slot)
        {
            if (slot >= MaxSlots) return null;
            return _items[inv][slot];
        }

        public bool SetSlots(byte amount)
        {
            if (amount < 4) amount = 4;
            else if (amount > 100) amount = 100;

            // Already used more slots than what we have
            if (amount < TotalSlotsUsed)
                return false;

            MaxSlots = amount;

            foreach (InventoryType inventory in Enum.GetValues(typeof(InventoryType)))
            {
                if (!_items.ContainsKey(inventory))
                {
                    _items.Add(inventory, new Item[MaxSlots]);
                }
                else
                {
                    var items = _items[inventory];
                    Array.Resize(ref items, MaxSlots);
                    _items[inventory] = items;
                }
            }

            return true;
        }

        public bool SlotsAvailable()
        {
            return MaxSlots - TotalSlotsUsed > 0;
        }


        public void ChangeMesos(int value)
        {
            int newMesos = 0;
            if (value < 0)
            { //if value is less than zero 
                if ((Mesos - value) < 0) newMesos = 0;
                else newMesos = Mesos - value; // neg - neg = pos
            }
            else
            {
                if ((long)(Mesos + value) > int.MaxValue) newMesos = int.MaxValue;
                else newMesos = Mesos - value; //this was the little fucker that fucked everything up
            }
            Mesos = newMesos;

            StoragePacket.SendChangedMesos(Character);
        }

    }
}
