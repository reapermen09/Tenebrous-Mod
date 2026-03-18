using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.Items.Materials.Starlight;

namespace TerrariaTenebrous.Content.Tiles.Ores
{
    public class StarlightOreTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 410;
            Main.tileShine[Type] = 975;
            Main.tileShine2[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            MineResist = 2.0f;
            MinPick = 40;
            
            RegisterItemDrop(ModContent.ItemType<StarlightOre>());
            AddMapEntry(new Color(200, 200, 255), CreateMapEntryName());

            HitSound = SoundID.Tink;
            DustType = DustID.GemDiamond;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.15f;
            g = 0.15f;
            b = 0.35f;
        }
    }
}