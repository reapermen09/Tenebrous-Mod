using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.Items.Weapons.StarContentPreH;
using TerrariaTenebrous.Content.Projectiles.StarContentPreH;

namespace TerrariaTenebrous.Content.NPCs.Bosses.Unistar
{
    [AutoloadBossHead]
    public class Unistar : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 30;

            base.SetStaticDefaults();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starmerang>(), 2, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NightskyString>(), 2, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StaffOfStellarSpawn>(), 2, 1, 1));

            base.ModifyNPCLoot(npcLoot);
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var bossDowned = ModContent.GetInstance<BossDownedStates>();
                bossDowned.isUnistarDefeated = true;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement("A star that descends upon the use of the Stellar Telescope."),
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 28;
            NPC.defense = 6;
            NPC.lifeMax = 1000;
            NPC.life = 1150;
            NPC.boss = true;
            NPC.value = 20000;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.npcSlots = 10f;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/UnistarsTheme");
        }

        public float speed = 9;
        public float accelleration = 0.0025f;
        public float preSlashDownTicker = 0;

        public enum AttackState
        {
            Intro,
            Chasing,
            Blast,
            PrepareDash,
            Dashing,
            Spread,
            Spin,
            PlayersDead
        }

        public AttackState currentAttackState = AttackState.Intro;
        public int side = 1;

        public override void AI()
        {
            if (Main.dayTime) currentAttackState = AttackState.PlayersDead;

            Lighting.AddLight(NPC.Center, Color.LightYellow.ToVector3());
            bool allPlayersDead = true;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    allPlayersDead = false;
                }
            }

            if (allPlayersDead) currentAttackState = AttackState.PlayersDead;

            if (NPC.frameCounter == 0)
            {
                NPC.TargetClosest();
                currentAttackState = AttackState.Intro;
            }

            Player player = Main.player[NPC.target];

            if (currentAttackState == AttackState.Intro)
            {
                NPC.rotation += 0.1f;
                NPC.velocity.Y += 0.033f;
                if (NPC.frameCounter > 244)
                {
                    NPC.frameCounter = 0;
                    currentAttackState = AttackState.Blast;
                    SoundEngine.PlaySound(SoundID.Pixie, NPC.Center);
                }
            }
            else if (currentAttackState == AttackState.PrepareDash)
            {
                NPC.velocity *= 0.9f;
                if (NPC.frameCounter < 30)
                {
                    if (NPC.frameCounter % 10 == 2)
                    {
                        PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 10f, 2f, 20, 1000f, FullName);
                        Main.instance.CameraModifiers.Add(modifier);
                        SoundEngine.PlaySound(SoundID.NPCHit5, NPC.Center);
                    }
                }
                else
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 6f, 20, 1000f, FullName);
                    Main.instance.CameraModifiers.Add(modifier);

                    for (int i = 0; i < 5; i++)
                    {
                        float rot = (float)i / 5 * (float)(Math.PI * 2);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, rot.ToRotationVector2() * 15, ModContent.ProjectileType<UnistarProjectile>(), NPC.damage / 2, 1f);
                    }

                    SoundEngine.PlaySound(SoundID.NPCHit5, NPC.Center);

                    NPC.velocity = Vector2.Normalize(player.Center + player.velocity * 50 - NPC.Center) * 15;

                    NPC.frameCounter = 0;
                    currentAttackState = AttackState.Dashing;
                }
            }
            else if (currentAttackState == AttackState.Dashing)
            {
                NPC.knockBackResist = 0;
                if (NPC.frameCounter > 100)
                {
                    NPC.frameCounter = 0;
                    NPC.knockBackResist = 1f;
                    currentAttackState = AttackState.Chasing;
                    side *= -1;
                }
            }
            else if (currentAttackState == AttackState.Blast)
            {
                NPC.velocity *= 0.9f;
                if (NPC.frameCounter < 50)
                {
                    if (NPC.frameCounter % 10 == 2)
                    {
                        PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 10f, 2f, 20, 1000f, FullName);
                        Main.instance.CameraModifiers.Add(modifier);
                        SoundEngine.PlaySound(SoundID.NPCHit5, NPC.Center);
                    }
                }
                else
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 6f, 20, 1000f, FullName);
                    Main.instance.CameraModifiers.Add(modifier);

                    for (int i = 0; i < 16; i++)
                    {
                        float rot = (float)i / 16 * (float)(Math.PI * 2);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, rot.ToRotationVector2() * 7, ModContent.ProjectileType<UnistarProjectile>(), NPC.damage / 2, 1f);
                    }

                    SoundEngine.PlaySound(SoundID.NPCHit5, NPC.Center);

                    NPC.frameCounter = 0;
                    currentAttackState = AttackState.Chasing;
                }
            }
            else if (currentAttackState == AttackState.Chasing)
            {
                NPC.velocity += (Vector2.Normalize(player.Center + new Vector2(side * 250 + (float)Math.Cos(NPC.frameCounter / 25) * 200, -200 + (float)Math.Sin(NPC.frameCounter / 10) * 200) - NPC.Center) * 15 - NPC.velocity) / 15;
                if (NPC.frameCounter > 150)
                {
                    NPC.frameCounter = 0;
                    if (Main.rand.NextBool(5))
                    {
                        currentAttackState = AttackState.Blast;
                    }
                    else if (Main.rand.NextBool(5))
                    {
                        currentAttackState = AttackState.PrepareDash;
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        currentAttackState = AttackState.Spin;
                    }
                    else
                    {
                        currentAttackState = AttackState.Spread;
                    }
                }
            }
            else if (currentAttackState == AttackState.Spread)
            {
                if (NPC.frameCounter > 30)
                {
                    if (NPC.frameCounter > 300)
                    {
                        NPC.frameCounter = 0;
                        currentAttackState = AttackState.PrepareDash;
                    }

                    if (NPC.frameCounter % 18 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit5, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), 8), ModContent.ProjectileType<UnistarProjectile>(), NPC.damage / 2, 1f);
                    }
                }

                Vector2 goTo = player.Center + new Vector2((float)Math.Cos((float)NPC.frameCounter / 25) * 500, -400 + (float)Math.Sin((float)NPC.frameCounter / 10) * 50);
                NPC.velocity += (Vector2.Normalize(goTo - NPC.Center) * 15 - NPC.velocity) / 15;
            }
            else if (currentAttackState == AttackState.Spin)
            {
                if (NPC.frameCounter > 30)
                {
                    if (NPC.frameCounter > 300)
                    {
                        NPC.frameCounter = 0;
                        currentAttackState = AttackState.Blast;
                    }

                    NPC.rotation = (player.Center - NPC.Center).ToRotation() + (float)Math.Cos(NPC.frameCounter / 8) / 3;

                    if (NPC.frameCounter % 18 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit5, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2() * Main.rand.Next(7, 12), ModContent.ProjectileType<UnistarProjectile>(), NPC.damage / 2, 1f);
                    }
                }

                NPC.velocity *= 0.98f;
            }
            else if (currentAttackState == AttackState.PlayersDead)
            {
                NPC.noTileCollide = true;
                NPC.velocity.Y -= 0.5f;
                NPC.velocity.X *= 0.98f;
                NPC.EncourageDespawn(1);
            }

            NPC.rotation += (NPC.velocity.X * 0.08f - NPC.rotation) / 15;
            NPC.frameCounter++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bodyTex = ModContent.Request<Texture2D>("TerrariaTenebrous/Content/NPCs/Bosses/Unistar/Unistar").Value;

            Vector2 origin = bodyTex.Size() / 2f;
            Vector2 drawPos = NPC.Center - screenPos;
            float time = Main.GlobalTimeWrappedHourly;
            float pulse = 1f + (float)Math.Sin(time * 5f) * 0.08f;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            int trailLength = NPC.oldPos.Length;
            for (int i = 0; i < trailLength; i++)
            {
                Vector2 oldDraw = NPC.oldPos[i] + NPC.Size / 2f - screenPos;
                float hue = ((time * 0.3f) + (i / (float)trailLength)) % 1f;
                Color trailColor = Main.hslToRgb(hue, 0.9f, 0.55f);
                float alpha = 0.55f * (1f - (float)i / trailLength);
                float scale = pulse * (1f + (1f - (float)i / trailLength) * 0.35f);
                spriteBatch.Draw(bodyTex, oldDraw, null, trailColor * alpha, NPC.rotation, origin, scale, SpriteEffects.None, 0f);
            }

            int auraLayers = 12;
            for (int i = 0; i < auraLayers; i++)
            {
                float progress = i / (float)auraLayers;
                float radius = 10f + progress * 32f;
                float angle = time * 1.2f + progress * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                float alpha = 0.22f * (1f - progress);
                Color auraColor = Color.Lerp(Color.Gold, Color.Cyan, progress) * alpha;
                float scale = pulse * (1.05f + progress * 0.45f);
                spriteBatch.Draw(bodyTex, drawPos + offset, null, auraColor, NPC.rotation, origin, scale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(bodyTex, drawPos, null, Color.Yellow * 0.55f, NPC.rotation, origin, pulse * 1.15f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(bodyTex, drawPos, null, drawColor, NPC.rotation, origin, 1f, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            NPC.velocity += Vector2.Normalize(projectile.velocity) * 5 * NPC.knockBackResist;
            base.OnHitByProjectile(projectile, hit, damageDone);
        }
    }
}
