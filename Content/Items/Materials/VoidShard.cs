using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.Items.Materials
{
    public class VoidShard : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}