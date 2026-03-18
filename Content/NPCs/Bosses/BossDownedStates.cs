using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TerrariaTenebrous.Content.NPCs.Bosses
{
    public class BossDownedStates : ModSystem
    {
        public bool isUnistarDefeated = false;

        public override void OnWorldLoad()
        {
            isUnistarDefeated = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["isUnistarDefeated"] = isUnistarDefeated;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("isUnistarDefeated"))
            {
                isUnistarDefeated = tag.GetBool("isUnistarDefeated");
            }
        }
    }
}
