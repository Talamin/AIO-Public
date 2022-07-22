//using robotManager.Helpful;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using wManager.Wow.Class;
//using wManager.Wow.Enums;
//using wManager.Wow.Helpers;
//using wManager.Wow.ObjectManager;

//public static class TrainerManager
//{
//    private static Npc DruidTrainer = new Npc
//    {
//        Name = "Turak Runetotem",
//        Entry = 3033,
//        Faction = Npc.FactionType.Horde,
//        ContinentId = ContinentId.Kalimdor,
//        Position = new Vector3(-1039.41, -281.56, 159.0305),
//        CanFlyTo = false,
//        Type = Npc.NpcType.DruidTrainer,
//    };
//    private static Npc ShamanTrainer = new Npc
//    {
//        Name = "Sian'tsu",
//        Entry = 3403,
//        Faction = Npc.FactionType.Horde,
//        ContinentId = ContinentId.Kalimdor,
//        Position = new Vector3(1932.88, -4211.3, 42.32275),
//        CanFlyTo = false,
//        Type = Npc.NpcType.ShamanTrainer,
//    };

//    private static Npc PaladinTrainer = new Npc
//    {
//        Name = "Master Pyreanor",
//        Entry = 23128,
//        Faction = Npc.FactionType.Horde,
//        ContinentId = ContinentId.Kalimdor,
//        Position = new Vector3(1939.69, -4133.33, 41.14468),
//        CanFlyTo = false,
//        Type = Npc.NpcType.PaladinTrainer,
//    };

//    private static Npc PriestTrainer = new Npc
//    {
//        Name = "Ur'kyo",
//        Entry = 6018,
//        Faction = Npc.FactionType.Horde,
//        ContinentId = ContinentId.Kalimdor,
//        Position = new Vector3(1452.43, -4179.82, 44.27711),
//        CanFlyTo = false,
//        Type = Npc.NpcType.PriestTrainer,
//    };

//    private static Npc MageTrainer = new Npc
//    {
//        Name = "Enyo",
//        Entry = 5883,
//        Faction = Npc.FactionType.Horde,
//        ContinentId = ContinentId.Kalimdor,
//        Position = new Vector3(1472.48, -4224.6, 43.18612),
//        CanFlyTo = false,
//        Type = Npc.NpcType.MageTrainer,
//    };

//    private static Npc WarlockTrainer = new Npc
//    {
//        Name = "Grol'dar",
//        Entry = 3324,
//        Faction = Npc.FactionType.Horde,
//        ContinentId = ContinentId.Kalimdor,
//        Position = new Vector3(1844.21, -4353.61, -14.6542),
//        CanFlyTo = false,
//        Type = Npc.NpcType.WarlockTrainer,
//    };

//    private static Npc RogueTrainer = new Npc
//    {
//        Name = "Shenthul",
//        Entry = 3401,
//        Faction = Npc.FactionType.Horde,
//        ContinentId = ContinentId.Kalimdor,
//        Position = new Vector3(1771.21, -4284.42, 7.980936),
//        CanFlyTo = false,
//        Type = Npc.NpcType.RogueTrainer,
//    };

//    private static Npc WarriorTrainer = new Npc
//    {
//        Name = "Sorek",
//        Entry = 3354,
//        Faction = Npc.FactionType.Horde,
//        ContinentId = ContinentId.Kalimdor,
//        Position = new Vector3(1970.95, -4808.17, 56.99199),
//        CanFlyTo = false,
//        Type = Npc.NpcType.WarriorTrainer,
//    };

//    private static Npc HunterTrainer = new Npc
//    {
//        Name = "Xor'juul",
//        Entry = 3406,
//        Faction = Npc.FactionType.Horde,
//        ContinentId = ContinentId.Kalimdor,
//        Position = new Vector3(2084.96, -4623.77, 58.82039),
//        CanFlyTo = false,
//        Type = Npc.NpcType.HunterTrainer,
//    };

//    public static List<Npc> Trainers = new List<Npc>
//    {
//        DruidTrainer, ShamanTrainer, PaladinTrainer, PriestTrainer,  MageTrainer,  RogueTrainer,  WarlockTrainer, WarriorTrainer,  HunterTrainer
//    };
//    public static void TrainerCheck()
//    {
//        try
//        {
//            foreach (var Trainer in Trainers)
//            {
//                if (!NpcDB.ListNpc.Contains(Trainer))
//                {
//                    NpcDB.AddNpc(Trainer);
//                    Main.Log("Added: " + Trainer);
//                    Thread.Sleep(50);
//                }
//            }
//        }
//        catch { Main.Log("something gone wrong"); }
//    }
//}