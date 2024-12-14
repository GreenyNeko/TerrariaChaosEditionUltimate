using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Server;
using Terraria.UI;
using Terraria.UI.Chat;
using TerrariaChaosEditionUnleashed.Utility;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosSystem : ModSystem
    {
        public ChaosManager manager;
        double timeSinceLastEffect = 0f;
        double lastUpdate;
        float effectCooldown, effectCooldownRandomOffset, effectDuration, effectDurationRandomOffset;
        ModConfigChaos.GameMode currGameMode;
        ModConfigChaos modConfig;

        float prevZoom = -1f;
        bool requireTooltipUpdate = true;

        public override void OnModLoad()
        {
            modConfig = ModContent.GetInstance<ModConfigChaos>();
            base.OnModLoad();
            manager = new ChaosManager();
            On_PopupText.Update += On_PopupText_Update;
            On_PopupText.ResetText += On_PopupText_ResetText;
            On_SoundPlayer.Play += On_SoundPlayer_Play;
        }

        private ReLogic.Utilities.SlotId On_SoundPlayer_Play(On_SoundPlayer.orig_Play orig, Terraria.Audio.SoundPlayer self, ref SoundStyle style, Vector2? position, SoundUpdateCallback updateCallback)
        {
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_AUDIO_PITCHES))
            {
                style.Pitch = Main.rand.NextFloat() * 2f - 1f;
            }
            return orig.Invoke(self, ref style, position, updateCallback);
        }

        private void On_PopupText_ResetText(On_PopupText.orig_ResetText orig, PopupText text)
        {
            requireTooltipUpdate = true;
            orig.Invoke(text);
        }

        private void On_PopupText_Update(On_PopupText.orig_Update orig, PopupText self, int whoAmI)
        {
            if (manager.IsEffectActive((int)ChaosManager.ChaosEffects.POPUP_MIX_UP))
            {
                if (requireTooltipUpdate)
                {
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.POPUP_MIX_UP, (byte)Main.rand.Next(256), 0);
                    requireTooltipUpdate = false;
                }
                //self.scale = 10f;
                int offset = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.POPUP_MIX_UP, 0);
                int id = (whoAmI + offset) % 5452;
                self.name = Lang.GetItemName(id).Value;
            }
            orig.Invoke(self, whoAmI);
        }

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            currGameMode = modConfig.gameMode;
            effectCooldown = 20f;
            effectCooldownRandomOffset = 7.5f;
            effectDuration = 30f;
            effectDurationRandomOffset = 10f;
            manager.CreateEffectPool(modConfig);
            switch (currGameMode)
            {
                case ModConfigChaos.GameMode.CUSTOM:
                    effectCooldown = modConfig.GameModeCustom.effectCooldown;
                    effectCooldownRandomOffset = modConfig.GameModeCustom.effectCooldownRandomOffset;
                    effectDuration = modConfig.GameModeCustom.effectLength;
                    effectDurationRandomOffset = modConfig.GameModeCustom.effectLengthRandomOffset;
                    break;
            }
            if (currGameMode == ModConfigChaos.GameMode.CUSTOM)
            {
                manager.ApplyWeights(modConfig.GameModeCustom);
            }
            lastUpdate = Main.gameTimeCache.TotalGameTime.TotalSeconds;
            manager.GenerateNextChaosEffect((int)modConfig.gameMode);
            timeSinceLastEffect = Math.Max(effectCooldown + Main.rand.NextFloat(new Terraria.Utilities.Terraria.Utilities.FloatRange(-effectCooldownRandomOffset, effectCooldownRandomOffset)), 0);
            if (modConfig.ShowNextEffectInChat)
            {
                Main.NewText("Effect '" + manager.GetNextChaosEffectName() + "' will happen in " + timeSinceLastEffect.ToString() + " seconds.");
            }
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (manager.IsEffectActive((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY))
            {
                byte counter = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 0);
                int blue = counter % 2;
                int green = (counter >> 1) % 2;
                int red = (counter >> 2) % 2;

                if (blue == 0 && green == 0 && red == 0)
                {
                    blue = green = red = 1;
                }
                backgroundColor = new Color((float)red, (float)green, (float)blue);
                tileColor = new Color((float)red, (float)green, (float)blue);
            }
            base.ModifySunLightColor(ref tileColor, ref backgroundColor);
        }

        public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
        {
            base.ModifyTimeRate(ref timeRate, ref tileUpdateRate, ref eventUpdateRate);
        }

        public override void ModifyLightingBrightness(ref float scale)
        {
            base.ModifyLightingBrightness(ref scale);
        }

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.FLATRARRIA))
            {
                // Flatrarria
                Transform.Zoom = new Vector2(3f, 0.5f);
            }
            /*if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.KOWALSKI_ANALYSIS))
            {
                // Kowalski Analysis
                Transform.Zoom *= 6f;
            }*/
            /*if (manager.IsEffectActive((int)ChaosManager.ChaosEffects.MINIRARRIA))
            {
                // Minirarria
                Transform.Zoom *= 0.5f;
            }*/
            base.ModifyTransformMatrix(ref Transform);
        }

        public override void PreUpdateWorld()
        {
            base.PreUpdateWorld();
            double deltaTime = Main.gameTimeCache.TotalGameTime.TotalSeconds - lastUpdate;//Main.gameTimeCache.ElapsedGameTime.TotalSeconds;
            timeSinceLastEffect -= deltaTime;
            manager.UpdateActiveEffects(deltaTime);
            if (timeSinceLastEffect <= 0f)
            {
                // reset cooldown
                timeSinceLastEffect = Math.Max(effectCooldown + Main.rand.NextFloat(new Terraria.Utilities.Terraria.Utilities.FloatRange(-effectCooldownRandomOffset, effectCooldownRandomOffset)), 0);
                if(modConfig.ShowEffectOverHead)
                {
                    CombatText.NewText(Main.player[Main.myPlayer].getRect(), Microsoft.Xna.Framework.Color.White, manager.GetNextChaosEffectName());
                }
                // enable effect for given duration
                manager.TriggerChaosEffect(effectDuration + Main.rand.NextFloat(new Terraria.Utilities.Terraria.Utilities.FloatRange(-effectDurationRandomOffset, effectDurationRandomOffset)));
                manager.GenerateNextChaosEffect((int)modConfig.gameMode);
                if (modConfig.ShowNextEffectInChat)
                {
                    Main.NewText("Effect '" + manager.GetNextChaosEffectName() + "' will happen in " + timeSinceLastEffect.ToString() + " seconds.");
                }
            }
            lastUpdate = Main.gameTimeCache.TotalGameTime.TotalSeconds;

            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.UNMISSABLE_CURSOR))
            {
                Main.cursorScale = 20;
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.NEARSIGHTED))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.NEARSIGHTED))
                {
                    // reset GameZoom and store
                    if (prevZoom > 0)
                    {
                        Main.GameZoomTarget = prevZoom;
                        prevZoom = -1;
                    }
                    prevZoom = Main.GameZoomTarget;
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.NEARSIGHTED);
                }
                Main.GameZoomTarget = 500f;
            }
            else
            {
                if(prevZoom > 0)
                {
                    Main.GameZoomTarget = prevZoom;
                    prevZoom = -1;
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.BRIGHT_NIGHT))
            {
                Main.lightning = 1;
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.CHANGE_HARDMODE))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.CHANGE_HARDMODE))
                {
                    Main.hardMode = !Main.hardMode;
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.CHANGE_HARDMODE);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.SKIP_TIME))
            {
                if (!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.CHANGE_HARDMODE))
                {
                    if (Main.IsItDay())
                    {
                        Main.fastForwardTimeToDusk = true;
                    }
                    else
                    {
                        Main.fastForwardTimeToDawn = true;
                    }
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.CHANGE_HARDMODE);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_MUSIC))
            {
                if (!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_MUSIC))
                {
                    Main.musicBox2 = Main.rand.Next(Main.maxMusic);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_MUSIC);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.CHAOS_MUSIC))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.CHAOS_MUSIC))
                {
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.CHAOS_MUSIC, BitConverter.GetBytes(10f), 0);
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.CHAOS_MUSIC, BitConverter.GetBytes(Main.rand.NextFloat() * 5), 4);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.CHAOS_MUSIC);
                }
                byte[] timeData = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.CHAOS_MUSIC, 0, 4);
                byte[] nextData = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.CHAOS_MUSIC, 0, 4);
                float time = BitConverter.ToSingle(timeData);
                float next = BitConverter.ToSingle(nextData);
                time += (float)deltaTime;
                if(time > next)
                {
                    Main.musicBox2 = Main.rand.Next(Main.maxMusic);
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.CHAOS_MUSIC, BitConverter.GetBytes(Main.rand.NextFloat() * 5), 4);
                }
                manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.CHAOS_MUSIC, BitConverter.GetBytes(time), 0);
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_SOUND))
            {
                if (!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_SOUND))
                {
                    SoundEngine.PlaySound(SoundID.SoundByIndex[(ushort)Main.rand.Next(SoundID.ItemSoundCount + SoundID.NPCDeathCount + SoundID.NPCHitCount)]);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_SOUND);
                }
            }
            if (manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_NPC))
            {
                if (!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_NPC))
                {
                    NPC.SpawnOnPlayer(Main.CurrentPlayer.whoAmI, Main.rand.Next(NPCID.Count));
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_NPC);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_CHAT_MSG))
            {
                if (!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_CHAT_MSG))
                {
                    var chatMsgs = new[]{ ("The Blood Moon is rising...", Color.Green), ("Impending doom approaches...", Color.Green),
                        ("A gobling army is approaching from the west!", Color.Purple), ("A goblin army is approaching from the east!", Color.Purple),
                        ("A horrible chill goes down your spine...", Color.Purple), ("Screams echo around you...", Color.Purple), ("Eater of Worlds has awoken!", Color.Purple),
                        ("This is going to be a terrible night...", Color.Purple), ("You feel vibrations from deep below...", Color.Purple),
                        ("You feel a quaking from deep underground...", Color.Purple), ("You feel the air getting colder around you...", Color.Purple),
                        ("The air is getting colder around you...", Color.Purple), ("Your mind goes numb...", Color.Purple), ("You are overwhelmed with pain...", Color.Purple),
                        ("Otherworldly voices linger around you...", Color.Purple), ("What a horrible night to have a curse.", Color.Purple), ("Party time's over!", Color.Pink),
                        ("A solar eclipse is happening!", Color.Green), ("Celestial creatures are invading!", Color.Purple)
                    };
                    var selection = Main.rand.Next(chatMsgs);
                    Main.NewText(selection.Item1, selection.Item2);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_CHAT_MSG);
                }
            }
            //Filters.Scene[]
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_MONOLITH_FX))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_MONOLITH_FX))
                {
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_MONOLITH_FX, (byte)Main.rand.Next(4), 0);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_MONOLITH_FX);
                }
                byte fx = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_MONOLITH_FX, 0);
                string[] monolith = new string[] { "MonolithSolar, MonolithVortex", "MonolithStardust", "MonolithNebula" };
                SkyManager.Instance[monolith[fx]].Activate(Main.CurrentPlayer.position);
                Filters.Scene[monolith[fx]].Activate(Main.CurrentPlayer.position);
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.HALLO_HALLOWEEN))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.HALLO_HALLOWEEN))
                {
                    Main.halloween = !Main.halloween;
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.HALLO_HALLOWEEN);
                }
            }
            if (manager.IsEffectActive((int)ChaosManager.ChaosEffects.HALLO_HALLOWEEN))
            {
                if (!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.HALLO_HALLOWEEN))
                {
                    Main.xMas = !Main.xMas;
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.HALLO_HALLOWEEN);
                }
            }
            if (manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_EVENT))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_EVENT))
                {
                    int selectedEvent = Main.rand.Next(6);
                    switch(selectedEvent)
                    {
                        case 5:
                            Terraria.GameContent.Events.LanternNight.LanternNightsOnCooldown = 0;
                            Terraria.GameContent.Events.LanternNight.CheckNight();
                            break;
                        case 4:
                            Terraria.GameContent.Events.BirthdayParty.PartyDaysOnCooldown = 0;
                            Terraria.GameContent.Events.BirthdayParty.CheckMorning();
                            break;
                        case 3:
                        case 2:
                            Main.eclipse = (selectedEvent == 3);
                            Main.bloodMoon = (selectedEvent == 2);
                            break;
                        case 1:
                            Main.StartSlimeRain();
                            break;
                        default:
                            Main.StartInvasion(Main.rand.Next(InvasionID.Count));
                            break;
                    }
                    
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_EVENT);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.WORLD_BLESSINGS))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.WORLD_BLESSINGS))
                {
                    bool temp = Main.hardMode;
                    Main.hardMode = true;
                    WorldGen.SmashAltar(0, 0);
                    WorldGen.SmashAltar(0, 0);
                    WorldGen.SmashAltar(0, 0);
                    Main.hardMode = temp;
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.WORLD_BLESSINGS);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.ULTIMATE_BOSS))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.ULTIMATE_BOSS))
                {
                    NPC.SpawnBoss((int)(Main.CurrentPlayer.position.X * 8), (int)(Main.CurrentPlayer.position.Y * 8), Main.rand.Next(NPCID.Count), Main.CurrentPlayer.whoAmI);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.ULTIMATE_BOSS);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_CREDITZ))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_CREDITZ))
                {
                    Terraria.GameContent.Events.CreditsRollEvent.SetRemainingTimeDirect(30);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_CREDITZ);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.SWAP_PLACES))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.SWAP_PLACES))
                {
                    int type = Main.rand.Next(2);
                    IEnumerable<NPC> activeNPCs;
                    bool alternative = false;
                    // NPC and Player
                    if(type == 1)
                    {
                        Player selectedPlayer = Main.rand.Next(Main.player);
                        // restrict to close by NPCs to not teleport across the whole world
                        activeNPCs = Main.npc.Where(npc =>
                            npc.active && (npc.position - selectedPlayer.position).Length() < 200f
                        );
                        alternative = activeNPCs.Count() <= 0;
                        if (activeNPCs.Count() > 0)
                        {
                            NPC npc = Main.rand.Next<NPC>(activeNPCs.ToArray());
                            Vector2 pos = selectedPlayer.position;
                            selectedPlayer.position = npc.position;
                            npc.position = selectedPlayer.position;
                        }
                    }
                    // NPC and NPC
                    if(type == 0 || alternative)
                    {
                        activeNPCs = Main.npc.Where(npc => npc.active);
                        NPC npcFirst = Main.rand.Next<NPC>(activeNPCs.ToArray());
                        NPC npcSecond = Main.rand.Next<NPC>(activeNPCs.ToArray());
                        Vector2 pos = npcFirst.position;
                        npcFirst.position = npcSecond.position;
                        npcSecond.position = pos;
                    }
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.SWAP_PLACES);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.APRIL_WEATHER))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.APRIL_WEATHER))
                {
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.APRIL_WEATHER, BitConverter.GetBytes(10f), 0);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.APRIL_WEATHER, 0, 4);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.APRIL_WEATHER);
                }
                byte[] timeData = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.APRIL_WEATHER, 0, 4);
                float time = BitConverter.ToSingle(timeData, 0);
                time += (float)deltaTime;
                if(time > 5f)
                {
                    // update time
                    time -= 5f;
                    // rain
                    float raining = MathF.Max(Main.rand.NextFloat() * 2 - 1f, 0f);
                    if (raining <= 0f)
                    {
                        Main.raining = false;
                    }
                    Main.maxRaining = raining;
                    // wind
                    float windStateGen = Main.rand.NextFloat();
                    byte state = 0;
                    // left wind
                    if (windStateGen > 0.75f)
                    {
                        state += 1;
                    }
                    if(windStateGen > 0.5f)
                    {
                        state += 1;
                    }
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.APRIL_WEATHER, state, 4);
                }
                byte windState = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.APRIL_WEATHER, 4);
                if(windState != 0)
                {
                    Main.windSpeedTarget = (windState == 1 ? -1 : 1) * Main.rand.NextFloat() * 2.5f;
                }
                manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.APRIL_WEATHER, BitConverter.GetBytes(time), 0);
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.READABLE_UI))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.READABLE_UI))
                {
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.READABLE_UI, BitConverter.GetBytes(Main.UIScale), 0);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.READABLE_UI);
                }
                Main.UIScale = 5f;
            }
            else
            {
                if(manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.READABLE_UI))
                {
                    byte[] uiScaleData = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.READABLE_UI, 0, 4);
                    Main.UIScale = BitConverter.ToSingle(uiScaleData);
                    manager.ResetEffectAsInitalDoneFlag((int)ChaosManager.ChaosEffects.READABLE_UI);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.WEIRD_LIGHTING))
            {
                Lighting.GlobalBrightness = -9999999;
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.GRAVEYARD_SHIFT))
            {
                Main.GraveyardVisualIntensity = 1f;
            }
            // ChaosTile hooks trigger once or for each tile, so we need something global
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_TILE_FX_II))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_TILE_FX_II))
                {
                    ChaosTile chaosTile = ModContent.GetInstance<ChaosTile>();
                    for(int i = 0; i < TileID.Count; i++)
                    {
                        TileOverride tileOverride = new TileOverride();
                        int fx = Main.rand.Next((int)OverrideType.MAX - 1) + 1;
                        tileOverride.ApplyOverrideType(fx, i, Main.rand.NextBool() ? (byte)1 : (byte)0);
                        chaosTile.tileOverrides.Add(tileOverride);
                    }                
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_TILE_FX_II);
                }
            }
            else
            {
                if (manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_TILE_FX_II))
                {
                    /* 
                     * if another code shares the same overrides, we store all effects after each other so we can store the start index and the count
                     * we can then use the start index and count to delete the applied effects
                     */
                    ChaosTile chaosTile = ModContent.GetInstance<ChaosTile>();
                    foreach (TileOverride tileOverride in chaosTile.tileOverrides)
                    {
                        if (tileOverride.overrideType != (int)OverrideType.COLOR)
                        {
                            tileOverride.UndoOverrideType();
                        }
                    }
                    manager.ResetEffectAsInitalDoneFlag((int)ChaosManager.ChaosEffects.RANDOM_TILE_FX_II);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.VERIFY_HUMAN))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.VERIFY_HUMAN))
                {
                   
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.VERIFY_HUMAN, 0, 0);
                    string randWord = "";
                    for(int i = 0; i < 6; i++)
                    {
                        randWord += ChaosUtilities.GetRandomAlphaNum();
                    }
                    Main.NewText("Please verify that you are human by sending the code \"" + randWord + "\" in chat.");
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.VERIFY_HUMAN, randWord.ToByteArray(), 1);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.VERIFY_HUMAN);
                }
                
            }
            else
            {
                if(manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.VERIFY_HUMAN))
                {
                    byte success = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.VERIFY_HUMAN, 0);
                    if(success == 0)
                    {
                        Main.GoToWorldSelect();
                    }
                    manager.ResetEffectAsInitalDoneFlag((int)ChaosManager.ChaosEffects.VERIFY_HUMAN);
                }
            }
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
        }

        public override void OnWorldUnload()
        {
            ChaosTile chaosTile = ModContent.GetInstance<ChaosTile>();
            foreach (TileOverride tileOverride in chaosTile.tileOverrides)
            {
                if (tileOverride.overrideType == (int)OverrideType.COLOR)
                {
                    tileOverride.UndoOverrideTile();
                }
                else
                {
                    tileOverride.UndoOverrideType();
                }
            }
            chaosTile.tileOverrides.RemoveAll(to => to.overrideType == (int)OverrideType.COLOR);
            base.OnWorldUnload();
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            base.PostDrawInterface(spriteBatch);
            if (manager.IsEffectActive((int)ChaosManager.ChaosEffects.BREAKOUT))
            {
                // const
                const int paddleWidth = 280;
                const int paddleY = 240;
                if (!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.BREAKOUT))
                {
                    int y = Main.ScreenSize.Y - paddleY;
                    int x = Main.rand.Next(Main.ScreenSize.X);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.BREAKOUT, 0, 28);
                    // ball pos
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(x), 0);
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(y), 4);
                    // ball dir
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(-1f), 8);
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(-1f), 12);
                    // paddle pos
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(x - paddleWidth / 2), 16);
                    // block pos
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(Main.rand.Next(Main.ScreenSize.X)), 20);
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(Main.rand.Next(Main.ScreenSize.Y / 2)), 24);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.BREAKOUT);
                }
                if(manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.BREAKOUT, 28) == 0)
                {
                    
                    // load data
                    byte[] dataBallPosX = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, 0, 4);
                    int ballPosX = BitConverter.ToInt32(dataBallPosX);
                    byte[] dataBallPosY = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, 4, 4);
                    int ballPosY = BitConverter.ToInt32(dataBallPosY);
                    byte[] dataBallDirX = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, 8, 4);
                    float ballDirX = BitConverter.ToSingle(dataBallDirX);
                    byte[] dataBallDirY = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, 12, 4);
                    float ballDirY = BitConverter.ToSingle(dataBallDirY);
                    byte[] dataPaddleX = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, 16, 4);
                    int paddleX = BitConverter.ToInt32(dataPaddleX);
                    byte[] dataBlockX = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, 20, 4);
                    byte[] dataBlockY = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, 24, 4);
                    int blockX = BitConverter.ToInt32(dataBlockX);
                    int blockY = BitConverter.ToInt32(dataBlockY);

                    // update / game logic
                    ballPosX += (int)(ballDirX * 5);
                    ballPosY += (int)(ballDirY * 5);
                    if (ballPosX > paddleX && ballPosX < (paddleX + paddleWidth)
                        && ballPosY < Main.ScreenSize.Y - (paddleY - 40) && ballPosY > Main.ScreenSize.Y - paddleY)
                    {
                        // hit paddle
                        // positive: right side, negative: left side
                        int xDiff = ballPosX - (paddleX + (paddleWidth / 2));
                        // < -100% ... 0% ... 100% >
                        float diffPercent = xDiff / (paddleWidth / 2f);
                        ballDirY = -ballDirY;
                        ballPosY = Main.ScreenSize.Y - paddleY;
                        // add skew based on ball pos
                        float skewAngle = diffPercent * MathF.PI / 4;
                        float tempDirX = ballDirX * MathF.Cos(skewAngle) - ballDirY * MathF.Sin(skewAngle);
                        ballDirY = ballDirX * MathF.Sin(skewAngle) + ballDirY * MathF.Cos(skewAngle);
                        ballDirX = tempDirX;
                        // prevent 0
                        if(ballDirY >= 0)
                        {
                            ballDirY = -0.5f;
                        }
                    }
                    if (ballPosX > blockX && ballPosX < blockX + 60
                        && ballPosY > blockY && ballPosY < blockY + 40)
                    {
                        manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.BREAKOUT, 1, 28);
                    }
                    if(ballPosX < 0 || ballPosX > Main.ScreenSize.X)
                    {
                        ballDirX = -ballDirX;
                    }
                    if(ballPosY < 0)
                    {
                        ballDirY = -ballDirY;
                    }
                    // ball lost
                    if (ballPosY > Main.ScreenSize.Y)
                    {
                        Main.CurrentPlayer.Hurt(PlayerDeathReason.ByOther(0), Main.CurrentPlayer.statLifeMax2, 0, false, false, 0, false, 9999f, 9999f, 0);
                        manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.BREAKOUT, 1, 28);
                    }

                    // update data
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(ballPosX) , 0);
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(ballPosY), 4);
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(ballDirX), 8);
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(ballDirY), 12);
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(paddleX), 16);

                    // draw
                    // ball
                    Main.instance.LoadProjectile(Terraria.ID.ProjectileID.Fireball);
                    spriteBatch.Draw(Terraria.GameContent.TextureAssets.Projectile[Terraria.ID.ProjectileID.Fireball].Value, new Rectangle(ballPosX - 20, ballPosY - 20, 40, 40), Color.White);
                    // paddle
                    Main.instance.LoadTiles(Terraria.ID.TileID.Grass);
                    spriteBatch.Draw(Terraria.GameContent.TextureAssets.Tile[Terraria.ID.TileID.Grass].Value, new Rectangle(paddleX, Main.ScreenSize.Y - paddleY, paddleWidth, 40), new Rectangle(17, 0, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);
                    // block
                    Main.instance.LoadTiles(Terraria.ID.TileID.LivingWood);
                    spriteBatch.Draw(Terraria.GameContent.TextureAssets.Tile[Terraria.ID.TileID.LivingWood].Value, new Rectangle(blockX, blockY, 60, 40), new Rectangle(17, 17, 16, 16), Color.Red);
                }
                
            }
        }

        public override void ResetNearbyTileEffects()
        {
            ChaosTile chaosTile = ModContent.GetInstance<ChaosTile>();
            foreach(TileOverride tileOverride in chaosTile.tileOverrides)
            {
                if(tileOverride.overrideType == (int)OverrideType.COLOR)
                {
                    tileOverride.UndoOverrideTile();
                }
            }
            chaosTile.tileOverrides.RemoveAll(to => to.overrideType == (int)OverrideType.COLOR);
            base.ResetNearbyTileEffects();
        }
    }
}
