using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.IO;
using TerrariaTenebrous.Content.Tiles.Ores;

namespace TerrariaTenebrous.Content.World
{
    public class StarlightWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int shiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

            if (shiniesIndex != -1)
            {
                tasks.Insert(shiniesIndex + 1,
                    new PassLegacy("Starlight Ore", GenerateStarlightOre));
            }
        }

        void GenerateStarlightOre(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Stars falling to the planet!";

            int attempts = (int)(Main.maxTilesX * Main.maxTilesY * 0.0002);

            for (int i = 0; i < attempts; i++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);

                int y = WorldGen.genRand.Next(
                    (int)Main.worldSurface,
                    (int)Main.rockLayer
                );

                Tile tile = Framing.GetTileSafely(x, y);

                if (tile.HasTile && tile.TileType == Terraria.ID.TileID.Stone)
                {
                    WorldGen.TileRunner(x,y,
                        WorldGen.genRand.Next(4, 7),
                        WorldGen.genRand.Next(4, 8),
                        ModContent.TileType<StarlightOreTile>()
                    );
                }
            }
        }
    }
}