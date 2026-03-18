using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.Items.Materials.Starlight
{
    public class StarlightBar : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.material = true;
            Item.value = Item.sellPrice(silver: 22);
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<StarlightOre>(), 3)
                .AddIngredient(ItemID.FallenStar, 1)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}