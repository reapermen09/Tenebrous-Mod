using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.Tiles.Ores;

namespace TerrariaTenebrous.Content.Items.Materials.Starlight
{
    public class StarlightOre : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.material = true;
            Item.value = Item.sellPrice(silver: 6);
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<StarlightOreTile>();
        }
    }
}