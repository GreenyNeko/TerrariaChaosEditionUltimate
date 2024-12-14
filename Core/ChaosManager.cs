using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;

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
            // v1 24/25
            CHANGE_HARDMODE,
            SKIP_TIME,
            NO_GRAVITY,
            RANDOM_MUSIC,
            RANDOM_SOUND,
            RANDOM_NPC,
            NO_CREATIVITY,
            RANDOM_TELEPORT,
            LIFE_MANA_SWAP,
            MAGIC_MIRROR,
            RECOLORED_NPCS,
            RANDOM_BUFF,
            RANDOM_DEBUFF,
            RANDOM_PET,
            HEALING_HURTS,
            RANDOM_CHAT_MSG,
            RANDOM_MONOLITH_FX,
            DANGEROUS_XRAY,
            RAND_TILE_CONVERT,
            RAND_NPC_FX,
            MERRY_XMAS,
            RANDOM_EVENT,
            NO_FLYING_AND_WORMING,
            SMASH_BROS,
            // v2 23+2/25
            IMMENSE_SPAWN_RATE,
            MAX_LIFE_MANA,
            BUTTER_FINGERS,
            INFINITE_BUGS,
            SHADOW_ENEMIES,
            NPC_SPIN_2_WIN,
            ULTIMATE_BOSS,
            WORLD_BLESSINGS,
            TALKING_CREATURES,
            ROD_OF_DISCORD,
            RAND_PROJ_FX,
            PLAYER_TORNADO,
            NPC_GROW_SHRINK,
            JUNGLE_GROWS,
            SWAP_PLACES,
            EXPLOSIVE_DEATH,
            RAND_ENEMY_FX_II,
            SONIC_HEALTH,
            UPSIDE_DOWN,
            RANDOM_CRAFT,
            RANDOM_TILE_FX,
            RECOLORED_TILES,
            ACCUMULATING_VELOCITY,
            VERIFY_HUMAN,
            NON_BINARY_GENDER,
            // v3 25/25
            RAND_LIFE_MANA,
            CURSED_BY_LUCK,
            BIG_WEAPONS,
            ITEMS_GO_HAM,
            HALLO_HALLOWEEN,
            RAND_NUMBER_FX,
            REINFORCEMENTS,
            INVERTED_ENEMIES,
            ALWAYS_WET,
            DARKNESS_ENSUES,
            INVERSE_DE_BUFF,
            RANDOM_CREDITZ,
            FAKE_PICKUP,
            RANDOM_AUDIO_PITCHES,
            APRIL_WEATHER,
            SPOOKY_GHOST,
            OLD_SCHOOL_CAM,
            PLAYER_SOLAR_SYSTEM,
            READABLE_UI,
            WEIRD_LIGHTING,
            GRAVEYARD_SHIFT,
            RANDOM_PYLON_FX,
            RAND_PROJ_FX_II,
            FALL_SENSITIVITY,
            RANDOM_TILE_FX_II,
            // unleashed 25
            CRAZY_GRAVITY,
            ABOMINATION,
            TIME_TRAVEL,
            PAIN_SHIFTS_REALITY,
            HUGE_WORLD,
            RANDOM_HEALTH_BARS,
            UNMISSABLE_CURSOR,
            NEARSIGHTED,
            ENEMIES_STUN,
            UP_OR_DIE,
            RAND_ITEM_DROP_FX,
            INVISIBLE_INVENTORY_ITEMS,
            RANDOM_ITEM_ICONS,
            RANDOM_NPC_SPRITES,
            SHIMMERING,
            HOT_WAWA,
            POPUP_MIX_UP,
            BREAKOUT,
            RANDOM_TOOLTIP_EFFECTS,
            FLATRARRIA,
            RAND_BUFF_UI_FX,
            //KOWALSKI_ANALYSIS,
            //MINIRARRIA,
            BRIGHT_NIGHT,
            SEVEN_YEARS_BAD_LUCK,
            DISCRETIZED_MOVEMENT,
            FAKE_ENEMIES,
            CHAOS_MUSIC,
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
            allEffects.Add(new ChaosEffect("Graveyard Shit! D:", 100).AddTag("legacy").AddTag("v3"));
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
            allEffects[11].weight = configCustom.RandomBuffUIFxWeight;
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
                    nextEffectId = runningWeightTotal.BinarySearch(roll);
                    if(nextEffectId < 0)
                    {
                        nextEffectId = ~nextEffectId;
                    }
                    break;
                case (int)ModConfigChaos.GameMode.CHALLENGE:
                case (int)ModConfigChaos.GameMode.SURVIVAL:
                case (int)ModConfigChaos.GameMode.CLASSIC:
                    nextEffectId = Main.rand.Next(0, allEffects.Count);
                    break;
            }
        }

        public string GetNextChaosEffectName()
        {
            return allEffects[nextEffectId].effectName;
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
