﻿using WvsBeta.Common.Sessions;

namespace WvsBeta.Game.Packets
{
    public class FieldEffectPacket : Packet
    {
        public enum Type : byte
        {
            QuestEffect = 2,
            Effect = 3,
            Sound = 4,
            BossHPBar = 5,
            Music = 6,
        }
        private FieldEffectPacket(Type type) : base(ServerMessages.FIELD_EFFECT)
        {
            WriteByte((byte)type);
        }
        public static FieldEffectPacket Effect(string path)
        {
            var pw = new FieldEffectPacket(Type.Effect);
            pw.WriteString(path);
            return pw;
        }
        public static FieldEffectPacket Sound(string path)
        {
            var pw = new FieldEffectPacket(Type.Sound);
            pw.WriteString(path);
            return pw;
        }
        public static FieldEffectPacket BossHPBar(int pHP, int pMaxHP, int pColorBottom, int pColorTop)
        {
            var pw = new FieldEffectPacket(Type.BossHPBar);
            pw.WriteInt(pHP);
            pw.WriteInt(pMaxHP);
            pw.WriteInt(pColorTop);
            pw.WriteInt(pColorBottom);
            return pw;
        }

        public static FieldEffectPacket QuestEffect()
        {
            var pw = new FieldEffectPacket(Type.QuestEffect);
            pw.WriteByte(2); // Portal type? 0 1 2 3 4
            pw.WriteString("gate");
            return pw;
        }
    }

}