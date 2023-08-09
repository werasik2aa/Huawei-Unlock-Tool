using System;
using System.Collections.Generic;
using static Witcher3_Multiplayer.langproc;

namespace Witcher3_Multiplayer.ClientHost
{
    [Serializable]
    public struct CombatTarget
    {
        public int Guid;
        public string Template;
    }
    [Serializable]
    public struct PlayerData
    {
        public string NickName;
        public string CharacterTemplate;
        public int ID;
        public double Version;
        public int HP;
        public int LevelID;
        public int Plevel;
        public int State;
        public Vector3 PlayerPosition;
        public Vector3 HorsePosition;
    }
    [Serializable]
    public struct ServerInfo
    {
        public string Name;
        public int CurPlayers;
        public int MaxPlayers;
        public double Version;
    }
    [Serializable]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        override public string ToString()
        {
            return Math.Round(x) + ", " + Math.Round(y) + ", " + Math.Round(z);
        }
    }
    [Serializable]
    public struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public Quaternion(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        override public string ToString()
        {
            return Math.Round(x) + ", " + Math.Round(y) + ", " + Math.Round(z);
        }
    }
    [Serializable]
    public struct Vector2
    {
        public float x;
        public float y;
        public Vector2(float X, float Y)
        {
            x = X;
            y = Y;
        }
        override public string ToString()
        {
            return Math.Round(x) + ", " + Math.Round(y);
        }
    }
    [Serializable]
    public class State
    {
        public byte[] buffer = new byte[DataTypes.bufSize];
    }
    public static class DataTypes
    {
        public static int bufSize = 8 * 1024;
        public enum WeatherState
        {
            STORM,
            RAINI
        }
        public enum PlayerStates
        {
            JumpClimb,
            Exploration,
            TraverseExploration,
            Swimming,
            HorseRiding,
            Sailing,
            SailingPassive,

        }
        public enum Acts
        {
            EBG_Combat_Shield,
            EBG_Combat_1Handed_Sword,
            EBG_Combat_1Handed_Axe,
            EBG_Combat_1Handed_Blunt,
            EBG_Combat_1Handed_Any,
            EBG_Combat_2Handed_Sword,
            EBG_Combat_2Handed_Any,
            EBG_Combat_2Handed_Hammer,
            EBG_Combat_2Handed_Axe,
            EBG_Combat_2Handed_Halberd,
            EBG_Combat_2Handed_Spear,
            EBG_Combat_2Handed_Staff,
            EBG_Combat_Fists,
            EBG_Combat_Bow,
            EBG_Combat_Crossbow,
            EBG_Combat_Witcher,
            EBG_Combat_Sorceress,
            EBG_Combat_WildHunt_Imlerith,
            EBG_Combat_WildHunt_Imlerith_Second_Stage,
            EBG_Combat_WildHunt_Caranthir,
            EBG_Combat_WildHunt_Caranthir_Second_Stage,
            EBG_Combat_WildHunt_Eredin,
            EBG_Combat_Olgierd,
            EBG_Combat_Caretaker,
            EBG_Combat_Dettlaff_Vampire,
            EBG_Combat_Gregoire,
            EBG_Combat_Dettlaff_Minion,
            EBG_None
        }
        public static string ActionStr(Acts graphEnum)
        {
            switch (graphEnum)
            {
                case Acts.EBG_Combat_Shield: return "Shield";
                case Acts.EBG_Combat_1Handed_Sword: return "sword_1handed";
                case Acts.EBG_Combat_1Handed_Axe: return "sword_1handed";
                case Acts.EBG_Combat_1Handed_Blunt: return "sword_1handed";
                case Acts.EBG_Combat_1Handed_Any: return "sword_1handed";
                case Acts.EBG_Combat_2Handed_Sword: return "sword_2handed";
                case Acts.EBG_Combat_2Handed_Any: return "TwoHanded";
                case Acts.EBG_Combat_2Handed_Hammer: return "TwoHanded";
                case Acts.EBG_Combat_2Handed_Axe: return "TwoHanded";
                case Acts.EBG_Combat_2Handed_Halberd: return "TwoHanded";
                case Acts.EBG_Combat_2Handed_Spear: return "TwoHanded";
                case Acts.EBG_Combat_2Handed_Staff: return "TwoHanded";
                case Acts.EBG_Combat_Fists: return "FistFight";
                case Acts.EBG_Combat_Bow: return "Bow";
                case Acts.EBG_Combat_Crossbow: return "Bow";
                case Acts.EBG_Combat_Witcher: return "Witcher";
                case Acts.EBG_Combat_Sorceress: return "Sorceress";
                case Acts.EBG_Combat_WildHunt_Imlerith: return "Imlerith";
                case Acts.EBG_Combat_WildHunt_Imlerith_Second_Stage: return "ImlerithSecondStage";
                case Acts.EBG_Combat_WildHunt_Caranthir: return "Caranthir";
                case Acts.EBG_Combat_WildHunt_Caranthir_Second_Stage: return "CaranthirSecondStage";
                case Acts.EBG_Combat_WildHunt_Eredin: return "Eredin";
                case Acts.EBG_Combat_Olgierd: return "Olgierd";
                case Acts.EBG_Combat_Caretaker: return "Exploration";
                case Acts.EBG_Combat_Dettlaff_Vampire: return "DettlaffVampire";
                case Acts.EBG_Combat_Gregoire: return "Exploration";
                case Acts.EBG_Combat_Dettlaff_Minion: return "DettlaffMinion";
                case Acts.EBG_None: return "None";
                default: return "";
            }
        }


        //JumpClimb
        //Exploration
        //JumpClimb
        //TraverseExploration
        //Swimming
        //HorseRiding
        //Sailing
        //SailingPassive
        enum EAreaName
        {
            AN_Undefined,
            AN_NMLandNovigrad,
            AN_Skellige_ArdSkellig,
            AN_Kaer_Morhen,
            AN_Prologue_Village,
            AN_Wyzima,
            AN_Island_of_Myst,
            AN_Spiral,
            AN_Prologue_Village_Winter,
            AN_Velen,
            AN_CombatTestLevel
        }
        public enum RecvSendTypes
        {
            //RET-FROMHOST TYPES
            RET_PLAYERDATAS,
            RET_ACCESS,
            RET_CONNECTED,
            RET_SAVEFILE,

            //SND-TOHOST TYPES
            SND_PLAYERINFO,
            SND_HOSTINFO,
            SND_CHATMSG,
            SND_COMMAND,
            SND_SAVEFILE,
            SND_DISCONNECTED,
            SND_PLAYERPOSITION,
            SND_PLAYERONHORSE,
            SND_PLAYERCOMBATTARGET,
            SND_PLAYERCOMBATTARGETKILL,
            SND_PLAYERHORSEPOSITION,
            SND_PLAYERROTATION,
            SND_PLAYERSTATE,

            //RCV-TOCLIENT TYPES
            RCV_ACCESSSHELL,
            RCV_PLAYERCOMBATTARGETKILL,
            RCV_PLAYERCOMBATTARGET,
            RCV_PLAYERONHORSE,
            RCV_HOSTINFO,
            RCV_PLAYERHORSEPOSITION,
            RCV_PLAYERINFO,
            RCV_COMMANDRESPONSE,
            RCV_PLAYERPOSITION,
            RCV_PLAYERROTATION,
            RCV_PLAYERSTATE,
            RCV_TIME,
            RCV_ITEMDATA,
            RCV_ENTITYSTATE,
            RCV_ENTITYDATA,
            RCV_ENTITYHP,
            RCV_ENTITYINVENTORYDATA,
            RCV_BOXCRATEDATA,
            RCV_BOXSTATE,
            RCV_QUESTDATA,
            RCV_ITERACTEDDATA,
            RCV_MAGICUSED,
            RCV_ATTACKUSED,
            RCV_PLAYERCHARED,
            RCV_CHATMSG,
            RCV_DISCONNECTED
        }
        public static List<string> BlockedCMDS = new List<string>()
        {
            "addkey",
            "god",
            "healme",
            "setlevel",
            "levelup",
            "addexp",
            "learnskill",
            "cat",
            "addmoney",
            "removemoney",
            "killall",
            "goto",
            "ShowAllFT",
            "ShowPins",
            "winGwint",
            "addgwintcards",
            "additem"
        };
        public static List<string> States = new List<string>()
        {
            "Idle",
            "Swim",
            "Climb",
            "CombatExploration",
            "TraverseExploration",
            "Jump",
            "Slide",
            "Interaction",
            "Land",
            "Roll",
            "IdleJump"
        };
        public static List<string> NpcsPlayer = new List<string>()
        {
            "characters\\npc_entities\\main_npc\\eskel.w2ent",
            "characters\\npc_entities\\main_npc\\lambert.w2ent",
            "characters\\npc_entities\\secondary_npc\\letho.w2ent",
            "characters\\npc_entities\\main_npc\\roche.w2ent",
            "characters\\npc_entities\\main_npc\\geralt_npc.w2ent"
        };
        public static List<string> Monsters = new List<string>()
        {
        "ce_giant",
        "cegiant",
        "iren",
        "ees",
        "lack_troll",
        "lacktroll",
        "ave_troll",
        "avetroll",
        "ockatrice",
        "troll",
        "troll_black",
        "djinn",
        "drowner",
        "ddead",
        "drowneddead",
        "drowned_dead",
        "nekker",
        "nekker_warrior",
        "bies",
        "lessog",
        "leszy",
        "leshy",
        "leshen",
        "lessun",
        "dao",
        "ifryt",
        "ifrit",
        "golem",
        "arachas",
        "arachas_armored",
        "armoredarachas",
        "arachas_poison",
        "poisonarachas",
        "poisonedarachas",
        "warewolf",
        "werewolf",
        "lycanthrope",
        "lycan",
        "endriaga",
        "endrega",
        "endriaga_worker",
        "endriaga_tailed",
        "endriaga_spikey",
        "wraith_lvl1",
        "wyvern",
        "basilisk",
        "harpy",
        "gryphon",
        "griffon",
        "wolf",
        "wolf_alpha",
        "wolf_white",
        "evil_dog",
        "wild_dog",
        "simulacrum",
        "erynia",
        "fogling",
        "forktail",
        "fugas",
        "gargoyle",
        "ghoul",
        "ghoul_lesser",
        "alghoul",
        "wildhunt_mage",
        "gravehag",
        "miscreant",
        "nightwraith",
        "noonwraith",
        "pesta",
        "volcanic_gryphon",
        "wraith",
        "waterhag",
        "czart",
        "willo_wisp",
        "willowisp",
        "katakan",
        "katakan_large",
        "katakanlarge",
        "gravehag_barons_wife",
        "baronswife",
        "ekima",
        "ekimma",
        "lamia",
        "bear",
        "bear_black",
        "bear_berserker",
        "bear_grizzly",
        "grizzly",
        "bear_white",
        "cyclop",
        "cyclops",
        "rotfiend",
        "rotfiend_large",
        "succubus",
        "heart",
        "him",
        "frozen_soldier",
        "ice_golem",
        "botchling",
        "alghoul_lvl1",
        "alghoul_lvl2",
        "alghoul_lvl3",
        "alghoul_mh",
        "arachas_lvl1",
        "arachas_lvl2__armored",
        "arachas_lvl3__poison",
        "arachas_mh__poison",
        "basilisk_lvl1",
        "bear_berserker_lvl1",
        "bear_lvl1__black",
        "bear_lvl2__grizzly",
        "bear_lvl3__white",
        "bies_lvl1",
        "bies_lvl2",
        "bies_mh",
        "black_mage_lvl1",
        "blood_flies",
        "burnedman_lvl1",
        "cockatrice_lvl1",
        "cockatrice_mh",
        "cyclop_lvl1",
        "czart_lvl1",
        "czart_mh",
        "drowner_lvl1",
        "drowner_lvl2",
        "drowner_lvl3",
        "drowner_lvl4__dead",
        "elemental_dao_lvl1",
        "elemental_dao_lvl2",
        "elemental_dao_lvl3__ice",
        "elemental_dao_mh",
        "endriaga_lvl1__worker",
        "endriaga_lvl2__tailed",
        "endriaga_lvl3__spikey",
        "fogling_lvl1",
        "fogling_lvl1__doppelganger",
        "fogling_lvl2",
        "fogling_lvl3__willowisp",
        "fogling_mh",
        "forktail_lvl1",
        "forktail_lvl2",
        "forktail_mh",
        "fugas_lvl1",
        "fugas_lvl2",
        "gargoyle_lvl1",
        "ghoul_lvl1",
        "ghoul_lvl2",
        "ghoul_lvl3",
        "golem_lvl1",
        "golem_lvl2",
        "golem_lvl2__ifryt",
        "golem_lvl3",
        "gryphon_lvl1",
        "gryphon_lvl2",
        "gryphon_lvl3__volcanic",
        "gryphon_mh__volcanic",
        "hag_grave_lvl1",
        "hag_grave_lvl1__barons_wife",
        "hag_grave__mh",
        "hag_water_lvl1",
        "hag_water_lvl2",
        "hag_water_mh",
        "harpy_lvl1",
        "harpy_lvl2",
        "harpy_lvl3__erynia",
        "lessog_lvl1",
        "lessog_lvl2__ancient",
        "lessog_mh",
        "nekker_lvl1",
        "nekker_lvl2",
        "nekker_lvl3__warrior",
        "nekker_mh__warrior",
        "nightwraith_lvl1",
        "nightwraith_lvl1__doppelganger",
        "nightwraith_mh",
        "noonwraith_lvl1",
        "noonwraith_lvl1__doppelganger",
        "noonwraith_mh",
        "rotfiend_lvl1",
        "rotfiend_lvl2",
        "siren_lvl1",
        "siren_lvl2__lamia",
        "siren_mh__lamia",
        "troll_cave_lvl1",
        "troll_cave_lvl2",
        "troll_cave_lvl3__ice",
        "troll_cave_mh__black",
        "vampire_ekima_lvl1",
        "vampire_ekima_mh",
        "vampire_katakan_lvl1",
        "vampire_katakan_mh",
        "werewolf_lvl1",
        "werewolf_lvl2",
        "werewolf_lvl3__lycan",
        "werewolf_lvl4__lycan",
        "wild_dog_lvl1",
        "wildhunt_minion_lvl1",
        "wildhunt_minion_lvl2",
        "wildhunt_minion_mh",
        "wolf_lvl1",
        "wolf_lvl1__summon",
        "wolf_lvl1__summon_were",
        "wolf_lvl2__alpha",
        "wolf_white_lvl2",
        "wolf_white_lvl3__alpha",
        "wraith_lvl1",
        "wraith_mh",
        "wyvern_lvl1",
        "wyvern_lvl2",
        "wyvern_mh",
        "_quest__bear_grizzly_honey",
        "_quest__endriaga_spiral",
        "_quest__fogling",
        "_quest__godling",
        "_quest__him",
        "_quest__miscreant",
        "_quest__miscreant_greater",
        "_quest__noonwright_pesta",
        "_quest__werewolf",
        "_quest__witch_1",
        "_quest__witch_2",
        "_quest__witch_3",
        "wildhunt_sword",
        "wild_hunt_sword",
        "wildhunt_axe",
        "wild_hunt_axe",
        "wildhunt_halberd",
        "wild_hunt_halberd",
        "wildhunt_hammer",
        "wild_hunt_hammer",
        "wildhunt_spear",
        "wild_hunt_spear",
        "wildhunt_minion",
        "wild_hunt_minion",
        "witch1",
        "witch_1",
        "witch2",
        "witch_2",
        "witch3",
        "witch_3",
        "witch_q105",
        "eredin",
        "imlerith",
        "caranthir",
        "bat",
        "cat",
        "chicken",
        "cow",
        "crab",
        "crow",
        "deer",
        "dog",
        "fish_kingfish",
        "fish_mackerel",
        "fish_roach",
        "fish_tuna",
        "goat",
        "goose",
        "goose_leader",
        "hare",
        "mountain_goat",
        "owl",
        "pig",
        "pigeon",
        "ram",
        "rat",
        "rooster",
        "seagull",
        "sheep",
        "snow_deer",
        "snow_rabbit",
        "sparrow",
        "swallow",
        "toad",
        "whale",
        "geralt_inventory_internal",
        "witcher",
        "vesemir",
        "yennefer",
        "triss",
        "keira",
        "grenn",
        "cirilla",
        "zoltan",
        "sorceress",
        "fists",
        "fists_medium",
        "fists_hard",
        "shield",
        "shield_axe",
        "shield_mace",
        "shield_hard",
        "hammer2h",
        "hammer2h_hard",
        "axe2h",
        "halberd2h",
        "spear2h",
        "bow",
        "bow_hard",
        "sword1h",
        "sword1h_easy",
        "sword1h_hard",
        "sword1h_super_hard",
        "axe1h",
        "axe1h_hard",
        "club1h",
        "club1h_hard",
        "1hand",
        "1handed",
        "sword2h",
        "shortsword_hard",
        "bowman",
        "bowman_hard",
        "xbow",
        "rider",
        "shovel_test",
        "pitchfork",
        "dwarf_sword1h",
        "dwarf_fists",
        "e3_bandit_sword",
        "e3_bandit_bow",
        "e3_bandit_shield",
        "e3_bandit_2hand",
        "e3_werewolf"
        };
        public static string GetNameMonsters(int id)
        {
            if (Monsters.Count > id)
                return Monsters[id];
            else
                return "";
        }
        public static string GetNamePlayerNPC(int id)
        {
            if (NpcsPlayer.Count > 0)
                return NpcsPlayer[id];
            else
                return "";
        }
        private static double GetDistanceTO(this Vector3 a, Vector3 b)
        {
            return Math.Sqrt(Math.Pow((b.x - a.x), 2) + Math.Pow((b.y - a.y), 2));
        }
        private static double GetDistanceVTOV(Vector3 a, Vector3 b)
        {
            return Math.Sqrt(Math.Pow((b.x - a.x), 2) + Math.Pow((b.y - a.y), 2));
        }
        public static bool Comapare(this Vector3 a, Vector3 b)
        {
            return !(a.x == b.x & a.y == b.y & a.y == b.y);
        }
        public static bool Comapare(this Quaternion a, Quaternion b)
        {
            return !(a.x == b.x & a.y == b.y & a.y == b.y);
        }
        public static float VectorAngleF(this Vector3 a, Vector3 b)
        {
            return (float)Math.Atan2(b.y - a.y, b.x - a.x);
        }
        public static int VectorAngleI(this Vector3 a, Vector3 b)
        {
            return (int)Math.Atan2(b.y - a.y, b.x - a.x);
        }
        public static double VectorAngleD(this Vector3 a, Vector3 b)
        {
            return (double)Math.Atan2(b.y - a.y, b.x - a.x);
        }
    }
}
