using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace TerrariaChaosEditionUnleashed.Utility
{
    /**
     * Contains helpful functions and data such as a list of all negative and positive buffs.
     */
    public static class ChaosUtilities
    {
        public static Dictionary<int,int> reverseBuff = new Dictionary<int, int>(){{BuffID.Regeneration, BuffID.Poisoned}, {BuffID.Poisoned, BuffID.Regeneration},
            {BuffID.RapidHealing, BuffID.Venom}, {BuffID.Venom, BuffID.RapidHealing}, {BuffID.OnFire, BuffID.Regeneration }, { BuffID.OnFire3, BuffID.RapidHealing },
            { BuffID.Darkness, BuffID.NightOwl }, { BuffID.NightOwl, BuffID.Darkness }, { BuffID.Daybreak, BuffID.RapidHealing }, { BuffID.Swiftness, BuffID.Slow},
            { BuffID.MagicPower, BuffID.ManaSickness }, { BuffID.ManaSickness, BuffID.MagicPower }, { BuffID.HeartLamp, BuffID.Poisoned }, { BuffID.Ironskin, BuffID.BrokenArmor },
            { BuffID.Frostburn, BuffID.Regeneration }, { BuffID.Frostburn2, BuffID.RapidHealing }, { BuffID.Sharpened, BuffID.Weak }, { BuffID.Weak, BuffID.Sharpened },
            { BuffID.MagicPower, BuffID.Silenced }, { BuffID.Rage, BuffID.WitheredWeapon }, { BuffID.CursedInferno, BuffID.RapidHealing },{ BuffID.Ichor, BuffID.Ironskin },
            { BuffID.BrokenArmor, BuffID.Endurance }, { BuffID.BeetleEndurance1, BuffID.Ironskin }, { BuffID.BeetleEndurance2, BuffID.BrokenArmor }, 
            { BuffID.BeetleEndurance3, BuffID.BrokenArmor }, { BuffID.Chilled, BuffID.Swiftness }, { BuffID.Electrified, BuffID.RapidHealing }, { BuffID.Campfire, BuffID.Poisoned },
            { BuffID.WaterCandle, BuffID.PeaceCandle }, { BuffID.PeaceCandle, BuffID.WaterCandle }, { BuffID.Hunger, BuffID.WellFed }, { BuffID.ShadowFlame, BuffID.RapidHealing },
            { BuffID.DryadsWard, BuffID.DryadsWardDebuff }, { BuffID.DryadsWardDebuff, BuffID.DryadsWard }, { BuffID.Midas, BuffID.WeaponImbueGold }, { BuffID.WeaponImbueGold, BuffID.Midas },
            { BuffID.BetsysCurse, BuffID.Ironskin }, { BuffID.WellFed, BuffID.Hunger }, { BuffID.Tipsy, BuffID.WellFed }, { BuffID.Slow, BuffID.Swiftness }, { BuffID.Calm, BuffID.Battle },
            { BuffID.Battle, BuffID.Calm }, { BuffID.Blackout, BuffID.NightOwl }, {BuffID.OgreSpit, BuffID.Swiftness}
        };

        public static int[] goodBuffs = { BuffID.ObsidianSkin, BuffID.Regeneration, BuffID.Swiftness, BuffID.Gills, BuffID.Ironskin, BuffID.ManaRegeneration, BuffID.MagicPower, BuffID.Featherfall,
            BuffID.Spelunker, BuffID.Invisibility, BuffID.Shine, BuffID.NightOwl, BuffID.Battle, BuffID.Thorns, BuffID.WaterWalking, BuffID.Archery, BuffID.Hunter, BuffID.Gravitation,
            BuffID.ShadowOrb, BuffID.WellFed, BuffID.FairyBlue, BuffID.Werewolf, BuffID.Clairvoyance, BuffID.Merfolk, BuffID.PaladinsShield, BuffID.Honey, BuffID.Pygmies, BuffID.TikiSpirit,
            BuffID.Wisp, BuffID.RapidHealing, BuffID.ShadowDodge, BuffID.LeafCrystal, BuffID.IceBarrier, BuffID.Panic, BuffID.WeaponImbueVenom, BuffID.WeaponImbueCursedFlames,
            BuffID.WeaponImbueGold, BuffID.WeaponImbueIchor, BuffID.WeaponImbueNanites, BuffID.WeaponImbueConfetti, BuffID.WeaponImbuePoison, BuffID.Campfire, BuffID.HeartLamp,
            BuffID.AmmoBox, BuffID.BeetleEndurance1, BuffID.BeetleEndurance2, BuffID.BeetleEndurance3, BuffID.BeetleMight1, BuffID.BeetleMight2, BuffID.BeetleMight3, BuffID.FairyRed,
            BuffID.FairyGreen, BuffID.Mining, BuffID.Heartreach, BuffID.Calm, BuffID.Builder, BuffID.Titan, BuffID.Flipper, BuffID.Summoning, BuffID.Dangersense, BuffID.AmmoReservation,
            BuffID.Lifeforce, BuffID.Endurance, BuffID.Rage, BuffID.Inferno, BuffID.Wrath, BuffID.MinecartLeft, BuffID.Fishing, BuffID.Sonar, BuffID.Crate, BuffID.Warmth, BuffID.HornetMinion,
            BuffID.ImpMinion, BuffID.BunnyMount, BuffID.PigronMount, BuffID.SlimeMount, BuffID.TurtleMount, BuffID.BeeMount, BuffID.SpiderMinion, BuffID.TwinEyesMinion, BuffID.PirateMinion,
            BuffID.MinecartRight, BuffID.SharknadoMinion, BuffID.UFOMinion, BuffID.ScutlixMount, BuffID.Sunflower, BuffID.MonsterBanner, BuffID.Bewitched, BuffID.SoulDrain, BuffID.MagicLantern,
            BuffID.CrimsonHeart, BuffID.PeaceCandle, BuffID.StarInBottle, BuffID.Sharpened, BuffID.DrillMount, BuffID.DeadlySphere, BuffID.UnicornMount, BuffID.DryadsWard,
            BuffID.MinecartLeftMech, BuffID.MinecartRightMech, BuffID.CuteFishronMount, BuffID.SolarShield1, BuffID.SolarShield2, BuffID.SolarShield3, BuffID.NebulaUpLife1,
            BuffID.NebulaUpLife2, BuffID.NebulaUpLife3, BuffID.NebulaUpDmg1, BuffID.NebulaUpDmg2, BuffID.NebulaUpDmg3, BuffID.StardustMinion, BuffID.MinecartLeftWood, BuffID.MinecartRightWood,
            BuffID.StardustDragonMinion, BuffID.StardustGuardianMinion, BuffID.SuspiciousTentacle, BuffID.SugarRush, BuffID.BasiliskMount, BuffID.BallistaPanic, BuffID.WellFed2,
            BuffID.WellFed3, BuffID.DesertMinecartLeft, BuffID.DesertMinecartRight, BuffID.FishMinecartLeft, BuffID.FishMinecartRight, BuffID.GolfCartMount, BuffID.BatOfLight,
            BuffID.VampireFrog, BuffID.CatBast, BuffID.BabyBird, BuffID.BeeMinecartLeft, BuffID.BeeMinecartRight, BuffID.LadybugMinecartLeft, BuffID.LadybugMinecartRight,
            BuffID.PigronMinecartLeft, BuffID.PigronMinecartRight, BuffID.SunflowerMinecartLeft, BuffID.SunflowerMinecartRight, BuffID.HellMinecartLeft, BuffID.HellMinecartRight,
            BuffID.WitchBroom, BuffID.ShroomMinecartLeft, BuffID.ShroomMinecartRight, BuffID.AmethystMinecartLeft, BuffID.AmethystMinecartRight, BuffID.TopazMinecartLeft,
            BuffID.TopazMinecartRight, BuffID.SapphireMinecartLeft, BuffID.SapphireMinecartRight, BuffID.EmeraldMinecartLeft, BuffID.EmeraldMinecartRight, BuffID.RubyMinecartLeft,
            BuffID.RubyMinecartRight, BuffID.DiamondMinecartLeft, BuffID.DiamondMinecartRight, BuffID.AmberMinecartLeft, BuffID.AmberMinecartRight, BuffID.BeetleMinecartLeft,
            BuffID.BeetleMinecartRight, BuffID.MeowmereMinecartLeft, BuffID.MeowmereMinecartRight, BuffID.PartyMinecartLeft, BuffID.PartyMinecartRight, BuffID.PirateMinecartLeft,
            BuffID.PirateMinecartRight, BuffID.SteampunkMinecartLeft, BuffID.SteampunkMinecartRight, BuffID.Lucky, BuffID.StormTiger, BuffID.CoffinMinecartLeft, BuffID.CoffinMinecartRight,
            BuffID.Smolstar, BuffID.DiggingMoleMinecartLeft, BuffID.DiggingMoleMinecartRight, BuffID.PaintedHorseMount, BuffID.MajesticHorseMount, BuffID.DarkHorseMount, BuffID.PogoStickMount,
            BuffID.PirateShipMount, BuffID.SpookyWoodMount, BuffID.SantankMount, BuffID.WallOfFleshGoatMount, BuffID.DarkMageBookMount, BuffID.LavaSharkMount, BuffID.TitaniumStorm,
            BuffID.SwordWhipPlayerBuff, BuffID.ScytheWhipPlayerBuff, BuffID.CoolWhipPlayerBuff, BuffID.ThornWhipPlayerBuff, BuffID.QueenSlimeMount, BuffID.EmpressBlade, BuffID.FlinxMinion,
            BuffID.AbigailMinion, BuffID.HeartyMeal, BuffID.FartMinecartLeft, BuffID.FartMinecartRight, BuffID.CoolWhipPlayerBuff, BuffID.WolfMount, BuffID.BiomeSight,
            BuffID.TerraFartMinecartLeft, BuffID.TerraFartMinecartRight, BuffID.WarTable,
        };
        public static int[] badBuffs = { BuffID.Poisoned, BuffID.PotionSickness, BuffID.Darkness, BuffID.Cursed, BuffID.OnFire, BuffID.Tipsy, BuffID.Bleeding, BuffID.Confused, BuffID.Slow, BuffID.Weak,
            BuffID.Silenced, BuffID.BrokenArmor, BuffID.Horrified, BuffID.TheTongue, BuffID.CursedInferno, BuffID.Frostburn, BuffID.Chilled, BuffID.Frozen, BuffID.Burning, BuffID.Suffocation,
            BuffID.Ichor, BuffID.Venom, BuffID.Midas, BuffID.Blackout, BuffID.WaterCandle, BuffID.ChaosState, BuffID.ManaSickness, BuffID.Wet, BuffID.Lovestruck, BuffID.Stinky, BuffID.Slimed,
            BuffID.Electrified, BuffID.MoonLeech, BuffID.Rabies, BuffID.Webbed, BuffID.ShadowFlame, BuffID.Stoned, BuffID.Dazed, BuffID.Obstructed, BuffID.VortexDebuff, BuffID.BoneJavelin,
            BuffID.StardustMinionBleed, BuffID.DryadsWardDebuff, BuffID.Daybreak, BuffID.WindPushed, BuffID.WitheredArmor, BuffID.WitheredWeapon, BuffID.OgreSpit, BuffID.NoBuilding,
            BuffID.BetsysCurse, BuffID.Oiled, BuffID.BlandWhipEnemyDebuff, BuffID.SwordWhipNPCDebuff, BuffID.ScytheWhipEnemyDebuff, BuffID.CoolWhipNPCDebuff, BuffID.FlameWhipEnemyDebuff,
            BuffID.ThornWhipNPCDebuff, BuffID.RainbowWhipNPCDebuff, BuffID.MaceWhipNPCDebuff, BuffID.GelBalloonBuff, BuffID.BrainOfConfusionBuff, BuffID.OnFire3, BuffID.Frostburn2,
            BuffID.BoneWhipNPCDebuff, BuffID.NeutralHunger, BuffID.Hunger, BuffID.Starving, BuffID.TentacleSpike, BuffID.BloodButcherer, BuffID.ShadowCandle, BuffID.Shimmer,
        };
        public static int[] petsBuffs = { BuffID.PetBunny, BuffID.BabyPenguin, BuffID.PetTurtle, BuffID.BabyEater, BuffID.BabySkeletronHead, BuffID.BabyHornet, BuffID.PetLizard, BuffID.PetParrot,
            BuffID.BabyTruffle, BuffID.PetSapling, BuffID.BabyDinosaur, BuffID.BabySlime, BuffID.EyeballSpring, BuffID.BabySnowman, BuffID.PetSpider, BuffID.Squashling, BuffID.Ravens,
            BuffID.BlackCat, BuffID.CursedSapling, BuffID.Rudolph, BuffID.Puppy, BuffID.BabyGrinch, BuffID.ZephyrFish, BuffID.MiniMinotaur, BuffID.BabyFaceMonster, BuffID.CompanionCube,
            BuffID.PetDD2Gato, BuffID.PetDD2Ghost, BuffID.PetDD2Dragon, BuffID.SugarGlider, BuffID.SharkPup, BuffID.UpbeatStar, BuffID.LilHarpy, BuffID.FennecFox, BuffID.GlitteryButterfly,
            BuffID.BabyImp, BuffID.BabyRedPanda, BuffID.Plantero, BuffID.Flamingo, BuffID.DynamiteKitten, BuffID.BabyWerewolf, BuffID.ShadowMimic, BuffID.VoltBunny, BuffID.KingSlimePet,
            BuffID.EyeOfCthulhuPet, BuffID.EaterOfWorldsPet, BuffID.BrainOfCthulhuPet, BuffID.SkeletronPet, BuffID.QueenBeePet, BuffID.DestroyerPet, BuffID.TwinsPet, BuffID.SkeletronPrimePet,
            BuffID.PlanteraPet, BuffID.GolemPet, BuffID.DukeFishronPet, BuffID.LunaticCultistPet, BuffID.MoonLordPet, BuffID.EverscreamPet, BuffID.PumpkingPet, BuffID.FairyQueenPet,
            BuffID.IceQueenPet, BuffID.MartianPet, BuffID.DD2OgrePet, BuffID.DD2BetsyPet, BuffID.QueenSlimePet, BuffID.BerniePet, BuffID.GlommerPet, BuffID.DeerclopsPet, BuffID.PigPet,
            BuffID.ChesterPet, BuffID.DualSlimePet, BuffID.JunimoPet, BuffID.BlueChickenPet, BuffID.Spiffo, BuffID.CavelingGardener, BuffID.DirtiestBlock
        };

        public static Player ClosestPlayer(Vector2 point)
        {
            Player closestPlayer = Main.player[0];
            Vector2 minDist = Vector2.One * float.MaxValue;
            foreach (Player player in Main.player)
            {
                Vector2 dist = (player.position - point);
                bool smaller = dist.Length() < minDist.Length();
                minDist = smaller ? dist : minDist;
                closestPlayer = smaller ? player : closestPlayer;
            }
            return closestPlayer;
        }

        public static char GetRandomAlphaNum()
        {
            int randVal = Main.rand.Next(10 + 26 + 26);
            randVal += 48;
            if(randVal >= 58)
            {
                randVal += 7;
            }
            if(randVal >= 91)
            {
                randVal += 6;
            }
            return (char)randVal;
        }
    }
}
