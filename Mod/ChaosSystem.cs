using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

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
            On_Player.WaterCollision += OnPlayerWaterCollision;
            On_PopupText.Update += On_PopupText_Update;
            On_PopupText.ResetText += On_PopupText_ResetText;
        }

        private void On_PopupText_ResetText(On_PopupText.orig_ResetText orig, PopupText text)
        {
            requireTooltipUpdate = true;
            orig.Invoke(text);
        }

        private void OnPlayerWaterCollision(On_Player.orig_WaterCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            // copy code from lava
            if(manager.IsEffectActive((int)ChaosManager.ChaosEffects.HOT_WAWA))
            {
                if (!self.lavaImmune)
                {
                    self.Hurt(PlayerDeathReason.ByOther(2), 80, 1, false, true, 2, false, 0, 0, 0);
                    self.AddBuff(Terraria.ID.BuffID.OnFire, Main.expertMode ? 14 * 30 : 7 * 30);
                }
            }
            orig.Invoke(self, fallThrough, ignorePlats);
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
                    spriteBatch.Draw(Terraria.GameContent.TextureAssets.Tile[Terraria.ID.TileID.Grass].Value, new Rectangle(paddleX, Main.ScreenSize.Y - paddleY, paddleWidth, 40), new Rectangle(0, 0, 16, 16), Color.White, MathF.PI/2f);
                    // block
                    Main.instance.LoadTiles(Terraria.ID.TileID.LivingWood);
                    spriteBatch.Draw(Terraria.GameContent.TextureAssets.Tile[Terraria.ID.TileID.LivingWood].Value, new Rectangle(blockX, blockY, 60, 40), new Rectangle(0, 0, 16, 16), Color.Red);
                }
                
            }
            //Main.caveParallax = 1;
            //Main.GraveyardVisualIntensity = -1f;
        }
    }
}
