using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using reNX;
using reNX.NXProperties;
using WvsBeta.Common.Enums;
using WvsBeta.Common.Objects;
#if !DEBUG
using System.Threading.Tasks;
#endif

namespace WvsBeta.Common.DataProviders
{
    public abstract class DataProvider
    {
        public static IDictionary<int, EquipData> Equips { get; protected set; }
        public static IDictionary<int, ItemData> Items { get; protected set; }
        public static IDictionary<int, PetData> Pets { get;
        protected set; }
        public static IDictionary<int, SkillData> Skills { get; protected set; }
        public static IDictionary<short, WZQuestData> Quests { get; protected set; }

        protected static List<NXFile> pOverride;
        protected static NXFile pClientFile;
        private static DateTime startTime;

        public static KeyValuePair<NXFile, List<NXFile>> GetMergedDatafiles()
        {
            var mainFile = new NXFile(Path.Combine(ConfigReader.DataSvrPath, "ClientData.nx"));
            var overrideFolder = Path.Combine(ConfigReader.DataSvrPath, "data");
            var otherFiles = new List<NXFile>();
            if (Directory.Exists(overrideFolder))
            {
                otherFiles.AddRange(Directory
                    .GetFiles(overrideFolder)
                    .Where(f => f.EndsWith(".nx"))
                    .Select(nxPath => new NXFile(nxPath))
                );

                foreach (var nxFile in otherFiles)
                {
                    Console.WriteLine("Importing " + nxFile.FilePath);
                    mainFile.Import(nxFile);
                }
            }

            return new KeyValuePair<NXFile, List<NXFile>>(mainFile, otherFiles);
        }

        protected static void StartInit()
        {
            startTime = DateTime.Now;
            var x = GetMergedDatafiles();
            pClientFile = x.Key;
            pOverride = new List<NXFile>();
            pOverride.AddRange(x.Value);
        }

        protected static void FinishInit()
        {
            Trace.WriteLine("Finished loading all WZ data in " + (DateTime.Now - startTime).TotalMilliseconds + " ms");
            pOverride.ForEach(x => x.Dispose());
            pOverride.Clear();
            pOverride = null;
            pClientFile.Dispose();
            pClientFile = null;

            // do some cleanup
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();
        }

        public static void LoadBase()
        {
            Action[] actions =
            {
            ReadEquips,
            ReadItems,
            ReadPets,
        };


#if DEBUG
            foreach (var act in actions)
            {
                act();
            }
#else
        Task.WaitAll(actions.Select(x => Task.Factory.StartNew(x)).ToArray());
#endif
            ReadItemNames();
        }

        protected static void IterateAll<T>(IEnumerable<T> elements, Action<T> func)
        {
#if DEBUG
            foreach (var nxNode in elements)
            {
                func(nxNode);
            }
#else
        Parallel.ForEach(elements, func);
#endif
        }

        /// <summary>
        /// Iterate over each element (Parallel in non-debug mode) and make a Dictionary as result
        /// </summary>
        /// <typeparam name="TIn">Input type of iterator</typeparam>
        /// <typeparam name="TOut">Iterations output type</typeparam>
        /// <typeparam name="TKey">Dictionary Key type</typeparam>
        /// <param name="elements">Elements to iterate over</param>
        /// <param name="func">Function to run per iteration</param>
        /// <param name="outToKey">Function to convert the output to a key</param>
        /// <returns>A regular Dictionary with the key/value pairs</returns>
        protected static Dictionary<TKey, TOut> IterateAllToDict<TIn, TOut, TKey>(
            IEnumerable<TIn> elements,
            Func<TIn, TOut> func,
            Func<TOut, TKey> outToKey) =>
            IterateAllToDict(elements, func, outToKey, x => x);

        protected static Dictionary<TKey, TVal> IterateAllToDict<TIn, TOut, TKey, TVal>(IEnumerable<TIn> elements, Func<TIn, TOut> func, Func<TOut, TKey> outToKey, Func<TOut, TVal> outToVal)
        {
#if DEBUG
            var dict = new Dictionary<TKey, TVal>();
            foreach (var nxNode in elements)
            {
                var ret = func(nxNode);
                dict[outToKey(ret)] = outToVal(ret);
            }
            return dict;
#else
        return elements.AsParallel().Select(x => func(x)).ToDictionary(outToKey, outToVal);
#endif
        }

        protected static void ReadEquips()
        {
            var equips =
                from category in pClientFile.BaseNode["Character"]
                where category.Name.EndsWith(".img") == false && category.Name != "Afterimage"
                from item in category
                select new { category.Name, item };

            Equips = IterateAllToDict(equips, p =>
            {
                EquipData eq = new EquipData();
                var infoBlock = p.item["info"];
                eq.ID = (int)Utils.ConvertNameToID(p.item.Name);

                foreach (var nxNode in infoBlock)
                {
                    switch (nxNode.Name)
                    {
                        case "islot":
                        case "vslot":
                        case "icon":
                        case "iconRaw":
                        case "afterImage":
                        case "sfx":
                        case "attack":
                        case "stand":
                        case "walk":
                        case "sample":
                        case "chatBalloon":
                        case "nameTag":
                            break;

                        // Nexon typos (= not used!)
                        case "incMMD": // Green Jester, would've been a really good buff!
                        case "regPOP": // Dark Lucida (Female)
                            break;

                        case "tuc": eq.Slots = nxNode.ValueByte(); break;
                        case "reqLevel": eq.RequiredLevel = nxNode.ValueByte(); break;
                        case "reqPOP": eq.RequiredFame = nxNode.ValueByte(); break;
                        case "reqDEX": eq.RequiredDexterity = nxNode.ValueUInt16(); break;
                        case "reqINT": eq.RequiredIntellect = nxNode.ValueUInt16(); break;
                        case "reqLUK": eq.RequiredLuck = nxNode.ValueUInt16(); break;
                        case "reqSTR": eq.RequiredStrength = nxNode.ValueUInt16(); break;
                        case "reqJob":
                            {
                                var job = nxNode.ValueInt64();
                                // Sake Bottle and Tuna patch; ingame all jobs are required and the flag is -1..

                                if (job == -1) eq.RequiredJob = 0xFFFF;
                                else eq.RequiredJob = (ushort)job;
                                break;
                            }
                        case "price": eq.Price = nxNode.ValueInt32(); break;
                        case "incSTR": eq.Strength = nxNode.ValueInt16(); break;
                        case "incDEX": eq.Dexterity = nxNode.ValueInt16(); break;
                        case "incLUK": eq.Luck = nxNode.ValueInt16(); ; break;
                        case "incINT": eq.Intellect = nxNode.ValueInt16(); break;
                        case "incMDD": eq.MagicDefense = nxNode.ValueByte(); break;
                        case "incPDD": eq.WeaponDefense = nxNode.ValueByte(); break;
                        case "incPAD": eq.WeaponAttack = nxNode.ValueByte(); break;
                        case "incMAD": eq.MagicAttack = nxNode.ValueByte(); break;
                        case "incSpeed": eq.Speed = nxNode.ValueByte(); break;
                        case "incJump": eq.Jump = nxNode.ValueByte(); break;
                        case "incACC": eq.Accuracy = nxNode.ValueByte(); break;
                        case "incEVA": eq.Avoidance = nxNode.ValueByte(); break;
                        case "incMHP": eq.HP = nxNode.ValueInt16(); break;
                        case "incMMP": eq.MP = nxNode.ValueInt16(); break;
                        case "quest":
                            eq.IsQuest = nxNode.ValueBool();
                            break;
                        case "only":
                            if (nxNode.ValueBool())
                            {
                                eq.IsOnly = true;
                            }
                            break;
                        case "tradeBlock":
                            {
                                eq.IsTradeBlock = nxNode.ValueBool();
                                break;
                            }
                        case "cash": eq.Cash = nxNode.ValueBool(); break;
                        case "attackSpeed": eq.AttackSpeed = nxNode.ValueByte(); break;
                        case "knockback": eq.KnockbackRate = nxNode.ValueByte(); break;
                        case "timeLimited": eq.TimeLimited = nxNode.ValueBool(); break;
                        case "recovery": eq.RecoveryRate = nxNode.ValueFloat(); break;
                        default:
                            Console.WriteLine($"Unhandled node {nxNode.Name} for equip {eq.ID}");
                            break;
                    }
                }
                return eq;
            }, x => x.ID, x => x);
        }

        private static NXNode ReadItemString(int itemID)
        {
            var inventory = Constants.getInventory(itemID);
            var itemType = Constants.getItemType(itemID);
            string itemCat = null;
            switch (inventory)
            {
                case InventoryType.Equip:
                    itemCat = "Eqp";
                    switch (itemType)
                    {
                        case 101:
                            itemCat += "/Accessory";
                            break;
                        case 100:
                            itemCat += "/Cap";
                            break;
                        case 110:
                            itemCat += "/Cape";
                            break;
                        case 104:
                            itemCat += "/Coat";
                            break;
                        case 200:
                            itemCat += "/Face";
                            break;
                        case 108:
                            itemCat += "/Glove";
                            break;
                        case 300:
                            itemCat += "/Hair";
                            break;
                        case 105:
                            itemCat += "/Longcoat";
                            break;
                        case 106:
                            itemCat += "/Pants";
                            break;
                        case 180:
                            itemCat += "/PetEquip";
                            break;
                        case 111:
                            itemCat += "/Ring";
                            break;
                        case 109:
                            itemCat += "/Shield";
                            break;
                        case 107:
                            itemCat += "/Shoes";
                            break;
                        case 130:
                            itemCat += "/Weapon";
                            break;
                        default:
                            break;
                    }
                    break;
                case InventoryType.Use:
                    itemCat = "Con";
                    break;
                case InventoryType.Setup:
                    itemCat = "Ins";
                    break;
                case InventoryType.Etc:
                    itemCat = "Etc";
                    break;
                case InventoryType.Cash:
                    if (itemType == 500)
                        itemCat = "Pet";
                    else
                        itemCat = "Cash";
                    break;
                default:
                    return null;
            }
            try
            {
                NXNode stringNode = pClientFile.ResolvePath(string.Format("String/Item.img/{0}/{1}", itemCat, itemID));
                return stringNode;
            }
            catch
            {
                Console.WriteLine("Missing strings for item {0}", itemID);
                return null;
            }
        }
        protected static void ReadItems()
        {
            var items =
                from category in pClientFile.BaseNode["Item"]
                where category.Name != "Pet"
                from itemType in category
                from item in itemType
                select item;

            Items = IterateAllToDict(items, p =>
            {
                var iNode = p;
                ItemData item = new ItemData();
                int ID = (int)Utils.ConvertNameToID(iNode.Name);
                item.ID = ID;

                NXNode itemString = ReadItemString(ID);

                if (iNode.ContainsChild("info"))
                {
                    var infoNode = iNode["info"];
                    foreach (var node in infoNode)
                    {
                        switch (node.Name)
                        {
                            case "unitPrice":
                                item.UnitPrice = node.ValueFloat();
                                break;
                            case "path":
                            case "floatType":
                            case "icon":
                            case "iconRaw":
                            case "iconReward": break;

                            case "type":
                                item.Type = node.ValueInt8();
                                break;
                            case "price":
                                item.Price = node.ValueInt32();
                                break;
                            case "timeLimited":
                                item.TimeLimited = node.ValueBool();
                                break;
                            case "cash":
                                item.Cash = node.ValueBool();
                                break;
                            case "slotMax":
                                item.MaxSlot = node.ValueUInt16();
                                break;
                            case "meso":
                                item.Mesos = node.ValueInt32();
                                break;
                            case "quest":
                                item.IsQuest = node.ValueBool();
                                break;
                            case "success":
                                item.ScrollSuccessRate = node.ValueByte();
                                break;
                            case "cursed":
                                item.ScrollCurseRate = node.ValueByte();
                                break;
                            case "incSTR":
                                item.IncStr = node.ValueByte();
                                break;
                            case "incDEX":
                                item.IncDex = node.ValueByte();
                                break;
                            case "incLUK":
                                item.IncLuk = node.ValueByte();
                                break;
                            case "incINT":
                                item.IncInt = node.ValueByte();
                                break;
                            case "incMHP":
                                item.IncMHP = node.ValueByte();
                                break;
                            case "incMMP":
                                item.IncMMP = node.ValueByte();
                                break;
                            case "pad":
                            case "incPAD":
                                item.IncWAtk = node.ValueByte();
                                break;
                            case "incMAD":
                                item.IncMAtk = node.ValueByte();
                                break;
                            case "incPDD":
                                item.IncWDef = node.ValueByte();
                                break;
                            case "incMDD":
                                item.IncMDef = node.ValueByte();
                                break;
                            case "incACC":
                                item.IncAcc = node.ValueByte();
                                break;
                            case "incEVA":
                                item.IncAvo = node.ValueByte();
                                break;
                            case "incJump":
                                item.IncJump = node.ValueByte();
                                break;
                            case "incSpeed":
                                item.IncSpeed = node.ValueByte();
                                break;
                            case "rate":
                                item.Rate = node.ValueByte();
                                break;
                            case "bigSize":
                                item.BigSize = node.ValueBool();
                                break;
                            case "only":
                                item.IsOnly = node.ValueBool();
                                break;
                            case "tradeBlock":
                                item.IsTradeBlock = node.ValueBool();
                                break;
                            case "time":
                                item.RateTimes = new Dictionary<byte, List<KeyValuePair<byte, byte>>>();
                                foreach (var lNode in node)
                                {
                                    string val = lNode.ValueString();
                                    string day = val.Substring(0, 3);
                                    byte hourStart = byte.Parse(val.Substring(4, 2));
                                    byte hourEnd = byte.Parse(val.Substring(7, 2));
                                    byte dayid = 0;

                                    switch (day)
                                    {
                                        case "MON": dayid = 0; break;
                                        case "TUE": dayid = 1; break;
                                        case "WED": dayid = 2; break;
                                        case "THU": dayid = 3; break;
                                        case "FRI": dayid = 4; break;
                                        case "SAT": dayid = 5; break;
                                        case "SUN": dayid = 6; break;
                                        case "HOL": dayid = ItemData.HOLIDAY_DAY; break;
                                    }
                                    if (!item.RateTimes.ContainsKey(dayid))
                                        item.RateTimes.Add(dayid, new List<KeyValuePair<byte, byte>>());

                                    item.RateTimes[dayid].Add(new KeyValuePair<byte, byte>(hourStart, hourEnd));
                                }
                                break;
                            default:
                                Console.WriteLine($"Unhandled item info node {node.Name} for id {item.ID}");
                                break;

                        }
                    }
                }
                else
                {
                    item.Price = 0;
                    item.Cash = false;
                    item.MaxSlot = 1;
                    item.Mesos = 0;
                    item.IsQuest = false;

                    item.ScrollSuccessRate = 0;
                    item.ScrollCurseRate = 0;
                    item.IncStr = 0;
                    item.IncDex = 0;
                    item.IncInt = 0;
                    item.IncLuk = 0;
                    item.IncMHP = 0;
                    item.IncMMP = 0;
                    item.IncWAtk = 0;
                    item.IncMAtk = 0;
                    item.IncWDef = 0;
                    item.IncMDef = 0;
                    item.IncAcc = 0;
                    item.IncAvo = 0;
                    item.IncJump = 0;
                    item.IncSpeed = 0;
                    item.Rate = 0;
                }
                if (iNode.ContainsChild("spec"))
                {
                    var specNode = iNode["spec"];
                    foreach (var node in specNode)
                    {
                        switch (node.Name)
                        {
                            case "moveTo":
                                item.MoveTo = node.ValueInt32();
                                break;
                            case "hp":
                                item.HP = node.ValueInt16();
                                break;
                            case "mp":
                                item.MP = node.ValueInt16();
                                break;
                            case "hpR":
                                item.HPRate = node.ValueInt16();
                                break;
                            case "mpR":
                                item.MPRate = node.ValueInt16();
                                break;
                            case "speed":
                                item.Speed = node.ValueInt16();
                                break;
                            case "eva":
                                item.Avoidance = node.ValueInt16();
                                break;
                            case "acc":
                                item.Accuracy = node.ValueInt16();
                                break;
                            case "mad":
                                item.MagicAttack = node.ValueInt16();
                                break;
                            case "pad":
                                item.WeaponAttack = node.ValueInt16();
                                break;
                            case "pdd":
                                item.WeaponDefense = node.ValueInt16();
                                break;
                            case "thaw":
                                item.Thaw = node.ValueInt16();
                                break;
                            case "time":
                                item.BuffTime = node.ValueInt32();
                                break;

                            case "curse":
                            case "darkness":
                            case "poison":
                            case "seal":
                            case "weakness":
                                if (node.ValueInt64() != 0)
                                {
                                    CureFlag flag = 0;
                                    switch (node.Name)
                                    {
                                        case "curse":
                                            flag = CureFlag.Curse;
                                            break;
                                        case "darkness":
                                            flag = CureFlag.Darkness;
                                            break;
                                        case "poison":
                                            flag = CureFlag.Poison;
                                            break;
                                        case "seal":
                                            flag = CureFlag.Seal;
                                            break;
                                        case "weakness":
                                            flag = CureFlag.Weakness;
                                            break;
                                    }
                                    item.Cures |= flag;
                                }
                                break;
                            default:
                                Console.WriteLine($"Unhandled item spec node {node.Name} for id {item.ID}");
                                break;
                        }
                    }
                }
                else
                {
                    //no spec, continue
                    item.MoveTo = 0;
                    item.HP = 0;
                    item.MP = 0;
                    item.HPRate = 0;
                    item.MPRate = 0;
                    item.Speed = 0;
                    item.Avoidance = 0;
                    item.Accuracy = 0;
                    item.MagicAttack = 0;
                    item.WeaponAttack = 0;
                    item.BuffTime = 0;
                }


                if (iNode.ContainsChild("mob")) //summons
                {
                    item.Summons = new List<ItemSummonInfo>();

                    foreach (var sNode in iNode["mob"])
                    {
                        item.Summons.Add(new ItemSummonInfo
                        {
                            MobID = sNode["id"].ValueInt32(),
                            Chance = sNode["prob"].ValueByte()
                        });
                    }

                }
                return item;
            }, x => x.ID);

        }

        protected static void ReadPets()
        {
            Pets = IterateAllToDict(pClientFile.BaseNode["Item"]["Pet"], pNode =>
            {
                var pd = new PetData(pNode);
                return pd;
            }, x => x.ID);
        }


        private static void ProcessNames(NXNode listNode, Action<int, string> handleName)
        {
            foreach (var item in listNode)
            {
                if (int.TryParse(item.Name, out var itemId))
                {
                    if (item.ContainsChild("name"))
                    {
                        handleName(itemId, item["name"].ValueString());
                    }
                    else
                    {
                        Trace.WriteLine($"Item {itemId} does not contain 'name' node.");
                    }
                }
                else
                {
                    Trace.WriteLine($"Node {item.Name} does not have a valid itemid as name!?");
                }

            }
        }

        public static void ReadItemNames()
        {

            foreach (var node in pClientFile.BaseNode["String"]["Item.img"])
            {
                if (node.Name == "Eqp")
                {
                    foreach (var cat in node)
                    {
                        ProcessNames(cat, (i, s) =>
                        {
                            if (!Equips.ContainsKey(i))
                                Trace.WriteLine($"Found name {s} for equip {i}, but equip did not exist!");
                            else
                                Equips[i].Name = s;
                        });
                    }
                }
                else if (node.Name == "Pet")
                {
                    ProcessNames(node, (i, s) =>
                    {
                        if (!Pets.ContainsKey(i))
                            Trace.WriteLine($"Found name {s} for pet {i}, but pet did not exist!");
                        else
                            Pets[i].Name = s;
                    });
                }
                else
                {
                    ProcessNames(node, (i, s) =>
                    {
                        if (!Items.ContainsKey(i))
                            Trace.WriteLine($"Found name {s} for item {i}, but item did not exist!");
                        else
                            Items[i].Name = s;
                    });
                }
            }
        }
    }
}