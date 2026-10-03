using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.Items.Weapons.Melee.Tangle_club
{
    public class Tangle_club_swing : ModProjectile
    {
        public override string Texture => "TerrariaTenebrous/Content/Items/Weapons/Melee/Tangle_club/Tangle_club";

        private int Combo => (int)Projectile.ai[0];
        private ref float Timer => ref Projectile.ai[1];
        public float MaxDuration => Combo == 2 ? 30f : 20f;

        public override void SetDefaults()
        {
            Projectile.damage = 20;
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.restrikeDelay = 60;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed)
            {
                Projectile.Kill();
                return;
            }

            Timer++;
            float rawProgress = MathHelper.Clamp(Timer / MaxDuration, 0f, 1f);
            float easedProgress = MathHelper.SmoothStep(0f, 1f, rawProgress);

            player.heldProj = Projectile.whoAmI;
            Projectile.timeLeft = 2;

            int dir = player.direction;
            Projectile.spriteDirection = dir;

            float baseAngle = dir == 1 ? 0f : MathHelper.Pi;
            float swingAngle = 0f;

            switch (Combo)
            {
                case 0: // 1. Overhead Downward Slash
                    swingAngle = MathHelper.Lerp(-MathHelper.PiOver2 * 1.3f, MathHelper.PiOver2 * 0.7f, easedProgress);
                    break;

                case 1: // 2. Underhand Uppercut Scoop
                    swingAngle = MathHelper.Lerp(MathHelper.PiOver2 * 0.9f, -MathHelper.PiOver2 * 1.2f, easedProgress);
                    break;

                case 2: // 3. 360 Full Spin into Ground Slam
                    swingAngle = MathHelper.Lerp(-MathHelper.TwoPi - MathHelper.PiOver4, MathHelper.PiOver2 * 0.85f, easedProgress);

                    if (rawProgress >= 0.82f && Projectile.localAI[0] == 0f)
                    {
                        TriggerGroundImpact(player);
                        Projectile.localAI[0] = 1f;
                    }
                    break;
            }

            float totalRotation = baseAngle + (swingAngle * dir);
            Projectile.rotation = totalRotation;

            Vector2 armSocket = player.RotatedRelativePoint(player.MountedCenter);
            Projectile.Center = armSocket + totalRotation.ToRotationVector2() * 28f;

            float armRotation = totalRotation - MathHelper.PiOver2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);

            if (Timer >= MaxDuration)
            {
                Projectile.Kill();
            }
        }

        private void TriggerGroundImpact(Player player)
        {
            Vector2 bladeTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * 40f;

            for (int i = 0; i < 20; i++)
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(4f, 2f);
                dustVel.Y = -MathF.Abs(dustVel.Y) - 1.5f;
                Dust dust = Dust.NewDustDirect(bladeTip - new Vector2(16, 16), 32, 32, DustID.Dirt, dustVel.X, dustVel.Y, 100, default, 1.3f);
                dust.noGravity = false;
            }
            SoundEngine.PlaySound(SoundID.Item70, bladeTip);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 start = player.RotatedRelativePoint(player.MountedCenter);
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * 60f;
            float collisionPoint = 0f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 24f, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Player player = Main.player[Projectile.owner];

            SpriteEffects effects;
            Vector2 origin;
            float drawRotation;

            if (player.direction == 1)
            {
                effects = SpriteEffects.None;
                origin = new Vector2(0f, texture.Height);
                drawRotation = Projectile.rotation + MathHelper.PiOver4;
            }
            else
            {
                effects = SpriteEffects.FlipHorizontally;
                origin = new Vector2(texture.Width, texture.Height);
                drawRotation = Projectile.rotation + MathHelper.Pi - MathHelper.PiOver4;
            }

            Vector2 drawPos = player.RotatedRelativePoint(player.MountedCenter) - Main.screenPosition;

            Main.EntitySpriteDraw(
                texture,
                drawPos,
                null,
                lightColor,
                drawRotation,
                origin,
                Projectile.scale,
                effects,
                0
            );

            return false;
        }
    }
}