using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.NPCs.Bosses.Unistar;

namespace TerrariaTenebrous.Content.Items.Consumables.BossSummons
{
    class StellarTelescope : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.consumable = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 20;
            Item.maxStack = 20;
            Item.autoReuse = false;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanUseItem(Player player) 
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Unistar>()) && !Main.dayTime;
        }

        public override bool? UseItem(Player player)
        {
            int type = ModContent.NPCType<Unistar>();

            /*
              should just come up from the sky,
              i edited unistar to come down until it reachs
              just above where the player spawned it
            */
            Vector2 spawn = player.Center + new Vector2(0, -1200);

            NPC.SpawnBoss(
                (int)spawn.X,
                (int)spawn.Y,
                type,
                player.whoAmI
            );

            SoundEngine.PlaySound(SoundID.Roar);

            Main.NewText("A star begins to descend...", 150, 200, 255);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
				.AddIngredient(ItemID.GoldBar, 3)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddIngredient(ItemID.Glass)
				.AddTile(TileID.Anvils)
				.Register();

            CreateRecipe()
				.AddIngredient(ItemID.PlatinumBar, 3)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddIngredient(ItemID.Glass)
				.AddTile(TileID.Anvils)
				.Register();
        }
    }
}