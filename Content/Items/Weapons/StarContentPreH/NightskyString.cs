using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.Items.Weapons.StarContentPreH
{
    public class NightskyString : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.PlatinumBow);
            Item.width = 26;
            Item.height = 46;
            Item.damage = 12;
            Item.shootSpeed = 25;
            Item.rare = ItemRarityID.Blue;
            Item.mana = 4;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.GetModPlayer<TenebrousPlayer>().noManaCostForStarSet)
                mult = 0f;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(SoundID.Item5);
                int pType = ProjectileID.JestersArrow;

                for (int i = 0; i < 2; i++)
                {
                    int calcSpread = Main.rand.Next(4, 8);
                    Vector2 spread =
                        velocity.RotatedByRandom(MathHelper.ToRadians(calcSpread));

                    Projectile.NewProjectile(
                        source,
                        position,
                        spread,
                        pType,
                        damage,
                        knockback,
                        player.whoAmI
                    );
                }
                return false;
            }

            Vector2 spawnPos = position + new Vector2(0, -500);
            SoundEngine.PlaySound(SoundID.Item4);

            for (int i = 0; i < 2; i++)
            {
                Vector2 starVelocity =
                    (Main.MouseWorld - spawnPos).ToRotation()
                    .ToRotationVector2() * Item.shootSpeed;

                starVelocity = starVelocity.RotatedByRandom(0.1f);

                Projectile.NewProjectile(
                    source,
                    spawnPos,
                    starVelocity,
                    ProjectileID.StarCannonStar,
                    damage,
                    knockback,
                    player.whoAmI
                );
            }
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.altFunctionUse == 2)
                return true;

            return base.CanConsumeAmmo(ammo, player);
        }
    }
}
