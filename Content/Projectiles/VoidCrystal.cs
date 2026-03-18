using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.Projectiles
{
    public class VoidCrystal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 48;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.alpha = 20;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;

            Projectile.light = 0.6f;

            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.BlackBolt;
        }

        public override void AI()
        {
            Lighting.AddLight(
                Projectile.Center,
                0.55f,
                0.1f,
                0.9f
            );

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;

            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 origin = new(texture.Width * 0.5f, Projectile.height * 0.5f);

            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                Vector2 drawPos =
                    Projectile.oldPos[i]
                    + origin
                    - Main.screenPosition
                    + Vector2.UnitY * Projectile.gfxOffY;

                float fade = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;

                Main.EntitySpriteDraw(
                    texture,
                    drawPos,
                    null,
                    Projectile.GetAlpha(lightColor) * fade,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);

            const int shardCount = 4;

            for (int i = 0; i < shardCount; i++)
            {
                Vector2 velocity =
                    Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.5f;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<VoidShrapnel>(),
                    Projectile.damage / 2,
                    0f,
                    Projectile.owner
                );
            }
        }
    }
}
