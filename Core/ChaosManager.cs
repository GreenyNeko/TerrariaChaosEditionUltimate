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
            allEffects[0].weight = configCustom.CrazyGravityFxWeight;
            allEffects[1].weight = configCustom.AbominationFxWeight;
            allEffects[2].weight = configCustom.TimeTravelFxWeight;
            allEffects[3].weight = configCustom.PainShiftsRealityFxWeight;
            allEffects[4].weight = configCustom.HugeWorldFxWeight;
            allEffects[5].weight = configCustom.RandomHealthBarsFxWeight;
            allEffects[6].weight = configCustom.UnmissableCursorFxWeight;
            allEffects[7].weight = configCustom.NearsightedFxWeight;
            allEffects[8].weight = configCustom.EnemiesStunFxWeight;
            allEffects[9].weight = configCustom.UpOrDieFxWeight;
            allEffects[10].weight = configCustom.RandomItemDropFxWeight;
            allEffects[11].weight = configCustom.InvisibleInventoryItemsFxWeight;
            allEffects[12].weight = configCustom.RandomItemIconsFxWeight;
            allEffects[13].weight = configCustom.RandomNPCSpritesFxWeight;
            allEffects[14].weight = configCustom.ShimmeringFxWeight;
            allEffects[15].weight = configCustom.HotWaWaFxWeight;
            allEffects[16].weight = configCustom.TooltipMixUpFxWeight;
            allEffects[17].weight = configCustom.BreakoutFxWeight;
            allEffects[18].weight = configCustom.RandomTooltipFxWeight;
            allEffects[19].weight = configCustom.FlatrarriaFxWeight;
            // allEffects[20].weight = configCustom.KowalskiAnalysisFxWeight;
            allEffects[20].weight = configCustom.RandomBuffUIFxWeight;
            //allEffects[21].weight = configCustom.MinirarriaFxWeight;
            allEffects[21].weight = configCustom.BrightNightFxWeight;
            allEffects[22].weight = configCustom.SevenYearsBadLuckFxWeight;
            allEffects[23].weight = configCustom.DiscretizedMovementFxWeight;
            allEffects[24].weight = configCustom.FakeEnemiesFxWeight;
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
