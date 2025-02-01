﻿using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using WvsBeta.Common;
using WvsBeta.Common.Sessions;
using WvsBeta.Database;
using WvsBeta.Common.Objects;
using WvsBeta.Common.Characters;
using WvsBeta.Shop.GameObjects;
using WvsBeta.Common.Extensions;

namespace WvsBeta.Shop
{
    class Server
    {
        public static Server Instance { get; private set; }
        public static bool Tespia { get; private set; }
        public string Name { get; private set; }
        public string WorldName { get; private set; }
        public byte WorldID { get; private set; }

        public int CenterPort { get; set; }
        public IPAddress CenterIP { get; set; }
        public bool CenterMigration { get; set; }

        public ushort Port { get; private set; }
        public IPAddress PublicIP { get; private set; }
        public IPAddress PrivateIP { get; private set; }

        public CenterSocket CenterConnection { get; set; }

        private ShopAcceptor ShopAcceptor { get; set; }
        public MySQL_Connection CharacterDatabase { get; private set; }

        public Dictionary<string, Player> PlayerList { get; } = new Dictionary<string, Player>();
        public Dictionary<int, ShopCharacter> CharacterList { get; } = new Dictionary<int, ShopCharacter>();
        public Dictionary<int, Packet> CCIngPlayerList { get; } = new Dictionary<int, Packet>();

        public int GetOnlineId() => RedisBackend.GetOnlineId(WorldID, 50);

        private string ConfigFilePath { get; set; }

        public IDictionary<CommodityCategory, IList<CommodityInfo>> BestItems { get; } = new Dictionary<CommodityCategory, IList<CommodityInfo>>();
        public List<ShopTransaction> Transactions { get; } = new List<ShopTransaction>();

        public void LogToLogfile(string what)
        {
            Program.LogFile.WriteLine(what);
        }

        public void AddPlayer(Player player)
        {
            string hash = Cryptos.GetNewSessionHash();
            while (PlayerList.ContainsKey(hash))
            {
                hash = Cryptos.GetNewSessionHash();
            }
            PlayerList[hash] = player;
            player.SessionHash = hash;
        }

        public void RemovePlayer(string hash)
        {
            PlayerList.Remove(hash);
        }

        public ShopCharacter GetCharacter(int ID)
        {
            if (CharacterList.TryGetValue(ID, out ShopCharacter chr)) return chr;
            return null;
        }

        public ShopCharacter GetCharacter(string name)
        {
            name = name.ToLowerInvariant();
            foreach (var kvp in CharacterList)
            {
                if (kvp.Value != null && kvp.Value.Name.ToLowerInvariant() == name)
                {
                    return kvp.Value;
                }
            }
            return null;
        }

        public bool IsPlayer(string hash)
        {
            return PlayerList.ContainsKey(hash);
        }

        public Player GetPlayer(string hash)
        {
            if (PlayerList.TryGetValue(hash, out Player player)) return player;
            return null;
        }

        public static void Init(string configFile)
        {
            Instance = new Server()
            {
                Name = configFile
            };
            Instance.Load();
        }

        public void ConnectToCenter()
        {
            if (CenterConnection?.Disconnected == false) return;
            CenterConnection = new CenterSocket();
        }

        public void Load()
        {
            ConfigFilePath = Path.Combine(ConfigReader.DataSvrPath, Name + ".img");
            LoadConfig(ConfigFilePath);
            LoadDBConfig(Path.Combine(ConfigReader.DataSvrPath, "Database.img"));

            ConnectToCenter();

            ShopAcceptor = new ShopAcceptor();

            CharacterDatabase.SetupPinger(MasterThread.Instance);
        }

        private void LoadDBConfig(string configFile)
        {
            var lines = File.ReadLines(configFile)
                .Select(l => l.Split(' '))
                .Select(p => p.Length == 2 ? "" : p[2])
                .ToList();
            string Username = lines[0];
            string Password = lines[1];
            string Database = lines[2];
            string Host = lines[3];

            CharacterDatabase = new MySQL_Connection(MasterThread.Instance, Username, Password, Database, Host);

            CharacterCashItems.Connection = CharacterDatabase;
            Inventory.Connection = CharacterDatabase;
        }

        private void LoadConfig(string configFile)
        {
            ConfigReader reader = new ConfigReader(configFile);

            Port = reader["port"].GetUShort();
            WorldID = reader["gameWorldId"].GetByte();

            CharacterCashItems.WorldID = WorldID;
            log4net.GlobalContext.Properties["WorldID"] = WorldID;

            Tespia = reader["tespia"]?.GetBool() ?? false;

            PublicIP = IPAddress.Parse(reader["PublicIP"].GetString());
            PrivateIP = IPAddress.Parse(reader["PrivateIP"].GetString());

            CenterIP = IPAddress.Parse(reader["center"]["ip"].GetString());
            CenterPort = reader["center"]["port"].GetUShort();
            WorldName = reader["center"]["worldName"].GetString();

            RedisBackend.Init(reader);
        }

        public void LoadCashShopData()
        {
            BestItems.Clear();
            var sales = new Dictionary<CommodityCategory, IList<CommodityInfo>>();
            using (var data = (MySqlDataReader)Server.Instance.CharacterDatabase.RunQuery("SELECT sn, COUNT(sn) AS snc FROM user_point_transactions WHERE sn > 0 GROUP BY sn ORDER BY snc DESC"))
            {
                while (data.Read())
                {
                    var sn = data.GetInt32("sn");
                    if (ShopDataProvider.Commodity.TryGetValue(sn, out CommodityInfo ci))
                    {
                        sales.SafeAdd(ci.Category, ci);
                    }
                }
            }
            foreach (CommodityCategory cat in CommodityExtensions.GetCategories())
            {
                if (!sales.ContainsKey(cat)) continue;
                var top = sales[cat].Take(5).ToList();
                BestItems.Add(cat, top);
            }
        }

        public void StartListening()
        {
            Program.MainForm.LogAppend($"Starting to listen on port {Port}");
            ShopAcceptor = new ShopAcceptor();
        }

        public void StopListening()
        {
            Program.MainForm.LogAppend($"Stopped listening on port {Port}");
            ShopAcceptor?.Stop();
            ShopAcceptor = null;
        }
    }
}