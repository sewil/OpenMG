﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using WvsBeta.Common;
using WvsBeta.Common.Characters;
using WvsBeta.Common.Sessions;
using WvsBeta.Login.Packets;
using WvsBeta.Login.PacketHandlers;
using WvsBeta.Common.Objects;
using WvsBeta.Common.Extensions;
using WvsBeta.Common.Enums;

namespace WvsBeta.Login
{
    public class ClientSession : ConnectionSession
    {
        private static ILog log = LogManager.GetLogger("LoginLogic");

        public Objects.Player Player { get; private set; }
        public bool Loaded { get; set; }
        public bool IsCCing { get; private set; }

        public ClientSession(System.Net.Sockets.Socket pSocket) : base(pSocket, false, "LoginClientSession")
        {
            Player = new Objects.Player()
            {
                LoggedOn = false,
                Socket = this
            };
            Loaded = false;
            Pinger.Add(this);
            Server.Instance.AddPlayer(Player);

            SendHandshake(Config.Instance.MapleVersion, Config.Instance.MaplePatchLocation, Config.Instance.MapleLocale);
            SendMemoryRegions();
        }

        public override void StartLogging()
        {
            base.StartLogging();

            log4net.ThreadContext.Properties["LoginState"] = Player.State;
            if (Loaded)
            {
                log4net.ThreadContext.Properties["UserID"] = Player.ID;
            }
        }

        public override void EndLogging()
        {
            base.EndLogging();
            log4net.ThreadContext.Properties.Remove("UserID");
            log4net.ThreadContext.Properties.Remove("LoginState");
        }

        public override void OnDisconnect()
        {
            try
            {
                StartLogging();
                try
                {
                    if (crashLogTmp != null)
                    {
                        FileWriter.WriteLine(Path.Combine("ClientCrashes", base.IP + "-unknown_username.txt"),
                            crashLogTmp);
                        crashLogTmp = null;
                    }
                }
                catch { }

                if (Player != null)
                {
                    Server.Instance.RemovePlayer(Player.SessionHash);
                    if (Player.LoggedOn)
                    {
                        Program.MainForm.ChangeLoad(false);

                        Player.Characters.Clear();

                        if (!IsCCing)
                            RedisBackend.Instance.RemovePlayerOnline(Player.ID, Player.Socket.IP);

                        Player.Socket = null;
                        Player = null;
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                EndLogging();
            }
            Pinger.Remove(this);
        }

        private string crashLogTmp = null;

        private static HashSet<ClientMessages> logIgnore = new HashSet<ClientMessages>() { ClientMessages.CLIENT_HASH, ClientMessages.PONG, ClientMessages.LOGIN_CHECK_PIN, ClientMessages.LOGIN_CHECK_PASSWORD, ClientMessages.LOGIN_WORLD_INFO_REQUEST, ClientMessages.LOGIN_SELECT_CHANNEL };
        private static HashSet<ClientMessages> traceIgnore = new HashSet<ClientMessages>() { ClientMessages.CLIENT_HASH, ClientMessages.PONG };

        public override void AC_OnPacketInbound(Packet packet)
        {
            try
            {
                ClientMessages header = (ClientMessages)packet.ReadByte();
                if (!traceIgnore.Contains(header))
                    Program.MainForm.LogDebug($"[Client->LoginServer] {header} - {packet}");
                if (!logIgnore.Contains(header))
                    Common.Tracking.PacketLog.ReceivedPacket(packet, (byte)header, Server.Instance.Name, this.IP);

                if (!Loaded)
                {
                    switch (header)
                    {
                        case ClientMessages.LOGIN_CHECK_PASSWORD:
                            new CheckPasswordHandler(this, log, packet, ref crashLogTmp);
                            break;
                        case ClientMessages.CLIENT_CRASH_REPORT:
                            crashLogTmp = packet.ReadString();
                            Program.MainForm.LogAppend("Received a crashlog!!! {0}", crashLogTmp);
                            if (crashLogTmp.Contains("LdrShutdownProcess"))
                            {
                                // Ignore
                                crashLogTmp = null;
                            }
                            else
                            {
                            }
                            break;
                        case ClientMessages.LOGIN_EULA:
                            new ConfirmEULAHandler(this, log, packet);
                            break;
                        case ClientMessages.LOGIN_SET_GENDER:
                            new SetGenderHandler(this, log, packet);
                            break;
                        case ClientMessages.LOGIN_CHECK_PIN:
                            PINHandler.HandlePinCheck(this, packet, log);
                            break;
                        case ClientMessages.LOGIN_SET_PIN:
                            PINHandler.HandlePinSet(this, packet);
                            break;
                    }
                }
                else
                {
                    switch (header)
                    {
                        // Ignore this one
                        case ClientMessages.LOGIN_CHECK_PASSWORD: break;

                        case ClientMessages.LOGIN_SELECT_CHANNEL:
                            ChannelSelectHandler.HandleClientRequest(this, log, packet);
                            break;
                        case ClientMessages.LOGIN_WORLD_INFO_REQUEST:
                            new WorldInfoHandler(this, log);
                            break;
                        case ClientMessages.LOGIN_WORLD_SELECT:
                            OnWorldSelect(packet);
                            break;
                        case ClientMessages.LOGIN_CHECK_CHARACTER_NAME:
                            OnCharNamecheck(packet);
                            break;
                        case ClientMessages.LOGIN_SELECT_CHARACTER:
                            new CharacterSelectHandler(this, log, packet);
                            break;
                        case ClientMessages.LOGIN_CREATE_CHARACTER:
                            OnCharCreation(packet);
                            break;
                        case ClientMessages.LOGIN_DELETE_CHARACTER:
                            OnCharDeletion(packet);
                            break;
                        case ClientMessages.PONG:
                            RedisBackend.Instance.SetPlayerOnline(Player.ID, 1, Player.Socket.IP);
                            break;
                        case ClientMessages.CLIENT_HASH: break;
                        default:
                            {
                                var errorText = "Unknown packet found " + packet;
                                Server.Instance.ServerTraceDiscordReporter.Enqueue(errorText);
                                Program.MainForm.LogAppend(errorText);

                                break;
                            }
                    }
                }
             }
            catch (Exception ex)
            {
                var errorText = "Exception caught: " + ex + ", packet: " + packet;
                Server.Instance.ServerTraceDiscordReporter.Enqueue(errorText);
                Program.MainForm.LogAppend(errorText);
                log.Error(ex);
                Disconnect();
            }
        }

        public override void OnHackDetected()
        {
            TryRegisterHackDetection();
        }

        public void TryRegisterHackDetection()
        {
            if (!Loaded) return;
            TryRegisterHackDetection(Player.ID);
        }

        public void HandleLogin()
        {
            StartLogging();
            TryRegisterHackDetection();
            Loaded = true;
            RedisBackend.Instance.SetPlayerOnline(Player.ID, 1, IP);

            string username = Player.Username;

            if (crashLogTmp != null)
            {
                var crashlogName = IP + "-" + username + ".txt";
                FileWriter.WriteLine(Path.Combine("ClientCrashes", crashlogName), crashLogTmp);
                crashLogTmp = null;
                Server.Instance.ServerTraceDiscordReporter.Enqueue($"Saving crashlog to {crashlogName}");
            }

            Player.LoggedOn = true;
            Program.MainForm.LogAppend($"Account {username} ({Player.ID}) logged on. Machine ID: {Player.MachineID}, Unique ID: {Player.UniqueID}, IP: {IP}, Ban counts: {Player.MachineBanCount}/{Player.UniqueBanCount}/{Player.IPBanCount}");
            Program.MainForm.ChangeLoad(true);

            // Update database
            Server.Instance.UsersDatabase.RunQuery(
                @"
                    UPDATE users SET 
                    last_login = NOW(), 
                    last_ip = @ip, 
                    last_machine_id = @machineId, 
                    last_unique_id = @uniqueId 
                    WHERE ID = @id",
                "@id", Player.ID,
                "@ip", IP,
                "@machineId", Player.MachineID,
                "@uniqueId", Player.UniqueID
            );
        }

        public bool IsValidName(string pName)
        {
            if (log.AssertWarning(Player.Characters.Count >= 3, "Reached maximum amount of characters and still did a namecheck.")) return false;
            if (log.AssertWarning(pName.Length < 4 || pName.Length > 12, "Name length invalid!")) return false;
            if (log.AssertWarning(pName.Any(x =>
            {
                if (x >= 'a' && x <= 'z') return false;
                if (x >= 'A' && x <= 'Z') return false;
                if (x >= '0' && x <= '9') return false;
                return true;
            }), "Name had invalid characters: " + pName)) return false;

            if (log.AssertWarning(WzReader.ForbiddenName.Exists(pName.Contains),
                "Charactername matched a ForbiddenName item. " + pName)) return false;

            return true;
        }

        public void ConnectToServer(int charid, byte[] IP, ushort port)
        {
            byte bit = 0, goPremium = 0;

            bit |= (byte)(goPremium << 1);

            log.Info($"Connecting to {IP[0]}.{IP[1]}.{IP[2]}.{IP[3]}:{port} world {Player.World} channel {Player.Channel} with charid {charid} name {Player.Characters[charid]}");

            IsCCing = true;

            var pack = new CharacterSelectPacket(IP, port, charid, Player.Characters[charid], bit).Encode(CharacterSelectPacket.Status.Success);
            SendPacket(pack);
        }

        public void OnCharNamecheck(Packet packet)
        {
            if (log.AssertWarning(Player.State != GameState.CharacterSelect && Player.State != GameState.CharacterCreation, "Trying to check character name while not in character select or creation screen.")) return;

            Player.State = GameState.CharacterCreation;

            string name = packet.ReadString();

            if (!IsValidName(name))
            {
                Packet pack = new Packet(ServerMessages.CHECK_CHARACTER_NAME_AVAILABLE);
                pack.WriteString(name);
                pack.WriteBool(true);
                SendPacket(pack);
                return;
            }

            if (Server.Instance.GetWorld(Player.World, out Center center))
            {
                center.Connection.CheckCharacternameTaken(Player.SessionHash, name);
            }
            else
            {
                log.AssertWarning(true, "Server was offline while checking for duplicate charname");
                Packet pack = new Packet(ServerMessages.CHECK_CHARACTER_NAME_AVAILABLE);
                pack.WriteString(name);
                pack.WriteBool(true);
                SendPacket(pack);
            }

        }

        public void OnCharDeletion(Packet packet)
        {
            if (log.AssertWarning(
                Player.State != GameState.CharacterSelect &&
                Player.State != GameState.CharacterCreation,
                "Trying to delete character while not in character select or create screen.")) return;

            int DoB = packet.ReadInt();
            int charid = packet.ReadInt();

            if (log.AssertWarning(Player.HasCharacterWithID(charid) == false, "Trying to delete a character that the player doesnt have. ID: " + charid)) return;

            if (Player.DateOfBirth != DoB)
            {
                log.Warn("Invalid DoB entered when trying to delete character.");

                var pack = new Packet(ServerMessages.DELETE_CHARACTER_RESULT);
                pack.WriteInt(charid);
                pack.WriteByte(18);
                SendPacket(pack);
                return;
            }

            if (!Server.Instance.GetWorld(Player.World, out Center center))
            {
                log.Error("Unable to connect to center server?");
                var pack = new Packet(ServerMessages.DELETE_CHARACTER_RESULT);
                pack.WriteInt(charid);
                pack.WriteByte(10);
                SendPacket(pack);
                return;
            }


            center.Connection?.RequestDeleteCharacter(Player.SessionHash, Player.ID, charid);
        }

        public void HandleCharacterDeletionResult(int characterId, byte result)
        {
            if (result == 0)
            {
                log.Info($"User deleted a character, called '{Player.Characters[characterId]}'");
                // Alright!
                Player.Characters.Remove(characterId);
            }

            var pack = new Packet(ServerMessages.DELETE_CHARACTER_RESULT);
            pack.WriteInt(characterId);
            pack.WriteByte(result);
            SendPacket(pack);
        }

        private bool IsValidCreationId(IEnumerable<int> validIds, int inputId, string name)
        {
            if (validIds.Contains(inputId)) return true;
            log.AssertError(true, $"[CharCreation] Invalid {name}: {inputId}");
            return false;
        }

        public void OnCharCreation(Packet packet)
        {
            if (log.AssertWarning(Player.State != GameState.CharacterCreation, "Trying to create character while not in character creation screen (skipped namecheck?).")) return;

            if (!Server.Instance.GetWorld(Player.World, out Center center))
            {
                log.Error("Unable to connect to center server?");
                goto not_available;
            }

            if (center.BlockCharacterCreation)
            {
                log.Error("Character creation blocked!");
                goto not_available;
            }

            Packet pack;
            string charname = packet.ReadString();

            if (!IsValidName(charname))
            {
                goto not_available;
            }

            int face = packet.ReadInt();
            int hair = packet.ReadInt();
            int haircolor = packet.ReadInt();
            int skin = packet.ReadInt();

            int top = packet.ReadInt();
            int bottom = packet.ReadInt();
            int shoes = packet.ReadInt();
            int weapon = packet.ReadInt();
            byte str = packet.ReadByte();
            byte dex = packet.ReadByte();
            byte intt = packet.ReadByte();
            byte luk = packet.ReadByte();

            log.AssertWarning(str >= 13 || dex >= 13 || intt >= 13 || luk >= 13, $" '{charname}'  is under suspicion of using Cheat Engine to get 13 stat ({str}/{dex}/{intt}/{luk}) during character creation.");

            if (!(str >= 4 && dex >= 4 && intt >= 4 && luk >= 4 && (str + dex + intt + luk) <= 25))
            {
                log.Error($"Invalid stats for character creation: {str} {dex} {intt} {luk}");
                goto not_available;
            }

            var cci = Player.Gender == 0 ? CreateCharacterInfo.Male : CreateCharacterInfo.Female;

            if (!IsValidCreationId(cci.Face, face, "face") ||
                !IsValidCreationId(cci.Hair, hair, "hair") ||
                !IsValidCreationId(cci.HairColor, haircolor, "haircolor") ||
                !IsValidCreationId(cci.Skin, skin, "skin") ||
                !IsValidCreationId(cci.Coat, top, "top") ||
                !IsValidCreationId(cci.Pants, bottom, "bottom") ||
                !IsValidCreationId(cci.Shoes, shoes, "shoes") ||
                !IsValidCreationId(cci.Weapon, weapon, "weapon"))
            {
                log.AssertError(true, $"User tried to create account with wrong starter equips. {face} {hair} {haircolor} {skin} {top} {bottom} {shoes} {weapon}");
                goto not_available;
            }


            pack = new Packet(ISClientMessages.PlayerCreateCharacter);
            pack.WriteString(Player.SessionHash);
            pack.WriteInt(Player.ID);
            pack.WriteByte((byte)Player.Gender);

            pack.WriteString(charname);

            pack.WriteInt(face);
            pack.WriteInt(hair);
            pack.WriteInt(haircolor);
            pack.WriteInt(skin);

            pack.WriteInt(top);
            pack.WriteInt(bottom);
            pack.WriteInt(shoes);
            pack.WriteInt(weapon);

            pack.WriteByte(str);
            pack.WriteByte(dex);
            pack.WriteByte(intt);
            pack.WriteByte(luk);

            center.Connection.SendPacket(pack);

            return;

            not_available:
            pack = new Packet(ServerMessages.CREATE_NEW_CHARACTER_RESULT);
            pack.WriteByte(1);
            SendPacket(pack);

        }

        public void HandleCreateNewCharacterResult(Packet packet)
        {
            var pack = new Packet(ServerMessages.CREATE_NEW_CHARACTER_RESULT);
            if (packet.ReadBool())
            {
                // Succeeded
                pack.WriteBool(false);
                var ad = new AvatarData(packet);
                ad.Encode(pack);


                log.Info($"User created a new character, called '{ad.CharacterStat.Name}'");
                Player.Characters.Add(ad.CharacterStat.ID, ad.CharacterStat.Name);
                Player.State = GameState.CharacterSelect;
            }
            else
            {
                pack.WriteBool(true);
            }
            Player.Socket.SendPacket(pack);
        }

        public void OnWorldSelect(Packet packet)
        {
            if (log.AssertWarning(Player.State != GameState.WorldSelect && Player.State != GameState.ChannelSelect,
                "Player tried to select world while not in worldselect or channelselect")) return;

            byte worldId = packet.ReadByte();

            if (!Server.Instance.GetWorld(worldId, out Center center))
            {
                var p = new CheckUserLimitPacket(WorldWarning.MaxUsers, WorldMarker.Overpopulated).Encode();
                SendPacket(p);
                return;
            }

            Player.World = worldId;

            center.Connection.RequestCharacterGetWorldLoad(Player.SessionHash, worldId);
        }

        public void HandleWorldLoadResult(Packet packet)
        {
            var pack = new CheckUserLimitPacket(packet).Encode();
            SendPacket(pack);
            Player.State = GameState.ChannelSelect;
        }

        public struct LoginLoggingStruct
        {
            public int localUserId { get; set; }
            public int uniqueId { get; set; }
            public string username { get; set; }
        }
        public void BackToLogin()
        {
            Player.State = GameState.LoginScreen;
            Program.MainForm.ChangeLoad(false);

            Loaded = false;
            Player.LoggedOn = false;
            RedisBackend.Instance.RemovePlayerOnline(Player.ID, Player.Socket.IP);
        }

        public override void SendPacket(Packet pPacket)
        {
            if ((ServerMessages)pPacket.Opcode != ServerMessages.PING)
                Program.MainForm.LogDebug($"[LoginServer->Client] {(ServerMessages)pPacket.Opcode} - {pPacket}");
            base.SendPacket(pPacket);
        }
    }
}
