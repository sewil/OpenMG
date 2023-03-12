﻿using System.Collections.Generic;
using WvsBeta.Game;
using WvsBeta.Game.Scripting;

namespace WvsBeta.Scripts.Scripts
{
    [Script("standard")]
    public class Standard : IStandardScript
    {
        public IDictionary<string, string> ScriptNameMap { get; set; }
        public Standard()
        {
            ScriptNameMap = new Dictionary<string, string>
            {
                { "taxi1", "taxi" },
                { "taxi2", "taxi" },
                { "taxi3", "taxi" },
                { "taxi4", "taxi" },
                { "mTaxi", "taxi" },
                { "market00", "market" },
                { "market01", "market" },
                { "market02", "market" },
                { "market03", "market" },
                { "market04", "market" },
                { "market05", "market" },
                { "market06", "market" },
                { "market07", "market" },
                { "market08", "market" },
                { "market09", "market" },
                { "levelUP", "etc" },
                { "levelUP2", "etc" },
                { "mBoxItem0", "mapleIsland" },
                { "begin2", "mapleIsland" },
                { "begin4", "mapleIsland" },
                { "begin5", "mapleIsland" },
                { "begin7", "mapleIsland" },
                { "bari", "mapleIsland" },
                { "rein", "mapleIsland" },
                { "pio", "mapleIsland" },
                { "rithTeleport", "victoria0" },
                { "jane", "victoria0" },
                { "q2073", "victoria0" },
                { "bush1", "victoria1" },
                { "bush2", "victoria1" },
                { "herb_out", "victoria1" },
                { "herb_in", "victoria1" },
                { "owen", "victoria2" },
                { "mike", "victoria2" },
                { "flower_in", "victoria2" },
                { "viola_pink", "victoria2" },
                { "viola_blue", "victoria2" },
                { "viola_white", "victoria2" },
                { "flower_out", "victoria2" },
                { "petmaster", "victoria2" },
                { "pet_life", "victoria2" },
                { "pet_lifeitem", "victoria2" },
                { "pet_letter", "victoria2" },
                { "subway_ticket", "victoria3" },
                { "_subway_in", "victoria3" },
                { "subway_in", "victoria3" },
                { "subway_get1", "victoria3" },
                { "subway_get2", "victoria3" },
                { "subway_get3", "victoria3" },
                { "subway_out", "victoria3" },
                { "hotel1", "victoria3" },
                { "guild_proc", "ossyria2" },
                { "guild_mark", "ossyria2" },
                { "party1_enter", "party" },
                { "party1_play", "party" },
                { "party1_out", "party" },
                { "refine_henesys", "make1" },
                { "refine_sleepy", "make1" },
                { "face_henesys1", "face" },
                { "face_henesys2", "face" },
                { "face_orbis1", "face" },
                { "face_orbis2", "face" },
                { "face_ludi1", "face" },
                { "face_ludi2", "face" },
                { "lens_henesys1", "face" },
                { "lens_orbis1", "face" },
                { "lens_ludi1", "face" },
                { "skin_henesys1", "skin" },
                { "skin_orbis1", "skin" },
                { "skin_ludi1", "skin" },
                { "hair_henesys1", "hair" },
                { "hair_henesys2", "hair" },
                { "hair_kerning1", "hair" },
                { "hair_kerning2", "hair" },
                { "hair_orbis1", "hair" },
                { "hair_orbis2", "hair" },
                { "hair_ludi1", "hair" },
                { "hair_ludi2", "hair" },
                { "magician", "job" },
                { "fighter", "job" },
                { "change_swordman", "job2" },
                { "change_magician", "job2" },
                { "change_archer", "job2" },
                { "change_rogue", "job2" },
                { "inside_swordman", "job2" },
                { "inside_magician", "job2" },
                { "inside_archer", "job2" },
                { "inside_rogue", "job2" },
                { "warrior3", "job3" },
                { "wizard3", "job3" },
                { "bowman3", "job3" },
                { "thief3", "job3" },
                { "crack", "job3" },
                { "3jobExit", "job3" },
                { "holyStone", "job3" },
                { "sBoxItem0", "victoria3" },
                { "sBoxItem1", "victoria3" },
                { "sell_ticket", "contimove" },
                { "get_ticket", "contimove" },
                { "goOutWaitingRoom", "contimove" },
                { "boxPaper0", "zakum" },
                { "boxKey0", "zakum" },
                { "money100", "zakum" },
                { "money10000", "zakum" },
                { "boxItem0", "zakum" },
                { "boxItem1", "zakum" },
                { "boxItem2", "zakum" },
                { "boxItem3", "zakum" },
                { "boxBItem0", "zakum" },
                { "boxMob0", "zakum" },
                { "go280010000", "zakum" },
                { "Zakum00", "zakum" },
                { "Zakum01", "zakum" },
                { "Zakum02", "zakum" },
                { "Zakum03", "zakum" },
                { "Zakum04", "zakum" },
                { "Zakum05", "zakum" },
                { "Zakum06", "zakum" },
                { "boss", "zakum" },
                { "1001000", "shop" },
                { "1001001", "shop" },
                { "1011000", "shop" },
                { "1011001", "shop" },
                { "1011100", "shop" },
                { "1012004", "shop" },
                { "1021000", "shop" },
                { "1021001", "shop" },
                { "1021100", "shop" },
                { "1031000", "shop" },
                { "1031001", "shop" },
                { "1031100", "shop" },
                { "1032103", "shop" },
                { "1052104", "shop" },
                { "1061001", "shop" },
                { "1061002", "shop" },
                { "2012003", "shop" },
                { "2012004", "shop" },
                { "2012005", "shop" },
                { "2020001", "shop" },
                { "2022000", "shop" },
                { "2022001", "shop" },
                { "2022002", "shop" },
                { "2030009", "shop" },
                { "2040049", "shop" },
                { "2041002", "shop" },
                { "2041003", "shop" },
                { "2041006", "shop" },
                { "2041014", "shop" },
                { "2041016", "shop" },
                { "guildquest1_comment", "guildquest1" },
                { "guildquest1_board", "guildquest1" },
                { "guildquest1_knight", "guildquest1" },
                { "guildquest1_enter", "guildquest1" },
                { "guildquest1_clear", "guildquest1" },
                { "guildquest1_out", "guildquest1" },
                { "guildquest1_bonus", "guildquest1" },
                { "guild1F00", "guildquest1" },
                { "guild1F01", "guildquest1" },
                { "guild1F02", "guildquest1" },
                { "guild1F03", "guildquest1" },
                { "guild1F04", "guildquest1" },
                { "statuegate_open", "guildquest1" },
                { "guildquest1_statue", "guildquest1" },
                { "speargate_open", "guildquest1" },
                { "stonegate_open", "guildquest1" },
                { "metalgate_open", "guildquest1" },
                { "watergate_open", "guildquest1" },
                { "under30gate", "guildquest1" },
                { "secretgate1_open", "guildquest1" },
                { "secretgate2_open", "guildquest1" },
                { "secretgate3_open", "guildquest1" },
                { "ghostgate_open", "guildquest1" },
                { "kinggate2_open", "guildquest1" },
                { "kinggate_open", "guildquest1" },
                { "guildquest1_NPC1", "guildquest1" },
                { "guildquest1_will", "guildquest1" },
                { "guildquest1_baseball", "guildquest1" },
                { "syarenCheck", "guildquest1" },
                { "syarenItem0", "guildquest1" },
                { "syarenItem1", "guildquest1" },
                { "syarenItem2", "guildquest1" },
                { "syarenItem3", "guildquest1" },
                { "syarenItem4", "guildquest1" },
                { "syarenItem5", "guildquest1" },
                { "syarenItem6", "guildquest1" },
                { "syarenItem7", "guildquest1" },
                { "syarenItem8", "guildquest1" },
                { "syarenItem9", "guildquest1" },
                { "syarenItem10", "guildquest1" },
                { "syarenItem11", "guildquest1" },
                { "syarenItem12", "guildquest1" },
                { "syarenMob0", "guildquest1" },
                { "syarenMob1", "guildquest1" },
                { "syarenNPC0", "guildquest1" },
                { "9208000", "guildquest1" },
                { "9208001", "guildquest1" },
                { "9208002", "guildquest1" },
                { "9208003", "guildquest1" },
                { "9208004", "guildquest1" },
                { "9208005", "guildquest1" },
                { "9208006", "guildquest1" },
                { "9208007", "guildquest1" },
                { "9208008", "guildquest1" },
                { "9208009", "guildquest1" },
                { "9208010", "guildquest1" },
                { "9208011", "guildquest1" },
                { "9208012", "guildquest1" },
                { "9208013", "guildquest1" },
            };
        }
    }
}
