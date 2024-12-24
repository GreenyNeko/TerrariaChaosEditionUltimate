using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using TerrariaChaosEditionUnleashed.Core;

namespace TerrariaChaosEditionUnleashed
{
    /***
     * Handles active chaos effects.
     */
    class ChaosManager
    {
        int nextEffectId;
        public enum ChaosEffects
        {
            // 21 to fix&/verify
            // v1 24/24 | 16/24 verified
            CHANGE_HARDMODE, // works
            SKIP_TIME, // works
            NO_GRAVITY, // works
            RANDOM_MUSIC,
            RANDOM_SOUND, // works
            RANDOM_NPC, // works
            NO_CREATIVITY,
            RANDOM_TELEPORT, // works
            LIFE_MANA_SWAP, // works
            MAGIC_MIRROR, // works?
            RECOLORED_NPCS, // works
            RANDOM_BUFF,
            RANDOM_DEBUFF, // works
            RANDOM_PET, // works
            HEALING_HURTS, // prolly works
            RANDOM_CHAT_MSG, // works
            RANDOM_MONOLITH_FX,
            DANGEROUS_XRAY, // works
            RAND_TILE_CONVERT, // assume it works
            RAND_NPC_FX, // prolly works?
            MERRY_XMAS, // prolly works
            RANDOM_EVENT, // works?
            NO_FLYING_AND_WORMING,
            SMASH_BROS,
            // v2 25/25 | 18/25 verified
            IMMENSE_SPAWN_RATE, // works
            MAX_LIFE_MANA, // works
            BUTTER_FINGERS, // works
            INFINITE_BUGS, // works
            SHADOW_ENEMIES, // works
            NPC_SPIN_2_WIN, // works
            ULTIMATE_BOSS, // works
            WORLD_BLESSINGS, //works?
            TALKING_CREATURES, //works
            ROD_OF_DISCORD, //works
            RAND_PROJ_FX, // prolly works
            PLAYER_TORNADO, // works
            NPC_GROW_SHRINK, // works
            JUNGLE_GROWS, // prolly works
            SWAP_PLACES, // works?
            EXPLOSIVE_DEATH, // works
            RAND_ENEMY_FX_II, // prolly works
            SONIC_HEALTH, // works
            UPSIDE_DOWN, // works
            RANDOM_CRAFT, // works
            RANDOM_TILE_FX, // prolly works
            RECOLORED_TILES, // works
            ACCUMULATING_VELOCITY,
            VERIFY_HUMAN, // works
            NON_BINARY_GENDER, //works
            // v3 25/25 | 21/25 verified
            RAND_LIFE_MANA, // works
            CURSED_BY_LUCK, // assume it works
            BIG_WEAPONS, // works
            ITEMS_GO_HAM, // works
            HALLO_HALLOWEEN, // prolly works
            RAND_NUMBER_FX, // works
            REINFORCEMENTS, // works
            INVERTED_ENEMIES,
            ALWAYS_WET, // works
            DARKNESS_ENSUES, // works
            INVERSE_DE_BUFF, // works
            RANDOM_CREDITZ, // works
            FAKE_PICKUP, // works
            RANDOM_AUDIO_PITCHES, // works
            APRIL_WEATHER,
            SPOOKY_GHOST, //works
            OLD_SCHOOL_CAM, // works
            PLAYER_SOLAR_SYSTEM, // works
            READABLE_UI, // works
            WEIRD_LIGHTING, // works
            GRAVEYARD_SHIFT, // works
            RANDOM_PYLON_FX,
            RAND_PROJ_FX_II, // prolly works
            FALL_SENSITIVITY, // works
            RANDOM_TILE_FX_II, // works?
            // unleashed 25 | 23/25 veified
            CRAZY_GRAVITY,
            ABOMINATION, // works
            TIME_TRAVEL, // works
            PAIN_SHIFTS_REALITY, // works
            HUGE_WORLD, // works
            RANDOM_HEALTH_BARS, // works
            UNMISSABLE_CURSOR, // works
            NEARSIGHTED, // works
            ENEMIES_STUN, // works
            UP_OR_DIE, // works
            RAND_ITEM_DROP_FX, // works
            INVISIBLE_INVENTORY_ITEMS, // works
            RANDOM_ITEM_ICONS, // works
            RANDOM_NPC_SPRITES, // works
            SHIMMERING, // works
            HOT_WAWA, // works
            POPUP_MIX_UP, // works
            BREAKOUT, // works
            RANDOM_TOOLTIP_EFFECTS, // works
            FLATRARRIA, // works
            RAND_BUFF_UI_FX, // works
            //KOWALSKI_ANALYSIS,
            //MINIRARRIA,
            BRIGHT_NIGHT, // works
            SEVEN_YEARS_BAD_LUCK, // assume it works
            DISCRETIZED_MOVEMENT, // works
            FAKE_ENEMIES, // works
            CHAOS_MUSIC, // works
        }

        List<ChaosEffect> allEffects;
        // for packs, or selection featued vs all
        List<ChaosEffect> effectPool;
        // reduce update
        List<ChaosEffect> activeEffects;
        // allows for weighted selection using binary search
        List<int> runningWeightTotal;

        public ChaosManager()
        {
            allEffects = new List<ChaosEffect>();
            effectPool = new List<ChaosEffect>();
            activeEffects = new List<ChaosEffect>();
            runningWeightTotal = new List<int>();
            //v1
            allEffects.Add(new ChaosEffect("Change Hardmode", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Time Skip!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("No Gravity!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Music!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Sound!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random NPC!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("No Creativity!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Teleport!", 100).AddTag("legacy").AddTag("v1").AddTag("teleport"));
            allEffects.Add(new ChaosEffect("Life & Mana Swap!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Magic Mirror!", 100).AddTag("legacy").AddTag("v1").AddTag("teleport"));
            allEffects.Add(new ChaosEffect("Recolored NPCs!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Buff!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Debuff!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Pet!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Healing Hurts!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Chat Messages!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Monolith Effect!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Dangerous X-ray!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Block Converted!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random NPC FX", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Merry Christmas!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Random Event!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("No more Worming and Flying!", 100).AddTag("legacy").AddTag("v1"));
            allEffects.Add(new ChaosEffect("Smash Bros Like Physics!", 100).AddTag("legacy").AddTag("v1"));
            //v2
            allEffects.Add(new ChaosEffect("Immense Spawnrate!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Max Life or Mana!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Butter Fingers!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Infinite Bugs!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Who is that NPC?", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Spin to Win", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Ultimate Boss", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("World Blessings!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Talking Creatures!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Rod of Discord!", 100).AddTag("legacy").AddTag("v2").AddTag("teleport"));
            allEffects.Add(new ChaosEffect("Random Projectile FX!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Player Tornado", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("NPCs Grow & Shrink!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Jungle Grows!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Swap Position!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Explosive Deaths!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Random Enemy FX II!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Sonic Health!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Upside Down!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Random Craft!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Random Tile FX", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Recolored Tiles!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Accumulating Velocity!", 100).AddTag("legacy").AddTag("v2"));
            allEffects.Add(new ChaosEffect("Verify Human", 100).AddTag("legacy").AddTag("v2").AddTag("minigame"));
            allEffects.Add(new ChaosEffect("Non-binary Gender", 100).AddTag("legacy").AddTag("v2"));
            //v3
            allEffects.Add(new ChaosEffect("Randomize Life & Mana", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Cursed by Luck", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Weapons compensate... for something", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Items go HAM!!", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Hallo Halloween", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("The Numbers Mason", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Reinforcements!", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Inverted Enemies!", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Always wet... try swimming ;3", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Darkness ensues...", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Inverts Buffs and Debuffs", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Random Creditz", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Fake Pickup!", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Random Audio Pitches!", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("April Weather!", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Spooky Ghost :O", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Old School Camera!", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Player becomes a Solar System~", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Readable UI", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Weird Lighting?!", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Graveyard Shift! D:", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Random Pylon FX", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Random Projectile FX II", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Fall Sensitivity!", 100).AddTag("legacy").AddTag("v3"));
            allEffects.Add(new ChaosEffect("Random Tile FX II!", 100).AddTag("legacy").AddTag("v3"));
            //unleashed
            allEffects.Add(new ChaosEffect("Crazy Gravity", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Abomination", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Time Travel", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Pain Shifts Reality", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Huge World", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Random Health Bars", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Unmissable Cursor", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Nearsighted", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Enemies Stun!", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Up or die!", 100).AddTag("featured").AddTag("minigame").AddTag("minigame"));
            allEffects.Add(new ChaosEffect("Random Item Drop FX", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Invisible Inventory Items", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Random Item Icons", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Random NPC Sprites", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Shimmering", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Hot Wawa!", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Tooltip Mix Up", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Breakout", 100).AddTag("featured").AddTag("minigame"));
            allEffects.Add(new ChaosEffect("Random Tooltip Effects", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Flatrarria", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Random Buff UI FX", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Bright Night", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Seven Years Bad Luck", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Discretized Movement", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Fake Enemies", 100).AddTag("featured"));
            allEffects.Add(new ChaosEffect("Chaos Music!", 100).AddTag("featured"));

            //allEffects.Add(new ChaosEffect("Kowalski Analysis!", 100).AddTag("featured")); // TODO: change to future
            //allEffects.Add(new ChaosEffect("Minirarria", 100).AddTag("featured")); // TODO: cahnge to future
        }

        public void CreateEffectPool(ModConfigChaos modConfig)
        {
            if(modConfig.gameMode == ModConfigChaos.GameMode.CLASSIC)
            {
                if(modConfig.GameModeClassic.classicEffectSelection == ModConfigChaos.ConfigClassic.ClassicEffectSelection.FEATURED)
                {
                    effectPool = allEffects.Where(fx => fx.HasTag("featured")).ToList();
                }
                else
                {
                    effectPool = allEffects;
                }
            }
            else if(modConfig.gameMode == ModConfigChaos.GameMode.CHALLENGE)
            {
                if(modConfig.GameModeChallenge.challengeSelection == ModConfigChaos.ConfigChallenge.ChallengeSelection.ORIGINAL)
                {
                    effectPool = allEffects.Where(fx => fx.HasTag("v1")).ToList();
                }
                else if(modConfig.GameModeChallenge.challengeSelection == ModConfigChaos.ConfigChallenge.ChallengeSelection.TELEPORTATION)
                {
                    effectPool = allEffects.Where(fx => fx.HasTag("teleport")).ToList();
                }
                else if(modConfig.GameModeChallenge.challengeSelection == ModConfigChaos.ConfigChallenge.ChallengeSelection.MINIGAMES)
                {
                    effectPool = allEffects.Where(fx => fx.HasTag("minigame")).ToList();
                }
            }
            else
            {
                effectPool = allEffects;
            }
        }

        public void ApplyWeights(ModConfigChaos.ConfigCustom configCustom)
        {
            // update weights given config
            // v1 weights
            allEffects[0].weight = configCustom.ChangeHardmodeFxWeight;
            allEffects[1].weight = configCustom.SkipTimeFxWeight;
            allEffects[2].weight = configCustom.NoGravityFxWeight;
            allEffects[3].weight = configCustom.RandomMusicFxWeight;
            allEffects[4].weight = configCustom.RandomSoundFxWeight;
            allEffects[5].weight = configCustom.RandomNPCFxWeight;
            allEffects[6].weight = configCustom.NoCreativityFxWeight;
            allEffects[7].weight = configCustom.RandomTeleportFxWeight;
            allEffects[8].weight = configCustom.LifeManaSwapFxWeight;
            allEffects[9].weight = configCustom.MagicMirrorFxWeight;
            allEffects[10].weight = configCustom.RecoloredNPCsFxWeight;
            allEffects[11].weight = configCustom.RandomBuffFxWeight;
            allEffects[12].weight = configCustom.RandomDebuffFxWeight;
            allEffects[13].weight = configCustom.RandomPetFxWeight;
            allEffects[14].weight = configCustom.HealingHurtsFxWeight;
            allEffects[15].weight = configCustom.RandomChatMsg;
            allEffects[16].weight = configCustom.RandomMonolithFxFxWeight;
            allEffects[17].weight = configCustom.DangerousXRayFxWeight;
            allEffects[18].weight = configCustom.RandTileConvert;
            allEffects[19].weight = configCustom.RandNPCFxFxWeight;
            allEffects[20].weight = configCustom.MerryXMaxFxWeight;
            allEffects[21].weight = configCustom.RandomEventFxWeight;
            allEffects[22].weight = configCustom.NoFlyingAndWormingFxWeight;
            allEffects[23].weight = configCustom.SmashBrosFxWeight;
            //v2
            allEffects[24].weight = configCustom.ImmenseSpawnRateFxWeight;
            allEffects[25].weight = configCustom.MaxLifeManaFxWeight;
            allEffects[26].weight = configCustom.ButterFingersFxWeight;
            allEffects[27].weight = configCustom.InfiniteBugsFxWeight;
            allEffects[28].weight = configCustom.ShadowEnemiesFxWeight;
            allEffects[29].weight = configCustom.NPCSpin2WinFxWeight;
            allEffects[30].weight = configCustom.UltimateBossFxWeight;
            allEffects[31].weight = configCustom.WorldBlessingsFxWeight;
            allEffects[32].weight = configCustom.TalkingCreaturesFxWeight;
            allEffects[33].weight = configCustom.RodOfDiscordFxWeight;
            allEffects[34].weight = configCustom.RandProjFxFxWeight;
            allEffects[35].weight = configCustom.PlayerTornadoFxWeight;
            allEffects[36].weight = configCustom.NPCGrowShrinkFxWeight;
            allEffects[37].weight = configCustom.JungleGrowsFxWeight;
            allEffects[38].weight = configCustom.SwapPlacesFxWeight;
            allEffects[39].weight = configCustom.ExplosiveDeathFxWeight;
            allEffects[40].weight = configCustom.RandEnemeyFxIIFxWeight;
            allEffects[41].weight = configCustom.SonicHealthFxWeight;
            allEffects[42].weight = configCustom.UpsideDownFxWeight;
            allEffects[43].weight = configCustom.RandomCraftFxWeight;
            allEffects[44].weight = configCustom.RandomTileFxfxWeight;
            allEffects[45].weight = configCustom.RecoloredTilesFxWeight;
            allEffects[46].weight = configCustom.AccumulatingVelocityFxWeight;
            allEffects[47].weight = configCustom.VerifyHumanFxWeight;
            allEffects[48].weight = configCustom.NonBinaryGenderFxWeight;
            //v3
            allEffects[49].weight = configCustom.RandLifeManaFxWeight;
            allEffects[50].weight = configCustom.CursedByLuckFxWeight;
            allEffects[51].weight = configCustom.BigWeaponsFxWeight;
            allEffects[52].weight = configCustom.ItemsGoHamFxWeight;
            allEffects[53].weight = configCustom.HalloHalloweenFxWeight;
            allEffects[54].weight = configCustom.RandNumberFxFxWeight;
            allEffects[55].weight = configCustom.ReinforcementsFxWeight;
            allEffects[56].weight = configCustom.InvertedEnemiesFxWeight;
            allEffects[57].weight = configCustom.AlwaysWetFxWeight;
            allEffects[58].weight = configCustom.DarknessEnsuesFxWeight;
            allEffects[59].weight = configCustom.InverseDeBuffFxWeight;
            allEffects[60].weight = configCustom.RandomCreditzFxWeight;
            allEffects[61].weight = configCustom.FakePickupFxWeight;
            allEffects[62].weight = configCustom.RandomAudioPitchesFxWeight;
            allEffects[63].weight = configCustom.AprilWeatherFxWeight;
            allEffects[64].weight = configCustom.SpookyGhostFxWeight;
            allEffects[65].weight = configCustom.OldSchoolCamFxWeight;
            allEffects[66].weight = configCustom.PlayerSolarSystemFxWeight;
            allEffects[67].weight = configCustom.ReadableUIFxWeight;
            allEffects[68].weight = configCustom.WeirdLightingFxWeight;
            allEffects[69].weight = configCustom.GraveyardShiftFxWeight;
            allEffects[70].weight = configCustom.RandomPylonFxFxWeight;
            allEffects[71].weight = configCustom.RandProjFxIIFxWeight;
            allEffects[72].weight = configCustom.FallSensitivityFxWeight;
            allEffects[73].weight = configCustom.RandomTileFxIIFxWeight;
            // unleashed v1
            allEffects[74].weight = configCustom.CrazyGravityFxWeight;
            allEffects[75].weight = configCustom.AbominationFxWeight;
            allEffects[76].weight = configCustom.TimeTravelFxWeight;
            allEffects[77].weight = configCustom.PainShiftsRealityFxWeight;
            allEffects[78].weight = configCustom.HugeWorldFxWeight;
            allEffects[79].weight = configCustom.RandomHealthBarsFxWeight;
            allEffects[80].weight = configCustom.UnmissableCursorFxWeight;
            allEffects[81].weight = configCustom.NearsightedFxWeight;
            allEffects[82].weight = configCustom.EnemiesStunFxWeight;
            allEffects[83].weight = configCustom.UpOrDieFxWeight;
            allEffects[84].weight = configCustom.RandomItemDropFxWeight;
            allEffects[85].weight = configCustom.InvisibleInventoryItemsFxWeight;
            allEffects[86].weight = configCustom.RandomItemIconsFxWeight;
            allEffects[87].weight = configCustom.RandomNPCSpritesFxWeight;
            allEffects[88].weight = configCustom.ShimmeringFxWeight;
            allEffects[89].weight = configCustom.HotWaWaFxWeight;
            allEffects[90].weight = configCustom.TooltipMixUpFxWeight;
            allEffects[91].weight = configCustom.BreakoutFxWeight;
            allEffects[92].weight = configCustom.RandomTooltipFxWeight;
            allEffects[93].weight = configCustom.FlatrarriaFxWeight;
            // allEffect[20].weight = configCustom.KowalskiAnalysisFxWeight;
            allEffects[94].weight = configCustom.RandomBuffUIFxWeight;
            //allEffects[21].weight = configCustom.MinirarriaFxWeight;
            allEffects[95].weight = configCustom.BrightNightFxWeight;
            allEffects[96].weight = configCustom.SevenYearsBadLuckFxWeight;
            allEffects[97].weight = configCustom.DiscretizedMovementFxWeight;
            allEffects[98].weight = configCustom.FakeEnemiesFxWeight;
            allEffects[99].weight = configCustom.ChaosMusicFxWeight;
            // create running weight total
            runningWeightTotal.Clear();
            for(int i = 0; i < effectPool.Count(); i++)
            {
                if(i == 0)
                {
                    runningWeightTotal.Add(effectPool[i].weight);
                    continue;
                }
                runningWeightTotal.Add(runningWeightTotal.Last() + effectPool[i].weight);
            }
        }

        public void UpdateActiveEffects(double deltaTime)
        {
            // update and remove
            for(int i = 0; i < activeEffects.Count; i++)
            {
                ChaosEffect effect = activeEffects[i];
                effect.Update((float)deltaTime);
                if(!effect.IsEnabled())
                {
                    activeEffects.RemoveAt(i);
                    i--;
                }
            }
        }

        public void TriggerChaosEffect(float duration)
        {
            ChaosEffect effect = effectPool[nextEffectId];
            effect.Enable(duration);
            if(!activeEffects.Contains(effect))
            {
                activeEffects.Add(effect);
            }
        }

        public void GenerateNextChaosEffect(int gameMode)
        {
            switch(gameMode)
            {
                case (int)ModConfigChaos.GameMode.CUSTOM:
                    int roll = Main.rand.Next(runningWeightTotal.Last()) + 1;
                    nextEffectId = runningWeightTotal.BinarySearch(roll) - 1;
                    if(nextEffectId < 0)
                    {
                        nextEffectId = ~nextEffectId;
                    }
                    if (allEffects[nextEffectId].weight == 0)
                    {
                        nextEffectId--;
                    }
                    break;
                case (int)ModConfigChaos.GameMode.CHALLENGE:
                    nextEffectId = Main.rand.Next(0, effectPool.Count);
                    break;
                //case (int)ModConfigChaos.GameMode.SURVIVAL:
                case (int)ModConfigChaos.GameMode.CLASSIC:
                    nextEffectId = Main.rand.Next(0, effectPool.Count);
                    break;
            }
        }

        public string GetNextChaosEffectName()
        {
            return effectPool[nextEffectId].effectName;
        }

        public bool IsEffectActive(int index)
        {
            return allEffects[index].IsEnabled();
        }

        public float GetEffectDuration(int index)
        {
            return allEffects[index].GetDurationLeft();
        }

        public void FlagEffectAsInitalDone(int index)
        {
            allEffects[index].FlagInitialDone();
        }

        public void ResetEffectAsInitalDoneFlag(int index)
        {
            allEffects[index].ResetInitialDoneFlag();
        }


        public bool IsEffectInitialDone(int index)
        {
            return allEffects[index].IsInitialDone();
        }

        public void WriteMetaDataByte(int fxIdx, byte data, uint offset)
        {
            allEffects[fxIdx].StoreMetaDataByte(data, offset);
        }

        public byte ReadMetaDataByte(int fxIdx, uint offset)
        {
            return allEffects[fxIdx].RetrieveMetaDataByte(offset);
        }

        public byte[] ReadMetaDataBytes(int fxIdx, uint offset, uint bytes)
        {
            return allEffects[fxIdx].RetrieveMetaDataBytes(offset, bytes);
        }

        public void WriteMetaDataBytes(int fxIdx, byte[] data, uint offset)
        {
            allEffects[fxIdx].StoreMetaDataBytes(data, offset);
        }
    }
}
