using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.Projectiles;
using TerrariaTenebrous.Content.Items.Materials;

namespace TerrariaTenebrous.Content.Items.Weapons.Ranger
{
    public class DarkshadeBlaster : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 26;

            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 3, silver: 50);

            Item.useTime = 45;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = false;

            Item.damage = 96;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 7f;
            Item.noMelee = true;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<VoidCrystal>();
            Item.shootSpeed = 30f;
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
            const int projectileCount = 4;
            int projectileType = ModContent.ProjectileType<VoidCrystal>();

            for (int i = 0; i < projectileCount; i++)
            {
                Vector2 perturbedVelocity =
                    velocity.RotatedByRandom(MathHelper.ToRadians(15f)) *
                    (1f - Main.rand.NextFloat(0.2f));

                Projectile.NewProjectile(
                    source,
                    position,
                    perturbedVelocity,
                    projectileType,
                    damage + Main.rand.Next(-10, 20),
                    knockback,
                    player.whoAmI
                );
            }

            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-2f, -2f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidShard>(), 50)
                .AddIngredient(ItemID.OnyxBlaster)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
