using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static TerrariaChaosEditionUnleashed.ChaosManager;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosPlayer : ModPlayer
    {
        bool stunTooltipSeen = false;
        float stunThreshold = 0;
        float stunTime = 0;
        float initialStunTime = 0;
        int lastInput = -1;
        double lastUpdate = 0;
        bool stunned = false;

        public override void PreUpdate()
        {
            double deltaTime = Main.gameTimeCache.TotalGameTime.TotalSeconds - lastUpdate;
            base.PreUpdate();
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.SHIMMERING))
            {
                Player.ShimmerCollision(true, true, true);
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.CRAZY_GRAVITY))
            {
                Player.gravity = Main.rand.NextFloat() * 4f - 1.8f;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.TIME_TRAVEL))
            {
                Main.time += Player.velocity.X * 8f;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ENEMIES_STUN))
            {
                if(stunThreshold > 0)
                {
                    stunThreshold -= (float)deltaTime;
                }
                if(stunTime > 0)
                {
                    stunTime -= (float)deltaTime;
                }
                else
                {
                    if(stunned)
                    {
                        CombatText.NewText(Player.getRect(), Color.LimeGreen, "Stun Recovery!", false);
                        stunned = false;
                    }
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.UP_OR_DIE))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.UP_OR_DIE))
                {
                    Main.NewText("Press UP to not die!");
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.UP_OR_DIE);
                    chaosManager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.UP_OR_DIE, BitConverter.GetBytes(0f), 0);
                }
                byte done = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.UP_OR_DIE, 4);
                if(done == 0)
                {
                    byte[] timeData = chaosManager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.UP_OR_DIE, 0, 4);
                    float time = BitConverter.ToSingle(timeData, 0);
                    time += (float)deltaTime;
                    if (time > 3)
                    {
                        Player.Hurt(PlayerDeathReason.ByOther(0), Player.statLifeMax2, 0, false, true, -1, false, 9999f, 9999f, 0f);
                        chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.UP_OR_DIE, 1, 4);
                    }
                    chaosManager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.UP_OR_DIE, BitConverter.GetBytes(time), 0);
                }
            }
            lastUpdate = Main.gameTimeCache.TotalGameTime.TotalSeconds;
        }

        public override void SetControls()
        {
            base.SetControls();
            if(stunTime > 0)
            {
                Player.controlUp = false;
                Player.controlDown = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlJump = false;
                Player.controlUseItem = false;
                Player.controlMount = false;
                Player.controlUseTile = false;
                Player.controlThrow = false;
                Player.controlQuickMana = false;
                Player.controlHook = false;
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ENEMIES_STUN))
            {
                stunThreshold += MathF.Pow(hurtInfo.Knockback + 1f, 2);
                if (stunTime > 0)
                {
                    stunTime += hurtInfo.Knockback;
                    if (stunTime > initialStunTime)
                    {
                        initialStunTime = stunTime;
                    }
                }
                else if (stunThreshold > 100)
                {
                    CombatText.NewText(Player.getRect(), Color.OrangeRed, "STUNNED!", true);
                    if (!stunTooltipSeen)
                    {
                        stunTooltipSeen = true;
                        Main.NewText("Hint: Tap left and right repeatedly to free yourself from the stun!");
                    }
                    stunned = true;
                    hurtInfo.Knockback *= 5;
                    stunTime = 7.5f + stunThreshold - 100;
                    initialStunTime = stunTime;
                    stunThreshold -= 100;
                }
            }
            base.OnHitByNPC(npc, hurtInfo);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY))
            {
                byte value = (byte)Main.rand.Next(8);
                chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, value, 0);
            }
            base.OnHurt(info);
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ABOMINATION))
            {
                List<Microsoft.Xna.Framework.Vector2> offsets = [
                    drawInfo.backShoulderOffset,
                    drawInfo.frontShoulderOffset,
                    drawInfo.hairOffset,
                    drawInfo.headVect,
                    drawInfo.bodyVect,
                    drawInfo.legVect,
                    drawInfo.helmetOffset,
                    drawInfo.legsOffset,
                ];
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.ABOMINATION))
                {
                    List<int> indices = [0, 1, 2, 3, 4, 5, 6, 7];
                    List<int> randIndices = new List<int>();
                    for (int i = 0; i < 8; i++)
                    {
                        int randIdx = Main.rand.Next(indices);
                        randIndices.Add(randIdx);
                        indices.Remove(randIdx);
                    }
                    for(int i = 0; i < 8; i++)
                    {
                        chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, (byte)randIndices[i], (uint)i);
                    }
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.ABOMINATION);
                }
                drawInfo.backShoulderOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 0)];
                drawInfo.frontShoulderOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 1)];
                drawInfo.hairOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 2)];
                drawInfo.headVect = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 3)];
                drawInfo.bodyVect = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 4)];
                drawInfo.legVect = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 5)];
                drawInfo.helmetOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 6)];
                drawInfo.legsOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 7)];
            }
            if(stunTime > 0)
            {
                Main.instance.DrawHealthBar(Player.position.X, Player.position.Y + 20f, (int)stunTime, (int)initialStunTime, 1f);
            }

            base.ModifyDrawInfo(ref drawInfo);
        }

        public override void UpdateDead()
        {
            stunTime = 0;
            stunThreshold = 0;
            stunned = false;
            base.UpdateDead();
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            //triggersSet.
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ENEMIES_STUN))
            {
                if(stunTime > 0)
                {
                    if (triggersSet.Left && lastInput != 0)
                    {
                        lastInput = 0;
                        stunTime -= 1;
                    }
                    else if (triggersSet.Right && lastInput != 1)
                    {
                        lastInput = 1;
                        stunTime -= 1;
                    }
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.UP_OR_DIE))
            {
                byte done = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.UP_OR_DIE, 4);
                if (done == 0)
                {
                    if (triggersSet.Up)
                    {
                        chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.UP_OR_DIE, 1, 4);
                    }
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.BREAKOUT))
            {
                if (chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.BREAKOUT, 28) == 0)
                {
                    byte[] dataPaddleX = chaosManager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, 16, 4);
                    int paddleX = BitConverter.ToInt32(dataPaddleX);
                    if (triggersSet.Left)
                    {
                        if (paddleX - 1 > 0)
                        {
                            paddleX -= 8;
                        }
                    }
                    else if (triggersSet.Right)
                    {
                        if (paddleX + 1 < Main.ScreenSize.X - 280 - 140)
                        {
                            paddleX += 8;
                        }
                    }
                    chaosManager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(paddleX), 16);
                }
            }

            base.ProcessTriggers(triggersSet);
        }
    }
}
