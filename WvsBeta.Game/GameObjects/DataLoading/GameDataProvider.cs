using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using reNX;
using reNX.NXProperties;
using WvsBeta.Common;
using WvsBeta.Common.DataProviders;
using WvsBeta.Common.Enums;
using WvsBeta.Common.Extensions;
using WvsBeta.Common.Objects;
using WvsBeta.Common.WzObjects;
using WvsBeta.Game.Events.GMEvents;
using WvsBeta.Game.GameObjects;
using WvsBeta.Game.GameObjects.DataLoading;
#if !DEBUG
using System.Threading.Tasks;
#endif

// if \(.*Node.ContainsChild\((.*)\)\)[\s\r\n]+\{[\s\r\n]+(.*)\r\n[\s\r\n]+\}
// case $1: $2 break;
namespace WvsBeta.Game
{
    public class GameDataProvider : DataProvider
    {
        public static IDictionary<int, Map> Maps { get; private set; }
        public static IDictionary<int, Reactor> Reactors { get; private set; }
        public static IDictionary<string, ReactorActionData> ReactorActions { get; private set; }
        public static IDictionary<int, NPCData> NPCs { get; private set; }
        public static IDictionary<int, MobData> Mobs { get; private set; }
        public static List<int> Jobs { get; private set; }
        public static IDictionary<byte, Dictionary<byte, MobSkillLevelData>> MobSkills { get; private set; }
        public static IDictionary<string, DropData[]> Drops { get; private set; }
        public static Dictionary<byte, List<QuizData>> QuizQuestions { get; } = new Dictionary<byte, List<QuizData>>();
        public static IDictionary<int, HashSet<short>> QuestItems { get; } = new Dictionary<int, HashSet<short>>();
        private static NXFile pServerFile;


        public static void Load()
        {
            StartInit();

            var dataPath = Path.Combine(Environment.CurrentDirectory, "..", "DataSvr");
            using (pServerFile = new NXFile(Path.Combine(dataPath, "ServerData.nx"), NXReadSelection.None))
            {
                var funcs = new Action[] {
                    LoadBase,
                    ReadSkills,
                    ReadMobData,
                    ReadReactors,
                    ReadReactorActions,
                    ReadNpcs,
                    ReadNpcShops,
                    ReadMapData,
                    ReadFieldSetData,
                    ReadDrops,
                    ReadQuiz,
                    ReadQuestData
                };

                foreach (var func in funcs)
                {
                    func();
                }

                // Cleanup the drops from nonexistant items and droppers
                
                foreach (var kvp in Drops.ToList())
                {
                    if (kvp.Key.StartsWith("m"))
                    {
                        var mobId = int.Parse(kvp.Key.Substring(1));
                        if (!Mobs.ContainsKey(mobId))
                        {
                            //Program.MainForm.LogAppend($"Removing nonexistant {mobId} mob from drops");
                            Drops.Remove(kvp.Key);
                            continue;
                        }
                    }

                    Drops[kvp.Key] = kvp.Value.Where(x =>
                    {
                        var itemId = x.ItemID;
                        if (itemId == 0) return true;

                        if (Constants.isEquip(itemId)
                            ? Equips.ContainsKey(itemId)
                            : Items.ContainsKey(itemId)) return true;

                        //Program.MainForm.LogAppend($"Removing item {itemId} because it doesnt exist, from {kvp.Key}");
                        return false;
                    }).ToArray();
                }

                // Cleanup map life

                foreach (var map in Maps.Values)
                {
                    var mg = map.MobGen.ToList();
                    foreach (var mgi in mg)
                    {
                        if (Mobs.ContainsKey(mgi.ID)) continue;
                        Console.WriteLine($"Removing mob {mgi.ID} from map {map.ID}, as it does not exist");
                        map.MobGen.Remove(mgi);
                    }

                    var npcs = map.NPC.ToList();
                    foreach (var npc in npcs)
                    {
                        if (NPCs.TryGetValue(npc.ID, out NPCData npcData))
                        {
                            npcData.IsAccessible = true;
                            continue;
                        }
                        Console.WriteLine($"Removing NPC {npc.ID} from map {map.ID}, as it does not exist");
                        map.NPC.Remove(npc);
                    }

                    foreach (var portal in map.Portals)
                    {
                        if (portal.Value.ToMapID == Constants.InvalidMap) continue;
                        if (!Maps.TryGetValue(portal.Value.ToMapID, out var otherMap))
                        {
                            Console.WriteLine($"Portal {portal.Key} in map {map.ID} points to an unknown map ({portal.Value.ToMapID})");
                        }
                        else if (!otherMap.Portals.ContainsKey(portal.Value.ToName))
                        {
                            Console.WriteLine($"Portal {portal.Key} in map {map.ID} points to an unknown portal (in other map) ({portal.Value.ToMapID}, {portal.Value.ToName})");
                        }
                    }

                    // Normal GMS would start spawning here. We don't care.
                }

                foreach (var mob in Mobs.Values)
                {
                    if (!Drops.ContainsKey("m" + mob.ID))
                    {
                        Console.WriteLine($"Mob {mob.ID} does not have drops!");
                    }
                    if (mob.Skills != null)
                    {
                        foreach (var msd in mob.Skills)
                        {
                            if (!MobSkills.TryGetValue(msd.SkillID, out Dictionary<byte, MobSkillLevelData> levelDataMap) || !levelDataMap.ContainsKey(msd.Level))
                            {
                                Program.MainForm.LogAppend("Removing unknown mob skill {0} at level {1} from mob {2}.", msd.SkillID, msd.Level, mob.ID);
                                mob.Skills.Remove(msd);
                            }
                        }
                    }
                    if (mob.HPTagBgColor > 0 || mob.HPTagColor > 0)
                    {
                        string bPath = "UI/UIWindow.img/MobGage/";
                        if (!pClientFile.ContainsPath($"{bPath}Mob/{mob.ID}"))
                        {
                            Program.MainForm.LogAppend("Mob {0} is missing hp gauge icon! Removing...", mob.ID);
                        }
                        else if (!pClientFile.ContainsPath($"{bPath}backgrnd{(mob.HPTagColor == 1 ? "" : mob.HPTagColor.ToString())}"))
                        {
                            Program.MainForm.LogAppend("Mob {0} is missing hp gauge background {1}! Removing...", mob.ID, mob.HPTagColor);
                        }
                        else if (!pClientFile.ContainsPath($"{bPath}Gage/{mob.HPTagBgColor}"))
                        {
                            Program.MainForm.LogAppend("Mob {0} is missing hp gauge tag color {1}! Removing...", mob.ID, mob.HPTagBgColor);
                        }
                        else
                        {
                            continue;
                        }
                        mob.HPTagBgColor = 0;
                        mob.HPTagColor = 0;
                    }
                }

                foreach (var quest in Quests)
                {
                    foreach (var questStage in quest.Value.Stages)
                    {
                        // Add quest items and check non-existing items
                        var items = questStage.Value.Check.Items.Select(i => i.Value).ToList();
                        items.AddRange(questStage.Value.Act.Items);
                        foreach (var questItem in items)
                        {
                            bool isEquip = Constants.isEquip(questItem.ItemID);
                            bool isQuestItem = false;
                            if (isEquip && Equips.TryGetValue(questItem.ItemID, out EquipData equipData))
                            {
                                isQuestItem = equipData.IsQuest;
                            }
                            else if (!isEquip && Items.TryGetValue(questItem.ItemID, out ItemData itemData))
                            {
                                isQuestItem = itemData.IsQuest;
                            }
                            else
                            {
                                Program.MainForm.LogAppend("Unknown {3} {0} in quest {1} (stage {2})!", questItem.ItemID, quest.Key, (byte)questStage.Key, isEquip ? "equip" : "item");
                            }
                            if (isQuestItem)
                            {
                                QuestItems.SafeAdd(questItem.ItemID, quest.Key);
                            }
                        }

                        // Check non-existing NPCs
                        var npcID = questStage.Value.Check.NpcID;
                        if (npcID != 0 && !NPCs.ContainsKey(npcID))
                        {
                            Program.MainForm.LogAppend("Unknown NPC {0} in quest {1} (stage {2})!", npcID, quest.Key, (byte)questStage.Key);
                        }

                        // Check non-existing quest triggers
                        foreach (var trigger in questStage.Value.Check.Quests)
                        {
                            if (!Quests.ContainsKey(trigger.QuestID))
                            {
                                Program.MainForm.LogAppend("Unknown quest reference {0} in quest {1} (stage {2})!", trigger.QuestID, quest.Key, (byte)questStage.Key);
                            }
                        }
                    }
                }

                // Check and instantiate NPC scripts
                //foreach (var npc in NPCs.Values)
                //{
                //    if (string.IsNullOrWhiteSpace(npc.Quest) || !npc.IsAccessible) continue;
                //    ScriptAccessor.GetScript(Server.Instance, npc.Quest, (err) =>
                //    {
                //        Program.MainForm.LogAppend($"Failed to find NPC script {npc.Quest} for NPC {npc.ID}!");
                //    });
                //}

                // Read reactors info maps
                foreach (var reactor in Reactors.Values)
                {
                    var mapIds = Regex.Match(reactor.Info, @"^\d+((,\d+)*|\.+\d+)$"); // Map ids
                    if (mapIds.Success)
                    {
                        var mapRange = Regex.Match(reactor.Info, @"^(\d+)\.+(\d+)$"); // Range n..o
                        if (mapRange.Success)
                        {
                            int from = int.Parse(mapRange.Groups[1].Value);
                            int to = int.Parse(mapRange.Groups[2].Value);
                            for (int mapID = from; mapID <= to; mapID++)
                            {
                                var map = Maps[mapID];
                                reactor.InfoMaps.Add(map);
                            }
                        }
                        else // Set n[,o,p,q]
                        {
                            var match = Regex.Match(reactor.Info, @"\d+");
                            do
                            {
                                int mapID = int.Parse(match.Value);
                                if (Maps.TryGetValue(mapID, out var map))
                                {
                                    reactor.InfoMaps.Add(map);
                                }
                                match = match.NextMatch();
                            } while (match.Success);
                        }
                    }
                }
            }

            Console.WriteLine($"Maps: {Maps.Count}");
            Console.WriteLine($"Mobs: {Mobs.Count}");
            Console.WriteLine($"NPCs: {NPCs.Count}");
            Console.WriteLine($"Eqps: {Equips.Count}");
            Console.WriteLine($"Itms: {Items.Count}");
            Console.WriteLine($"Quest items: {QuestItems.Count}");

            FinishInit();
        }

        public static bool HasJob(short JobID)
        {
            if (Jobs.Contains(JobID))
                return true;
            return false;
        }
        static void ReadMobData()
        {
            var mobs = pClientFile.BaseNode["Mob"].ToList();


            byte GetGroupIdx(string str) => (byte)(str[str.Length - 1] - '1');

            Mobs = IterateAllToDict(mobs, pNode =>
            {
                var data = new MobData();

                data.ID = (int)Utils.ConvertNameToID(pNode.Name);

                string stringPath = "String/Mob.img/" + data.ID;
                if (!pClientFile.ContainsPath(stringPath))
                    Program.MainForm.LogAppend("Missing string for mob " + data.ID);
                else
                {
                    string name = pClientFile.ResolvePath(stringPath)["name"].ValueString();
                    data.Name = name;
                }

                var infoNode = pNode["info"];
                var nonInfoNodes = pNode;
                if (infoNode.ContainsChild("link"))
                {
                    var linkedMobId = infoNode["link"].ValueString();

                    Trace.WriteLine($"Mob {data.ID} has a link to mob {linkedMobId}");
                    linkedMobId += ".img";
                    nonInfoNodes = mobs.First(y => y.Name == linkedMobId);
                }


                data.Jumps = nonInfoNodes.ContainsChild("jump");

                foreach (NXNode node in nonInfoNodes)
                {
                    if (node.Name == "info") continue;
                    byte id = GetGroupIdx(node.Name);

                    data.Animations.Add(node.Name, new WzAnimation(node));

                    if (node.Name.StartsWith("attack"))
                    {
                        var mad = new MobAttackData();
                        mad.ID = id;

                        foreach (var subNode in node["info"])
                        {
                            switch (subNode.Name)
                            {
                                // Effects
                                case "ball":
                                case "hit":
                                case "bulletSpeed":
                                case "attackAfter":
                                case "effect":
                                case "effectAfter":
                                case "jumpAttack": break;

                                case "disease": mad.Disease = subNode.ValueUInt8(); break;
                                case "elemAttr": mad.ElemAttr = subNode.ValueString()[0]; break;
                                case "conMP": mad.MPConsume = subNode.ValueInt16(); break;
                                case "magic": mad.Magic = subNode.ValueBool(); break;
                                case "type": mad.Type = subNode.ValueByte(); break;
                                case "PADamage": mad.PADamage = subNode.ValueInt16(); break;
                                case "level": mad.SkillLevel = subNode.ValueByte(); break;
                                case "range":
                                    if (subNode.ContainsChild("lt"))
                                    {
                                        var lt = subNode["lt"].ValueOrDie<Point>();
                                        mad.RangeLTX = (short)lt.X;
                                        mad.RangeLTY = (short)lt.Y;
                                        var rb = subNode["rb"].ValueOrDie<Point>();
                                        mad.RangeRBX = (short)rb.X;
                                        mad.RangeRBY = (short)rb.Y;
                                    }
                                    else
                                    {
                                        mad.RangeR = subNode["r"].ValueInt16();
                                        var sp = subNode["sp"].ValueOrDie<Point>();
                                        mad.RangeSPX = (short)sp.X;
                                        mad.RangeSPY = (short)sp.Y;
                                    }
                                    break;
                                default:
                                    Console.WriteLine($"Did not handle attack info node {subNode.Name} of mob {data.ID}");
                                    break;
                            }
                        }
                        (data.Attacks = data.Attacks ?? new Dictionary<byte, MobAttackData>()).Add(id, mad);
                    }
                }


                foreach (var node in infoNode)
                {
                    switch (node.Name)
                    {
                        case "link": break;

                        case "level": data.Level = node.ValueByte(); break;
                        case "undead": data.Undead = node.ValueBool(); break;
                        case "bodyAttack": data.BodyAttack = node.ValueBool(); break;
                        case "summonType": data.SummonType = node.ValueByte(); break;
                        case "exp": data.EXP = node.ValueInt32(); break;
                        case "maxHP": data.MaxHP = node.ValueInt32(); break;
                        case "maxMP": data.MaxMP = node.ValueInt32(); break;
                        case "elemAttr": data.elemAttr = node.ValueString(); break;
                        case "PADamage": data.PAD = node.ValueInt32(); break;
                        case "PDDamage": data.PDD = node.ValueInt32(); break;
                        case "MADamage": data.MAD = node.ValueInt32(); break;
                        case "MDDamage": data.MDD = node.ValueInt32(); break;
                        case "eva": data.Eva = node.ValueInt32(); break;
                        case "pushed": data.Pushed = node.ValueBool(); break;
                        case "noregen": data.NoRegen = node.ValueBool(); break;
                        case "invincible": data.Invincible = node.ValueBool(); break;
                        case "selfDestruction": data.SelfDestruction = node.ValueBool(); break;
                        case "firstAttack": data.FirstAttack = node.ValueBool(); break;
                        case "acc": data.Acc = node.ValueInt32(); break;
                        case "publicReward": data.PublicReward = node.ValueBool(); break;
                        case "explosiveReward": data.ExplosiveReward = node.ValueBool(); break;
                        case "fs": data.FS = node.ValueFloat(); break;
                        case "flySpeed":
                        case "speed":
                            data.Flies = node.Name == "flySpeed";
                            data.Speed = node.ValueInt16();
                            break;
                        case "revive":
                            data.Revive = node.Select(x => x.ValueInt32()).ToList();
                            break;
                        case "skill":
                            data.Skills = new List<MobSkillData>();
                            foreach (var skillNode in node)
                            {
                                var mobSkillData = new MobSkillData(skillNode);
                                data.Skills.Add(mobSkillData);
                            }

                            break;
                        case "hpRecovery": data.HPRecoverAmount = node.ValueInt32(); break;
                        case "mpRecovery": data.MPRecoverAmount = node.ValueInt32(); break;
                        case "hpTagColor": data.HPTagColor = node.ValueByte(); break;
                        case "hpTagBgcolor": data.HPTagBgColor = node.ValueByte(); break;
                        case "boss": data.Boss = node.ValueBool(); break;
                        default:
                            Console.WriteLine($"Did not handle node {node.Name} of mob {data.ID}");
                            break;
                    }
                }

                return data;
            }, x => x.ID);
        }

        static void ReadMapData()
        {
            var maps =
                from region in pClientFile.BaseNode["Map"]["Map"]
                where region.Name.StartsWith("Map")
                from map in region
                where map.Name != "AreaCode.img" && map.Name.EndsWith(".img")
                select map;

            Maps = IterateAllToDict(maps, p =>
            {
                var mapNode = p;
                var infoNode = mapNode["info"];
                int ID = (int)Utils.ConvertNameToID(mapNode.Name);

                var fieldType = infoNode.ContainsChild("fieldType") ? infoNode["fieldType"].ValueByte() : 0;

                string stringConti = null;
                string mapName = null;
                if (ID < 100_000_000) stringConti = "maple";
                else if (mapNode.Name.StartsWith("1")) stringConti = "victoria";
                else if (mapNode.Name.StartsWith("2")) stringConti = "ossyria";
                else if (mapNode.Name.StartsWith("6")) stringConti = "weddingGL";
                else if (mapNode.Name.StartsWith("9")) stringConti = "etc";
                
                if (stringConti != null && pClientFile.BaseNode["String"]["Map.img"][stringConti].TryGetValue(ID.ToString(), out NXNode stringNode))
                {
                    mapName = stringNode["mapName"].ValueString();
                }
                else
                {
                    Program.MainForm.LogAppend("Missing strings for map " + ID);
                }

                Map map;
                switch (fieldType)
                {
                    case 8: // Ergoth boss map 990000900
                    case 7: // Snowball entry map 109060001
                    case 2: // Contimove 101000300
                    case 0:
                        map = new Map(ID);
                        break;
                    case 5: // OX Quiz 109020001
                        map = new Map_OXQuiz(ID);
                        break;
                    case 4: // Coconut harvest 109080000
                        map = new Map_Coconut(ID, p);
                        break;
                    case 1: // Snowball 109060000
                        map = new Map_Snowball(ID);
                        break;
                    case 6: // JQ maps and such
                        map = new Map_PersonalTimeLimit(ID);
                        break;
                    case 3:
                        map = new Map_Tournament(ID);
                        break; // Tournament 109070000
                    default:
                        throw new Exception($"Unknown FieldType found!!! {fieldType}");
                }

                map.HasClock = mapNode.ContainsChild("clock");
                map.Name = mapName;

                int VRLeft = 0, VRTop = 0, VRRight = 0, VRBottom = 0;
                foreach (var node in infoNode)
                {
                    switch (node.Name)
                    {
                        case "VRLeft":
                            VRLeft = node.ValueInt32();
                            break;
                        case "VRTop":
                            VRTop = node.ValueInt32();
                            break;
                        case "VRRight":
                            VRRight = node.ValueInt32();
                            break;
                        case "VRBottom":
                            VRBottom = node.ValueInt32();
                            break;

                        case "bgm":
                            string[] bgmPath = node.ValueString().Split('/');
                            string imgPath = bgmPath[0] + ".img";
                            string bgmName = bgmPath[1];
                            var sound = pClientFile.BaseNode["Sound"];
                            if (!sound.ContainsChild(imgPath) || !sound[imgPath].ContainsChild(bgmName))
                            {
                                Program.MainForm.LogAppend("Bgm {0} not found for map {1}", node.ValueString(), ID);
                            }
                            break;
                        case "snow": // ???? Only 1 map
                        case "rain": // ???? Only 1 map
                        case "moveLimit": // Unknown
                        case "mapMark":
                        case "version":
                        case "hideMinimap":
                        case "streetName":
                        case "mapName":
                        case "mapDesc": // Event description (can be empty)?
                        case "help": // More event help? map 101000200 has it
                        case "cloud": // Show clouds in front of the screen
                        case "fs": // Speed thing
                        case "fieldType":
                            break;

                        case "decHP":
                            map.DecreaseHP = node.ValueInt16();
                            break;
                        case "protectItem":
                            map.ProtectItem = node.ValueInt32();
                            break;
                        case "recovery":
                            var amount = node.ValueInt16();
                            // Negative decrease HP
                            map.DecreaseHP = (short)-amount;
                            break;

                        case "timeLimit":
                            map.TimeLimit = node.ValueInt16();
                            break;
                        case "forcedReturn":
                            map.ForcedReturn = node.ValueInt32();
                            break;
                        case "returnMap":
                            map.ReturnMap = node.ValueInt32();
                            break;
                        case "town":
                            map.Town = node.ValueBool();
                            break;
                        case "swim":
                            map.Swim = node.ValueBool();
                            break;
                        case "personalShop":
                            map.AcceptPersonalShop = node.ValueBool();
                            break;
                        case "scrollDisable":
                            map.DisableScrolls = node.ValueBool();
                            break;
                        case "everlast":
                            map.EverlastingDrops = node.ValueBool();
                            break;
                        case "bUnableToShop":
                            map.DisableGoToCashShop = node.ValueBool();
                            break;
                        case "bUnableToChangeChannel":
                            map.DisableChangeChannel = node.ValueBool();
                            break;
                        case "mobRate":
                            map.MobRate = node.ValueFloat();
                            break;
                        case "fieldLimit":
                            map.Limitations = (FieldLimit)node.ValueInt32();
                            break;
                        default:
                            //Console.WriteLine($"Unhandled info node {node.Name} for map {ID}");
                            break;

                    }
                }

                if (map.ReturnMap == Constants.InvalidMap)
                {
                    Trace.WriteLine($"No return map for {map.ID}");
                    if (map.ForcedReturn == Constants.InvalidMap)
                    {
                        Trace.WriteLine($"Also no forced return map for {map.ID}");
                    }
                }

                if (map.DisableGoToCashShop) Trace.WriteLine($"Mapid {map.ID}: No cashshop");
                if (map.DisableChangeChannel) Trace.WriteLine($"Mapid {map.ID}: No CC");

                /**************************** HOTFIX??? SEEMS TO WORK PERFECTLY EXCEPT FOR PET PARK **********************************/
                if (map.ForcedReturn == Constants.InvalidMap && (map.Limitations & FieldLimit.SkillLimit) != 0)
                {
                    Program.MainForm.LogDebug($"Found jq map with no forced return. Setting it to nearest town. Map id:{map.ID}");
                    map.ForcedReturn = map.ReturnMap;
                }
                /*********************************************************************************************************************/

                var footholds =
                    from fhlayer in mapNode["foothold"]
                    from fhgroup in fhlayer
                    from fh in fhgroup
                    select new { fh };

                map.SetFootholds(footholds.Select(x => new Foothold
                {
                    ID = (ushort)Utils.ConvertNameToID(x.fh.Name),
                    NextIdentifier = x.fh["next"].ValueUInt16(),
                    PreviousIdentifier = x.fh["prev"].ValueUInt16(),
                    X1 = x.fh["x1"].ValueInt16(),
                    X2 = x.fh["x2"].ValueInt16(),
                    Y1 = x.fh["y1"].ValueInt16(),
                    Y2 = x.fh["y2"].ValueInt16()
                }).ToList());

                map.GenerateMBR(Rectangle.FromLTRB(VRLeft, VRTop, VRRight, VRBottom));

                ValidateMapLayers(mapNode, map);
                ReadLife(mapNode, map);
                ReadPortals(mapNode, map);
                ReadSeats(mapNode, map);
                ReadAreas(mapNode, map);
                ReadMapReactors(mapNode, map);

                return map;
            }, x => x.ID);

        }

        static void ValidateMapLayers(NXNode mapNode, Map map)
        {
            var bNode = pClientFile.BaseNode["Map"];
            for (int layer = 0; layer <= 7; layer++)
            {
                NXNode layerNode = mapNode[layer.ToString()];
                foreach (NXNode layerSubNode in layerNode)
                {
                    switch (layerSubNode.Name)
                    {
                        case "tile":
                            if (layerSubNode.ChildCount == 0) continue;
                            string tS;
                            try
                            {
                                tS = layerNode["info"]["tS"].ValueString();
                            }
                            catch (NullReferenceException)
                            {
                                throw new NullReferenceException(string.Format("Map layer tS not found when trying to parse tiles in map {0} at layer {1}. This will cause a client crash.", map.ID, layer));
                            }
                            foreach (NXNode tileNode in layerSubNode)
                            {
                                int tileIdx = int.Parse(tileNode.Name);
                                string tileName = tileNode["u"].ValueString();
                                int tileNo = tileNode["no"].ValueInt32();
                                string tilePath = "Map/Tile/" + tS + ".img/" + tileName + "/" + tileNo;
                                try
                                {
                                    pClientFile.ResolvePath(tilePath);
                                }
                                catch (NullReferenceException)
                                {
                                    throw new NullReferenceException(string.Format("Map tile not found at \"{0}\" for map {1} at layer {2}, tile idx {3}. This will cause a client crash.", tilePath, map.ID, layer, tileIdx));
                                }
                            }
                            break;
                        case "obj":
                            foreach (NXNode objNode in layerSubNode)
                            {
                                int objIdx = int.Parse(objNode.Name);
                                string objectImgName = objNode["oS"].ValueString() + ".img";
                                string imgSubPath = string.Join("/", objNode.Where(n => n.Name.StartsWith("l")).Select(n => n.ValueString()));
                                string fullPath = "Map/Obj/" + objectImgName + "/" + imgSubPath;
                                if (!pClientFile.ContainsPath(fullPath))
                                {
                                    throw new NullReferenceException(string.Format("Map obj not found at \"{0}\", for map {1} at layer {2}, obj idx {3}. This will cause a client crash.", fullPath, map.ID, layer, objIdx));
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break_layer: continue;
            }
            foreach (NXNode backSubNode in mapNode["back"].Where(i => i.ChildCount > 0))
            {
                string bS = backSubNode["bS"].ValueString();
                if (string.IsNullOrWhiteSpace(bS)) continue;
                int backIdx = int.Parse(backSubNode.Name);
                string img = bS + ".img";
                int no = backSubNode["no"].ValueInt32();
                string backPath = "Map/Back/" + img + "/back/" + no;
                try
                {
                    pClientFile.ResolvePath(backPath);
                }
                catch (NullReferenceException)
                {
                    throw new NullReferenceException(string.Format("Map background not found at \"{0}\" for map {1}, back idx {2}. This will cause a client crash.", backPath, map.ID, backIdx));
                }
            }
        }
        
        static void ReadFieldSetData()
        {
            foreach (var pNode in pServerFile.BaseNode["FieldSet.img"])
            {
                try
                {
                    var fsData = new FieldSetData(pNode);
                    FieldSet.Instances[fsData.Name] = new FieldSet(fsData);
                }
                catch (ControlledException e)
                {
                    Program.MainForm.LogAppend("Field set {0} not loaded due to error: {1}", pNode.Name, e.Message);
                }
            }
        }
        
        static void ReadLife(NXNode mapNode, Map map)
        {
            if (!mapNode.ContainsChild("life")) return;

            foreach (var pNode in mapNode["life"])
            {
                var lifeNode = pNode;
                Life lf = new Life();

                foreach (var node in lifeNode)
                {
                    switch (node.Name)
                    {
                        // Not sure what to do with this one
                        case "hide": break;

                        case "mobTime": lf.RespawnTime = node.ValueInt32(); break;
                        case "f": lf.FacesLeft = node.ValueBool(); break;
                        case "x": lf.X = node.ValueInt16(); break;
                        case "y": lf.Y = node.ValueInt16(); break;
                        case "cy": lf.Cy = node.ValueInt16(); break;
                        case "rx0": lf.Rx0 = node.ValueInt16(); break;
                        case "rx1": lf.Rx1 = node.ValueInt16(); break;
                        case "fh": lf.Foothold = node.ValueUInt16(); break;
                        case "id": lf.ID = (Int32)Utils.ConvertNameToID(node.ValueOrDie<string>()); break;
                        case "type": lf.Type = char.Parse(node.ValueOrDie<string>()); break;
                        case "info": break; // Unknown node. Only 1 NPC has this, with the value 5
                        default:
                            {
                                Console.WriteLine($"Did not handle node {node.Name} of life {lf.ID} node {pNode.Name} map {map.ID}");
                                break;
                            }
                    }
                }
                map.AddLife(lf);
            }
        }

        static void ReadAreas(NXNode mapNode, Map map)
        {
            if (!mapNode.ContainsChild("area")) return;

            foreach (var pNode in mapNode["area"])
            {
                map.AddArea(new MapArea(pNode));
            }
        }
        static void ReadMapReactors(NXNode mapNode, Map map)
        {
            if (!mapNode.ContainsChild("reactor")) return;

            foreach (var rNode in mapNode["reactor"])
            {
                int rID = rNode["id"].ValueInt32();
                if (!GameDataProvider.Reactors.ContainsKey(rID))
                {
                    Program.MainForm.LogAppend($"Reactor id {rID} not found in map {map.ID} at reactor {rNode.Name}! Skipping...");
                    continue;
                }
                var mReactor = new FieldReactor(map, rNode);
                map.ReactorPool.AddReactor(mReactor, true);
            }
        }
        static void ReadReactors()
        {
            var reactorNodes = pClientFile.BaseNode["Reactor"];
            Reactors = new Dictionary<int, Reactor>();
            foreach (var rNode in reactorNodes)
            {
                var reactor = new Reactor(rNode);
                Reactors.Add(reactor.ID, reactor);
            }
            foreach (var r in Reactors.Where(r => r.Value.Link > 0).Select(r => r.Value))
            {
                r.States = Reactors[r.Link].States;
            }
        }

        static void ReadReactorActions()
        {
            ReactorActions = new Dictionary<string, ReactorActionData>();
            foreach (var node in pServerFile.BaseNode["ReactorAction.img"])
            {
                var rAction = new ReactorActionData(node);
                ReactorActions.Add(rAction.Name, rAction);
            }
        }

        static void ReadPortals(NXNode mapNode, Map map)
        {
            if (!mapNode.ContainsChild("portal")) return;

            byte portalId = 0;
            foreach (var pNode in mapNode["portal"])
            {
                map.AddPortal(new Portal(pNode, portalId++));
            }
        }

        static void ReadSeats(NXNode mapNode, Map map)
        {
            if (!mapNode.ContainsChild("seat")) return;

            foreach (var pNode in mapNode["seat"])
            {
                Point pPoint = pNode.ValueOrDie<Point>();

                Seat seat = new Seat
                (
                    (byte)Utils.ConvertNameToID(pNode.Name),
                    (short)pPoint.X,
                    (short)pPoint.Y
                );
                map.AddSeat(seat);
            }
        }

        static void ReadQuestData()
        {
            Quests = new Dictionary<short, WZQuestData>();
            foreach (var cQuest in pClientFile.ResolvePath("Quest/Check.img"))
            {
                // Ensure connected nodes exist
                pClientFile.ResolvePath($"Quest/Act.img/{cQuest.Name}");
                pClientFile.ResolvePath($"Quest/QuestInfo.img/{cQuest.Name}");
                pClientFile.ResolvePath($"Quest/Say.img/{cQuest.Name}");

                WZQuestData qd = new WZQuestData(pClientFile, cQuest);
                Quests.Add(qd.QuestID, qd);
            }
        }

        static void ReadNpcs()
        {
            var npcs = pClientFile.BaseNode["Npc"].ToList();

            NPCs = IterateAllToDict(npcs, pNode =>
            {
                NPCData npc = new NPCData(pClientFile, pNode);
                return npc;
            }, x => x.ID);
        }

        static void ReadNpcShops()
        {
            foreach (var shopNode in pServerFile.BaseNode["NpcShop.img"])
            {
                int npcID = int.Parse(shopNode.Name);
                if (!NPCs.TryGetValue(npcID, out var npcData)) continue;
                foreach (var itemNode in shopNode)
                {
                    var shopItem = new ShopItemData(itemNode);
                    if (!Items.ContainsKey(shopItem.ItemID) && !Equips.ContainsKey(shopItem.ItemID)) continue;
                    npcData.Shop.Add(shopItem);
                }
            }
        }

        static void ReadSkills()
        {
            var allJobs = pClientFile.BaseNode["Skill"].Where(x => x.Name != "MobSkill.img").ToList();

            Jobs = IterateAllToDict(allJobs, pNode => int.Parse(pNode.Name.Replace(".img", "")), x => x).Values.ToList();


            Skills = IterateAllToDict(allJobs.SelectMany(x => x["skill"]), mNode =>
            {
                int SkillID = int.Parse(mNode.Name);
                var skillData = new SkillData
                {
                    ID = SkillID
                };

                var elementFlags = SkillElement.Normal;

                if (mNode.ContainsChild("elemAttr"))
                {
                    string pbyte = mNode["elemAttr"].ValueString();
                    switch (pbyte.ToLowerInvariant())
                    {
                        case "i":
                            elementFlags = SkillElement.Ice;
                            break;
                        case "f":
                            elementFlags = SkillElement.Fire;
                            break;
                        case "s":
                            elementFlags = SkillElement.Poison;
                            break;
                        case "l":
                            elementFlags = SkillElement.Lightning;
                            break;
                        case "h":
                            elementFlags = SkillElement.Holy;
                            break;
                        default:
                            Console.WriteLine($"Unhandled elemAttr type {pbyte} for id {SkillID}");
                            break;
                    }
                }

                skillData.Element = elementFlags;

                foreach (var iNode in mNode)
                {
                    switch (iNode.Name)
                    {
                        case "damage": // Crit hit for 4100001
                        case "summon":
                        case "special":

                        case "Frame":
                        case "cDoor":
                        case "mDoor":

                        case "prepare":
                        case "mob":
                        case "ball":
                        case "ball0":
                        case "ball1":
                        case "action":
                        case "affected":
                        case "effect":
                        case "effect0":
                        case "effect1":
                        case "effect2":
                        case "effect3":
                        case "finish": // him
                        case "hit":
                        case "icon":
                        case "iconMouseOver":
                        case "iconDisabled":
                        case "afterimage":
                        case "tile":
                        case "level":
                        case "state":
                        case "mob0":
                        case "finalAttack": break;

                        case "elemAttr":
                            string pbyte = iNode.ValueString();
                            switch (pbyte.ToLowerInvariant())
                            {
                                case "i":
                                    elementFlags = SkillElement.Ice;
                                    break;
                                case "f":
                                    elementFlags = SkillElement.Fire;
                                    break;
                                case "s":
                                    elementFlags = SkillElement.Poison;
                                    break;
                                case "l":
                                    elementFlags = SkillElement.Lightning;
                                    break;
                                case "h":
                                    elementFlags = SkillElement.Holy;
                                    break;
                                default:
                                    Console.WriteLine($"Unhandled elemAttr type {pbyte} for id {SkillID}");
                                    break;
                            }
                            break;

                        case "skillType":
                            skillData.Type = iNode.ValueByte();
                            break;
                        case "weapon":
                            skillData.Weapon = iNode.ValueByte();
                            break;

                        case "req":
                            skillData.RequiredSkills = new Dictionary<int, byte>();
                            foreach (var nxNode in iNode)
                            {
                                skillData.RequiredSkills[int.Parse(nxNode.Name)] = nxNode.ValueByte();
                            }
                            break;
                        default:
                            Trace.WriteLine($"Unknown skill prop {iNode.Name} in {SkillID}");
                            break;
                    }

                }

                skillData.Levels = new SkillLevelData[mNode["level"].ChildCount + 1];
                foreach (var iNode in mNode["level"])
                {
                    var sld = new SkillLevelData();
                    sld.Level = byte.Parse(iNode.Name);

                    foreach (var nxNode in iNode)
                    {
                        switch (nxNode.Name)
                        {
                            case "hs": // help string (refer to Strings.wz)
                            case "action": // Stance
                            case "ball":
                            case "hit":
                            case "bulletConsume": break; // Avenger uses like 3 stars

                            case "x":
                                sld.XValue = nxNode.ValueInt16();
                                break;
                            case "y":
                                sld.YValue = nxNode.ValueInt16();
                                break;
                            case "z":
                                sld.ZValue = nxNode.ValueInt16();
                                break;
                            case "attackCount":
                                sld.HitCount = nxNode.ValueByte();
                                break;
                            case "mobCount":
                                sld.MobCount = nxNode.ValueByte();
                                break;
                            case "time":
                                sld.BuffSeconds = nxNode.ValueInt32();
                                break;
                            case "damage":
                                sld.Damage = nxNode.ValueInt16();
                                break;
                            case "range":
                                sld.AttackRange = nxNode.ValueInt16();
                                break;
                            case "mastery":
                                sld.Mastery = nxNode.ValueByte();
                                break;
                            case "hp":
                                sld.HPProperty = nxNode.ValueInt16();
                                break;
                            case "mp":
                                sld.MPProperty = nxNode.ValueInt16();
                                break;
                            case "prop":
                                sld.Property = nxNode.ValueInt16();
                                break;
                            case "hpCon":
                                sld.HPUsage = nxNode.ValueInt16();
                                break;
                            case "mpCon":
                                sld.MPUsage = nxNode.ValueInt16();
                                break;
                            case "itemCon":
                                sld.ItemIDUsage = nxNode.ValueInt32();
                                break;
                            case "itemConNo":
                                sld.ItemAmountUsage = nxNode.ValueInt16();
                                break;
                            case "bulletCount":
                                sld.BulletUsage = nxNode.ValueInt16();
                                break;
                            case "moneyCon":
                                sld.MesosUsage = nxNode.ValueInt16();
                                break;
                            case "speed":
                                sld.Speed = nxNode.ValueInt16();
                                break;
                            case "jump":
                                sld.Jump = nxNode.ValueInt16();
                                break;
                            case "eva":
                                sld.Avoidability = nxNode.ValueInt16();
                                break;
                            case "acc":
                                sld.Accurancy = nxNode.ValueInt16();
                                break;
                            case "mad":
                                sld.MagicAttack = nxNode.ValueInt16();
                                break;
                            case "mdd":
                                sld.MagicDefense = nxNode.ValueInt16();
                                break;
                            case "pad":
                                sld.WeaponAttack = nxNode.ValueInt16();
                                break;
                            case "pdd":
                                sld.WeaponDefense = nxNode.ValueInt16();
                                break;
                            case "lt":
                                {
                                    Point pPoint = nxNode.ValueOrDie<Point>();
                                    sld.LTX = (short)pPoint.X;
                                    sld.LTY = (short)pPoint.Y;
                                    break;
                                }
                            case "rb":
                                {
                                    Point pPoint = nxNode.ValueOrDie<Point>();
                                    sld.RBX = (short)pPoint.X;
                                    sld.RBY = (short)pPoint.Y;
                                    break;
                                }

                            default:
                                Console.WriteLine($"Unhandled skill level node {nxNode.Name} for id {SkillID} (level {sld.Level})");
                                break;
                        }
                    }

                    sld.ElementFlags = elementFlags;
                    if (SkillID == Constants.Gm.Skills.Hide)
                    {
                        // Give hide some time... like lots of hours
                        sld.BuffSeconds = 24 * 60 * 60;
                        sld.XValue = 1; // Eh. Otherwise there's no buff
                    }

                    skillData.Levels[sld.Level] = sld;
                }
                skillData.MaxLevel = (byte)(skillData.Levels.Length - 1); // As we skip 0

                return skillData;
            }, x => x.ID);

            MobSkills = IterateAllToDict(pClientFile.BaseNode["Skill"]["MobSkill.img"], eNode =>
            {
                var dict = new Dictionary<byte, MobSkillLevelData>();
                byte SkillID = (byte)int.Parse(eNode.Name);
                foreach (var sNode in eNode["level"])
                {
                    var levelNode = sNode;

                    byte Level = (byte)int.Parse(sNode.Name);
                    MobSkillLevelData msld = new MobSkillLevelData()
                    {
                        SkillID = SkillID,
                        Level = Level
                    };

                    foreach (var node in levelNode)
                    {
                        switch (node.Name)
                        {
                            case "effect":
                            case "affected":
                            case "tile": // Clouds (for the poison cloud skill)
                            case "mob":
                            case "mob0": break;

                            case "time":
                                msld.Time = node.ValueInt16();
                                break;
                            case "mpCon":
                                msld.MPConsume = node.ValueInt16();
                                break;
                            case "x":
                                msld.X = node.ValueInt32();
                                break;
                            case "y":
                                msld.Y = node.ValueInt32();
                                break;
                            case "prop":
                                msld.Prop = node.ValueByte();
                                break;
                            case "interval":
                                msld.Cooldown = node.ValueInt16();
                                break;
                            case "hp":
                                msld.HPLimit = node.ValueByte();
                                break;
                            case "limit":
                                msld.SummonLimit = node.ValueUInt16();
                                break;
                            case "summonEffect":
                                msld.SummonEffect = node.ValueByte();
                                break;
                            case "lt":
                                {
                                    Point pPoint = node.ValueOrDie<Point>();
                                    msld.LTX = (short)pPoint.X;
                                    msld.LTY = (short)pPoint.Y;
                                    break;
                                }
                            case "rb":
                                {
                                    Point pPoint = node.ValueOrDie<Point>();
                                    msld.RBX = (short)pPoint.X;
                                    msld.RBY = (short)pPoint.Y;
                                    break;
                                }

                            default:
                                {
                                    if (node.Name.All(char.IsDigit))
                                    {
                                        var summonId = node.ValueInt32();
                                        (msld.Summons ?? (msld.Summons = new List<int>())).Add(summonId);
                                    }
                                    else
                                    {
                                        Console.WriteLine(
                                            $"Unhandled Mob skill {msld.SkillID} level {msld.Level} node {node.Name}");
                                    }
                                    break;
                                }
                        }
                    }


                    dict[msld.Level] = msld;
                }

                return new Tuple<byte, Dictionary<byte, MobSkillLevelData>>(SkillID, dict);
            }, x => x.Item1, x => x.Item2);
        }

        static void ReadQuiz()
        {
            var questions = from page in pClientFile.BaseNode["Etc"]["OXQuiz.img"]
                       from question in page
                       select new QuizData(byte.Parse(page.Name), byte.Parse(question.Name), question["a"].ValueByte() == 0 ? 'x' : 'o');

            var pages = questions
                .GroupBy(q => q.QuestionPage)
                .Where(p => p.Key < 8); //pages past 7 are untranslated korean in this version

            foreach (var page in pages)
            {
                QuizQuestions[page.Key] = page.ToList();
            }
        }

        public static int CalculateDropChance(double x)
        {
            if (x > 1.0 || x < 0.0)
                throw new Exception("Invalid dropchance");


            x *= 1000000000.0;
            var y = Math.Min((int)x, 1000000000);

            return y;
        }

        static void ReadDrops()
        {
            Drops = IterateAllToDict(pServerFile.BaseNode["Reward_ori.img"], pNode =>
            {
                string dropper = pNode.Name;

                var drops = pNode.Select(iNode =>
                    {
                        var dropdata = new DropData(iNode, dropper);
                        return dropdata;

                    }
                ).ToArray();

                if (dropper.StartsWith("m"))
                {
                    string trimmed = dropper.Trim().StartsWith("m0") ? dropper.Trim().Replace("m0", "m") : dropper;
                    //int mobId = (int)Utils.ConvertNameToID(trimmed.Substring(1));
                    //if (Mobs.ContainsKey(mobId))
                    //{
                    //    foreach (var drop in drops)
                    //    {
                    //        if (!Items.ContainsKey(drop.ItemID))
                    //            Program.MainForm.LogAppend("Couldn't find item {0} from mob {1}", drop.ItemID, mobId);
                    //    }
                    //}

                    return new Tuple<string, DropData[]>(trimmed, drops);
                }
                else if (dropper.StartsWith("r"))
                {
                    string trimmed = dropper.Trim().StartsWith("r000") ? dropper.Trim().Replace("r000", "r") : dropper;
                    return new Tuple<string, DropData[]>(trimmed, drops);
                }
                else
                {
                    Console.WriteLine("Unknown dropper type? {0}", dropper);
                    return new Tuple<string, DropData[]>(dropper, drops);
                }
            }, x => x.Item1, x => x.Item2);

            // AddThanksgivingDrops(); //TODO remove this after thanksgiving
        }

        private static void AddThanksgivingDrops() //Mother fucking turkeys?
        {
            List<DropData> tdaydrops = new List<int>() { 03994012, 03994000, 03994006, 03994003, 03994001, 03994013, 03994008, 03994005, 03994007, 03994010 }.Select(id => new DropData()
            {
                ItemID = id,
                Chance = CalculateDropChance((id == 03994012 || id == 03994013) ? 0.00007 : 0.00015)
            }).ToList();

            var fuck = Drops.Select(dropList =>
            {
                var newlist = dropList.Value.ToList();
                newlist.AddRange(tdaydrops);
                return new KeyValuePair<string, DropData[]>(dropList.Key, newlist.ToArray());
            });
            fuck.ForEach(shit => { Drops.Remove(shit.Key); Drops.Add(shit.Key, shit.Value); });
        }



    }
}