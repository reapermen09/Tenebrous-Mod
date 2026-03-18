using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.Items.Weapons.StarContentPreH
{
    public class LilStarBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LilStarProjectile>()] > 0)
            {
                player.buffTime[buffIndex] = Int32.MaxValue;
            }
            else
            {
                player.DelBuff(buffIndex);
            }
        }
    }

    public class LilStarProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.damage = 9;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, 0.7f, 0.8f, 0.1f);
            if (!player.HasBuff(ModContent.BuffType<LilStarBuff>()))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            float maxDistance = 50f * 16f;
            if (Vector2.Distance(Projectile.Center, player.Center) > maxDistance)
            {
                Projectile.ai[0] = 0f;
                Vector2 returnDir = player.Center - Projectile.Center;
                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    returnDir.SafeNormalize(Vector2.Zero) * 16f,
                    0.2f
                );
                return;
            }

            int chosenIndex = -1;
            Vector2 target = Projectile.Center;
            int highestLife = 0;
            bool targetFound = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy())
                    continue;

                float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
                if (npcDist > 500f)
                    continue;

                int assigned = 0;
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile pr = Main.projectile[p];
                    if (!pr.active || pr.owner != Projectile.owner || pr.type != Projectile.type)
                        continue;

                    if ((int)pr.ai[0] - 1 == i)
                        assigned++;
                }

                if (assigned > 0)
                    continue;

                if (npc.life > highestLife)
                {
                    highestLife = npc.life;
                    target = npc.Center;
                    chosenIndex = i;
                    targetFound = true;
                }
            }

            if (!targetFound)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.CanBeChasedBy())
                        continue;

                    float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (npcDist > 500f)
                        continue;

                    if (npc.life > highestLife)
                    {
                        highestLife = npc.life;
                        target = npc.Center;
                        chosenIndex = i;
                        targetFound = true;
                    }
                }
            }

            Projectile.ai[0] = chosenIndex >= 0 ? chosenIndex + 1f : 0f;

            Vector2 moveDirection;

            if (targetFound)
            {
                moveDirection = Vector2.Normalize(target - Projectile.Center) * 10f;
            }
            else
            {
                List<Projectile> sameMinions = new();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile pr = Main.projectile[i];
                    if (pr.active && pr.owner == Projectile.owner && pr.type == Projectile.type)
                        sameMinions.Add(pr);
                }

                int index = sameMinions.IndexOf(Projectile);
                int count = sameMinions.Count;

                float radius = 48f;
                float angle = MathHelper.TwoPi * index / count + Main.GlobalTimeWrappedHourly * 1.8f;

                Vector2 idlePos = player.Center + new Vector2(0, 32) // go 2 blocks down pls
                    + new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * radius
                    + new Vector2(0f, -40f);

                moveDirection = (idlePos - Projectile.Center) / 10f;
            }

            Projectile.velocity = (Projectile.velocity * 20f + moveDirection) / 21f;
        }
    }


    public class StaffOfStellarSpawn : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 9;
            Item.mana = 10;
            Item.knockBack = 0;
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<LilStarProjectile>();
            Item.buffType = ModContent.BuffType<LilStarBuff>();
            Item.shootSpeed = 10f;
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.GetModPlayer<TenebrousPlayer>().noManaCostForStarSet)
                mult = 0f;
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return true;
        }
    }
}