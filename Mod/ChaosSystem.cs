using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
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

        public override void OnModLoad()
        {
            modConfig = ModContent.GetInstance<ModConfigChaos>();
            base.OnModLoad();
            manager = new ChaosManager();
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
            
            //Main.GraveyardVisualIntensity = -1f;
        }
    }
}
