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
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
        }
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
            return !NPC.AnyNPCs(ModContent.NPCType<Unistar>()) && !Main.IsItDay();
        }

        public override bool? UseItem(Player player)
        {
            if(player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);
                int type = ModContent.NPCType<Unistar>();
        
                Vector2 spawn = player.Center + new Vector2(0, -1200);

                if(Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(null, (int)spawn.X, (int)spawn.Y, type, Target:player.whoAmI);
                }
                else 
                {
                    ModPacket packet = ModContent.GetInstance<TerrariaTenebrous>().GetPacket();
                    packet.Write((byte)TerrariaTenebrous.ModMessageType.SummonUnistar);
                    packet.Write((int)player.position.X);
                    packet.Write((int)player.position.Y);
                    packet.Write(player.whoAmI);
                    packet.Send();
                }
            }

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
