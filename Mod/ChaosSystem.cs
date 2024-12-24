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
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
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
        public int currMusic = -1;
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
            currMusic = -1;
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
                if (!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY))
                {
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 0, 0);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, backgroundColor.R, 1);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, backgroundColor.G, 2);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, backgroundColor.B, 3);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, tileColor.R, 4);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, tileColor.G, 5);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, tileColor.B, 6);
                    Main.NewText(tileColor);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY);
                }
                if(manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 0) != 0)
                {
                    backgroundColor.R = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 1);
                    backgroundColor.G = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 2);
                    backgroundColor.B = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 3);
                    tileColor.R = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 4);
                    tileColor.G = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 5);
                    tileColor.B = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 6);
                }
            }
            if (manager.IsEffectActive((int)ChaosManager.ChaosEffects.DARKNESS_ENSUES))
            {
                backgroundColor = new Color(0, 0, 0);
                tileColor = new Color(0, 0, 0);
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
                    currMusic = Main.rand.Next(MusicID.Count);
                    //Main.musicBox2 
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
                    currMusic = Main.rand.Next(MusicID.Count);
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
                    CustomSmashAltar();
                    CustomSmashAltar();
                    CustomSmashAltar();
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.WORLD_BLESSINGS);
                }
            }
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_CREDITZ))
            {
                if(!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_CREDITZ))
                {
                    Terraria.GameContent.Events.CreditsRollEvent.Reset();
                    //Terraria.GameContent.Events.CreditsRollEvent.SetRemainingTimeDirect((int)(60*manager.GetEffectDuration((int)ChaosManager.ChaosEffects.RANDOM_CREDITZ)));
                    Terraria.GameContent.Events.CreditsRollEvent.TryStartingCreditsRoll();
                    Terraria.GameContent.Events.CreditsRollEvent.SetRemainingTimeDirect((int)(60 * manager.GetEffectDuration((int)ChaosManager.ChaosEffects.RANDOM_CREDITZ)));
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
                        IEnumerable<Player> activePlayers = Main.player.Where(pl => pl.active);
                        Player selectedPlayer = Main.rand.Next(activePlayers.ToArray());
                        // restrict to close by NPCs to not teleport across the whole world
                        activeNPCs = Main.npc.Where(npc =>
                            npc.active && (npc.position - selectedPlayer.position).Length() < 1000f
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
                        chaosTile.tileTypeOverrides.Add((fx,i),tileOverride);
                    }                
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_TILE_FX_II);
                }
            }
            else
            {
                if (manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_TILE_FX_II))
                {
                    /* 
                     * if another code shares the same overrides, we need to store the owner
                     */
                    ChaosTile chaosTile = ModContent.GetInstance<ChaosTile>();
                    foreach (KeyValuePair<(int,int),TileOverride> tileOverride in chaosTile.tileTypeOverrides)
                    {
                        if (tileOverride.Key.Item1 != (int)OverrideType.COLOR)
                        {
                            tileOverride.Value.UndoOverrideType();
                        }
                    }
                    chaosTile.tileTypeOverrides.Clear();
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
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.VERIFY_HUMAN, Encoding.ASCII.GetBytes(randWord), 1);
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
                        Main.menuMode = MenuID.WorldSelect;
                        Main.gameMenu = true;
                        Netplay.Disconnect = true;
                    }
                    manager.ResetEffectAsInitalDoneFlag((int)ChaosManager.ChaosEffects.VERIFY_HUMAN);
                }
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            
            base.UpdateUI(gameTime);
        }

        public override void PostUpdateEverything()
        {
            double deltaTime = Main.gameTimeCache.TotalGameTime.TotalSeconds - lastUpdate;//Main.gameTimeCache.ElapsedGameTime.TotalSeconds;
            if (manager.IsEffectActive((int)ChaosManager.ChaosEffects.APRIL_WEATHER))
            {
                if (!manager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.APRIL_WEATHER))
                {
                    manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.APRIL_WEATHER, BitConverter.GetBytes(10f), 0);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.APRIL_WEATHER, 0, 4);
                    manager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.APRIL_WEATHER);
                }
                byte[] timeData = manager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.APRIL_WEATHER, 0, 4);
                float time = BitConverter.ToSingle(timeData, 0);
                time += (float)deltaTime;
                if (time > 3f)
                {
                    // update time
                    time -= 3f;
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
                    if (windStateGen > 0.5f)
                    {
                        state += 1;
                    }
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.APRIL_WEATHER, (byte)((Main.rand.NextFloat() > 0.5f) ? 1 : 0), 5);
                    manager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.APRIL_WEATHER, state, 4);
                }
                byte windState = manager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.APRIL_WEATHER, 4);
                if (windState != 0)
                {
                    Main.windSpeedTarget = (windState == 1 ? -1 : 1) * Main.rand.NextFloat() * 2.5f;
                }
                manager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.APRIL_WEATHER, BitConverter.GetBytes(time), 0);
            }
            base.PostUpdateEverything();
        }

        public override void OnWorldUnload()
        {

            base.OnWorldUnload();
        }

        public override void PreSaveAndQuit()
        {
            ChaosTile chaosTile = ModContent.GetInstance<ChaosTile>();
            foreach (KeyValuePair<(int, int, int), TileOverride> tileOverride in chaosTile.tileOverrides)
            {
                if (tileOverride.Key.Item1 == (int)OverrideType.COLOR)
                {
                    tileOverride.Value.UndoOverrideTile();
                }
            }
            foreach (KeyValuePair<(int, int), TileOverride> tileOverride in chaosTile.tileTypeOverrides)
            {
                tileOverride.Value.UndoOverrideType();
            }
            chaosTile.tileOverrides.Clear();
            chaosTile.tileTypeOverrides.Clear();
            base.PreSaveAndQuit();
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
            // remove all overrides when the code is no longer active
            if(!manager.IsEffectActive((int)ChaosManager.ChaosEffects.RECOLORED_TILES))
            {
                ChaosTile chaosTile = ModContent.GetInstance<ChaosTile>();
                List<(int,int,int)> keysToRemove = new List<(int,int,int)> ();
                foreach (KeyValuePair<(int,int,int),TileOverride> tileOverride in chaosTile.tileOverrides)
                {
                    if (tileOverride.Key.Item1 == (int)OverrideType.COLOR)
                    {
                        keysToRemove.Add(tileOverride.Key);
                        tileOverride.Value.UndoOverrideTile();
                    }
                }
                for (int i = 0; i < keysToRemove.Count; i++)
                {
                    chaosTile.tileOverrides.Remove(keysToRemove[i]);
                }
            }
            base.ResetNearbyTileEffects();
        }

        public void AltarOreSpawn(ref int oreCount, int ore, int altOre, string oreName, string altOreName, ref float num3)
        {
            int oreTile = WorldGen.genRand.Next(2) == 1 ? ore : altOre;
            int num5 = 14;
            if (oreTile == altOre)
            {
                num5 += 9;
                num3 *= 0.9f;
            }
            string tileName = oreTile == ore ? oreName : altOreName;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("Your world has been blessed with " + tileName + "!", Color.Green);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Your world has been blessed with " + tileName + "!"), Color.Green, -1);
            }
            oreCount = oreTile;
            if(oreCount == 0)
            {
                num3 *= 1.05f;
            }
        }

        public void CustomSmashAltar()
        {
            int oreCount = WorldGen.altarCount % 3;
            int repitition = WorldGen.altarCount / 3 + 1;
            float num3 = (float)(Main.maxTilesX / 4200);
            int num4 = 1 - oreCount;
            num3 = num3 * 310f - (float)(85 * oreCount);
            num3 *= 0.85f;
            num3 /= (float)repitition;
            if(Main.hardMode)
            {
                if(oreCount == 2)
                {
                    AltarOreSpawn(ref oreCount, TileID.Adamantite, TileID.Titanium, "Adamantite", "Titanium", ref num3);
                }
                if (oreCount == 1)
                {
                    AltarOreSpawn(ref oreCount, TileID.Mythril, TileID.Orichalcum, "Mythril", "Orichalcum", ref num3);
                }
                if(oreCount == 0)
                {
                    AltarOreSpawn(ref oreCount, TileID.Cobalt, TileID.Palladium, "Cobalt", "Palladium", ref num3);
                }
            }
            else
            {
                if (oreCount == 2)
                {
                    AltarOreSpawn(ref oreCount, TileID.Gold, TileID.Platinum, "Gold", "Platinum", ref num3);
                }
                if (oreCount == 1)
                {
                    AltarOreSpawn(ref oreCount, TileID.Silver, TileID.Tungsten, "Silver", "Tungsten", ref num3);
                }
                if (oreCount == 0)
                {
                    AltarOreSpawn(ref oreCount, TileID.Iron, TileID.Lead, "Iron", "Lead", ref num3);
                }
            }
            
            int num8 = 0;
            while ((float)num8 < num3)
            {
                int arg_31A_0 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                double num9 = Main.worldSurface;
                if (oreCount == TileID.Mythril || oreCount == TileID.Orichalcum || oreCount == TileID.Silver || oreCount == TileID.Tungsten)
                {
                    num9 = Main.rockLayer;
                }
                if (oreCount == TileID.Adamantite || oreCount == TileID.Titanium || oreCount == TileID.Gold || oreCount == TileID.Platinum)
                {
                    num9 = (Main.rockLayer + Main.rockLayer + (double)Main.maxTilesY) / 3.0;
                }
                int j2 = WorldGen.genRand.Next((int)num9, Main.maxTilesY - 150);
                WorldGen.OreRunner(arg_31A_0, j2, (double)WorldGen.genRand.Next(5, 9 + num4), WorldGen.genRand.Next(5, 9 + num4), (ushort)oreCount);
                num8++;
            }
            int num10 = WorldGen.genRand.Next(3);
            int num11 = 0;
            while (num10 != 2 && num11++ < 1000)
            {
                int num12 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int num13 = WorldGen.genRand.Next((int)Main.rockLayer + 50, Main.maxTilesY - 300);
                if (Main.tile[num12, num13].HasTile && Main.tile[num12, num13].TileType == 1)
                {
                    if (num10 == 0)
                    {
                        if (WorldGen.crimson)
                        {
                            Main.tile[num12, num13].TileType = TileID.Crimstone; 
                        }
                        else
                        {
                            Main.tile[num12, num13].TileType = TileID.Ebonstone;
                        }
                    }
                    else
                    {
                        Main.tile[num12, num13].TileType = TileID.Pearlstone; 
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, num12, num13, 1, TileChangeType.None);
                        break;
                    }
                    break;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int num14 = Main.rand.Next(2) + 1;
                int npcID = Main.hardMode ? NPCID.Wraith : NPCID.Ghost;
                for (int k = 0; k < num14; k++)
                {
                    
                    NPC.SpawnOnPlayer((int)Player.FindClosest(Vector2.Zero, 16, 16), npcID);
                }
            }
            WorldGen.altarCount++;
        }
    }
}
