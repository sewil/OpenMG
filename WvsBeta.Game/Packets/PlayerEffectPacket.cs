﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WvsBeta.Common.Sessions;
using static WvsBeta.Game.GameInventory;

namespace WvsBeta.Game.Packets
{
    public enum PlayerEffectType
    {
        LevelUp = 0,
        SkillOnSelf = 1,
        SkillOnOther = 2,
        QuestEffect = 3,
        InventoryChanged = 3,
        Pet = 4,
        UseEXPCharm = 6,
        Portal,
        JobChanged = 8,
    }
    public enum PetEffectType
    {
        LevelUp = 0,
        Tp = 1,
        TpBack = 2
    }
    public class PlayerEffectPacket : Packet
    {
        /// <summary>
        /// The write packet for showing player effects to other players.
        /// </summary>
        private Packet foreignPW;
        private GameCharacter chr;
        private PlayerEffectPacket(GameCharacter chr, PlayerEffectType type) : base(ServerMessages.PLAYER_EFFECT)
        {
            this.chr = chr;
            foreignPW = new Packet(ServerMessages.SHOW_FOREIGN_EFFECT);
            foreignPW.WriteInt(chr.ID);
            foreignPW.WriteByte((byte)type);

            WriteByte((byte)type);
        }
        private void Send(bool foreignOnly, bool localOnly)
        {
            if (!localOnly)
            {
                chr.Field.SendPacket(chr, foreignPW);
            }

            if (!foreignOnly)
            {
                chr.SendPacket(this);
            }
        }
        /// <summary>
        /// Send skill player effect packet.
        /// </summary>
        /// <param name="chr"></param>
        /// <param name="skillId"></param>
        /// <param name="skillLevel"></param>
        /// <param name="foreignOnly">Only show foreign effect.</param>
        /// <param name="localOnly">Only show local effect.</param>
        /// <param name="skillOnOther">Whether to show the skill effect on other or self.</param>
        public static void SendSkill(GameCharacter chr, int skillId, byte skillLevel, bool foreignOnly = false, bool localOnly = false, bool skillOnOther = false)
        {
            var p = new PlayerEffectPacket(chr, skillOnOther ? PlayerEffectType.SkillOnOther : PlayerEffectType.SkillOnSelf);
            if (!localOnly)
            {
                p.foreignPW.WriteInt(skillId);
                p.foreignPW.WriteByte(skillLevel);
            }
            if (!foreignOnly)
            {
                p.WriteInt(skillId);
                p.WriteByte(skillLevel);
            }
            p.Send(foreignOnly, localOnly);
        }

        public static void SendPetEffect(GameCharacter chr, PetEffectType type, bool foreignOnly = false, bool localOnly = false)
        {
            var p = new PlayerEffectPacket(chr, PlayerEffectType.Pet);
            if (!localOnly)
            {
                p.foreignPW.WriteByte((byte)type);
            }
            if (!foreignOnly)
            {
                p.WriteByte((byte)type);
            }
            p.Send(foreignOnly, localOnly);
        }

        public static void SendUseEXPCharm(GameCharacter chr, bool isSafetyCharm, int itemid, byte daysLeft, byte timesLeft)
        {
            var pw = new PlayerEffectPacket(chr, PlayerEffectType.UseEXPCharm);
            pw.WriteBool(isSafetyCharm);
            pw.WriteByte(timesLeft);
            pw.WriteByte(daysLeft);
            if (!isSafetyCharm)
            {
                pw.WriteInt(itemid);
            }
            pw.Send(false, true);
        }
        /// <summary>
        /// Beware: References Effect/Quest.img/%d which doesn't exist
        /// </summary>
        public static void SendQuestEffect(GameCharacter chr, string effect, int effectid)
        {
            var pw = new PlayerEffectPacket(chr, PlayerEffectType.QuestEffect);
            pw.WriteByte(0);
            pw.WriteString(effect);
            pw.WriteInt(effectid);
            pw.Send(false, true);
        }
        public static void SendInventoryChanged(GameCharacter chr, params (int itemid, short amount)[] items)
        {
            var itemsList = items.Select(i => new ExchangeItem(i.itemid, i.amount)).ToList();
            SendInventoryChanged(chr, itemsList);
        }
        /// <summary>
        /// Sends grey text items gained/lost. Max 255 items.
        /// </summary>
        public static void SendInventoryChanged(GameCharacter chr, IList<ExchangeItem> items)
        {
            if (items.Count == 0) return; // Prevent triggering quest effect

            var pw = new PlayerEffectPacket(chr, PlayerEffectType.InventoryChanged);
            pw.WriteByte((byte)items.Count);
            foreach (var item in items)
            {
                pw.WriteInt(item.itemID);
                pw.WriteInt(item.amount);
            }
            pw.Send(false, true);
        }

        public static void SendPortalSoundEffect(GameCharacter chr)
        {
            var pw = new PlayerEffectPacket(chr, PlayerEffectType.Portal);
            pw.Send(false, true);
        }
    }
}
