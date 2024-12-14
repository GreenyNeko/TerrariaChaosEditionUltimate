using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using TerrariaChaosEditionUnleashed.Utility;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosProjectile : GlobalProjectile
    {
        // TODO: put them into ProjectileFxData
        Vector2 prevPos = Vector2.Zero;
        Vector2 subPixel = Vector2.Zero;
        Vector2 vanillaVel = Vector2.Zero;
        Vector2 effectVel = Vector2.Zero;
        public override void SetDefaults(Projectile entity)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_PROJ_FX))
            {
                int flags = Main.rand.Next(256);
                entity.alpha = (flags & 1) == 1 ? Main.rand.Next(256) : entity.alpha;
                entity.friendly = (flags & 2) == 2 ? !entity.friendly : entity.friendly;
                entity.hostile = (flags & 4) == 4 ? !entity.hostile : entity.hostile;
                entity.ignoreWater = (flags & 8) == 8 ? !entity.ignoreWater : entity.ignoreWater;
                entity.knockBack = (flags & 16) == 16 ? Main.rand.NextFloat() * 10f : entity.knockBack;
                entity.light = (flags & 32) == 32 ? Main.rand.NextFloat() * 3f : entity.light;
                entity.penetrate = (flags & 64) == 64 ? 255 : entity.penetrate;
                entity.rotation = (flags & 128) == 128 ? Main.rand.NextFloat() * MathF.PI : entity.rotation;
                flags = Main.rand.Next(256);
                entity.scale = (flags & 1) == 1 ? Main.rand.NextFloat() * 5f : entity.scale;
                entity.shimmerWet = (flags & 2) == 2 ? !entity.shimmerWet : entity.shimmerWet;
                entity.shouldFallThrough = (flags & 4) == 4 ? !entity.shouldFallThrough : entity.shouldFallThrough;
                entity.soundDelay = (flags & 8) == 8 ? 10 : entity.soundDelay;
                entity.spriteDirection = (flags & 16) == 16 ? Main.rand.Next(4) : entity.spriteDirection;
                entity.tileCollide = (flags & 32) == 32 ? !entity.tileCollide : entity.tileCollide;
                //entity.type = (flags & 64) == 64 ? Main.rand.Next(Terraria.ID.ProjectileID.Count) : entity.type;
                entity.aiStyle = (flags & 128) == 128 ? Main.rand.Next(256) : entity.aiStyle;
            }
            base.SetDefaults(entity);
        }

        public override bool PreAI(Projectile projectile)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.PLAYER_TORNADO))
            {
                Player closestPlayer = ChaosUtilities.ClosestPlayer(projectile.position);
                Vector2 vec = closestPlayer.position - projectile.position;
                vec.Normalize();
                projectile.velocity = vec;
            }
            return base.PreAI(projectile);
        }

        public override void PostAI(Projectile projectile)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ACCUMULATING_VELOCITY))
            {
                Vector2 velDiff = projectile.velocity - projectile.oldVelocity;
                projectile.velocity += velDiff * 0.05f;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.DISCRETIZED_MOVEMENT))
            {
                // TODO: which of these vars do we want to store in the effect?
                // get vanilla vel
                vanillaVel = projectile.velocity - effectVel;
                // remove effect vel from previous subpixel boost
                projectile.velocity = vanillaVel;
                Vector2 playerPos = projectile.position;
                Vector2 roundedPos = new Vector2(MathF.Round(playerPos.X / 16f) * 16, MathF.Round(playerPos.Y / 16f) * 16);
                // fix to ground
                roundedPos.Y += 6;
                subPixel += projectile.position - roundedPos;
                projectile.position = roundedPos;
                Vector2 newVel = Vector2.Zero;
                if (MathF.Abs(subPixel.X) >= 16)
                {
                    newVel.X = MathF.Sign(subPixel.X) * 16;
                    subPixel.X = 0;
                }
                if (MathF.Abs(subPixel.Y) >= 16)
                {
                    newVel.Y = MathF.Sign(subPixel.Y) * 16;
                    subPixel.Y = 0;
                }
                effectVel = newVel;
                projectile.velocity = vanillaVel + effectVel;
                prevPos = roundedPos;
            }
            base.PostAI(projectile);
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_PROJ_FX_II))
            {
                int fx = Main.rand.Next(6);
                switch(fx)
                {
                    case 5:
                        projectile.velocity = Vector2.Zero;
                        break;
                    case 4:
                        projectile.rotation = Main.rand.NextFloat() * MathF.PI * 2;
                        break;
                    case 3:
                        if(projectile.owner == Main.myPlayer)
                        {
                            Main.player[projectile.owner].velocity -= projectile.velocity;
                        }
                        break;
                    case 2:
                        projectile.velocity *= 2;
                        break;
                    case 1:
                        projectile.velocity -= projectile.velocity;
                        break;
                    default:
                        break;
                }
            }
            base.OnSpawn(projectile, source);
        }
    }
}
