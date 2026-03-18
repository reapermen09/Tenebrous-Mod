using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.Items.Consumables.BossSummons;
using TerrariaTenebrous.Content.NPCs.Bosses;

namespace TerrariaTenebrous.Content.NPCs
{
    public class GlobalNPCDrops : GlobalNPC
    {
        static BossDownedStates bossDowned;
        public override void Load()
        {
            bossDowned = ModContent.GetInstance<BossDownedStates>();
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Harpy)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StellarTelescope>(), 40, 1, 1));
            else if(!npc.boss && !bossDowned.isUnistarDefeated && npc.damage > 0 && npc.lifeMax >= 10)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StellarTelescope>(), 250, 1, 1));
        }
    }
}