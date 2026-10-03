using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.Items.Weapons.Melee.Tangle_club;

namespace TerrariaTenebrous.Content.Items.Weapons.Melee.Tangle_club
{
    public class Tangle_club : ModItem
    {
        public int ComboIndex = 0;
        public int cooldown = 0;
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
            Item.height = 54;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.knockBack = 5f;
            Item.value = 3000;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Tangle_club_swing>();
        }
       

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(ComboIndex != 0)
            {
                Projectile.NewProjectile(source, player.MountedCenter, velocity, type, damage, knockback, player.whoAmI, ai0: ComboIndex);

                ComboIndex = (ComboIndex + 1) % 3;
                return false;
            }
            if (ComboIndex == 0)
            { 
                if(cooldown <= 0)
                {
                    Projectile.NewProjectile(source, player.MountedCenter, velocity, type, damage, knockback, player.whoAmI, ai0: ComboIndex);
                    ComboIndex = (ComboIndex + 1) % 3;
                    cooldown = 60;  
                }
                return false;
            }
            else return false;
        }
        public override void HoldItem(Player player)
        {
            if (ComboIndex == 0)
            {
                cooldown--;
            }
        }
    }
}
