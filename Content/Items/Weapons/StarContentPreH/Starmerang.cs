using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.Items.Weapons.StarContentPreH
{
    public class Starmerang : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 38;
            Item.damage = 38;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;

            Item.shoot = ModContent.ProjectileType<StarmerangProj>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            Projectile.NewProjectile(
                source,
                position + new Vector2(0, -24),
                velocity,
                type,
                damage,
                knockback,
                player.whoAmI
            );
            return false;
        }
    }

    public class StarmerangProj : ModProjectile
    {
        public override string Texture =>
            $"{nameof(TerrariaTenebrous)}/Content/Items/Weapons/StarContentPreH/Starmerang";

        private const float MaxDistance = 400f;
        private const float ReturnSpeed = 16f;
        private const float OutSpeed = 12f;

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            // Infinite hits, both directions
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.aiStyle = -1; // CUSTOM AI
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Kill if owner is dead or inactive
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation += 0.45f * Projectile.direction;

            // ai[0] == 0 → going out
            // ai[0] == 1 → returning
            if (Projectile.ai[0] == 0f)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * OutSpeed;

                float distance = Vector2.Distance(player.Top, Projectile.Center);
                if (distance >= MaxDistance)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Vector2 toPlayer = player.Top - Projectile.Center;
                float distance = toPlayer.Length();

                if (distance < 30f)
                {
                    Projectile.Kill();
                    return;
                }

                toPlayer.Normalize();
                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    toPlayer * ReturnSpeed,
                    0.25f
                );
            }

            // Glow effect while flying
            Lighting.AddLight(Projectile.Center, 1f, 0.9f, 0.4f); // warm yellow glow

            // Optional: subtle dust trail
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.GoldFlame,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    150,
                    default,
                    0.9f
                );
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[0] = 1f;
                Projectile.velocity = oldVelocity * -0.5f;

                // Spawn stars
                if (Main.myPlayer == Projectile.owner)
                {
                    int count = Main.rand.Next(2, 6);

                    for (int i = 0; i < count; i++)
                    {
                        Vector2 spawnPos = Projectile.Center + new Vector2(
                            Main.rand.NextFloat(-16f, 16f),
                            -600f
                        );

                        Vector2 velocity = new Vector2(
                            Main.rand.NextFloat(-2f, 2f),
                            Main.rand.NextFloat(12f, 16f)
                        );

                        Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            spawnPos,
                            velocity,
                            ModContent.ProjectileType<StarmerangStar>(),
                            9,
                            0f,
                            Projectile.owner
                        );
                    }
                }

                // ⭐ Add glow/dust + sound on tile hit
                HitEffect();

                return false;
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // ⭐ Add glow/dust + sound on NPC hit
            HitEffect();
        }

        // Central method for glow + sound + dust
        private void HitEffect()
        {
            // Sound
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            // Light
            Lighting.AddLight(Projectile.Center, 1f, 0.9f, 0.4f);

            // Dust burst
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.GoldFlame,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f),
                    150,
                    default,
                    1.2f
                );
                Main.dust[dust].noGravity = true;
            }
        }
    }
    public class StarmerangStar : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.Starfury;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            // Rotation
            Projectile.rotation += 0.3f * Projectile.direction;

            // Starfury-like gravity
            Projectile.velocity.Y += 0.3f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            // ⭐ Soft golden lighting
            Lighting.AddLight(
                Projectile.Center,
                0.9f,  // R
                0.85f, // G
                0.4f   // B
            );

            // ✨ Sparkle dust
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.GoldFlame,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    150,
                    default,
                    0.9f
                );

                Main.dust[dust].noGravity = true;
            }

            // 🔊 Falling whoosh
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = 20;
                SoundEngine.PlaySound(
                    SoundID.Item9 with { Volume = 0.35f, PitchVariance = 0.2f },
                    Projectile.Center
                );
            }
        }

        // Allow falling through platforms, collide with solid tiles
        public override bool TileCollideStyle(
            ref int width,
            ref int height,
            ref bool fallThrough,
            ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 💥 Impact sound
            SoundEngine.PlaySound(
                SoundID.Item10 with { Volume = 0.6f, PitchVariance = 0.1f },
                Projectile.Center
            );

            Projectile.Kill();
            return true;
        }
    }
}
