using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.NPCs.Bosses.Unistar;

namespace TerrariaTenebrous
{
	public class TerrariaTenebrous : Mod
	{
		public enum ModMessageType : byte
		{
			SummonUnistar,
		}
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModMessageType msgType = (ModMessageType)reader.ReadByte();
			switch (msgType)
			{
				case ModMessageType.SummonUnistar:
					if(Main.netMode != NetmodeID.Server) break;
					int UnistarPosX = reader.ReadInt32();
					int UnistarPosY = reader.ReadInt32();
					int UnistarTargetWhoAmI = reader.ReadInt32();
					NPC.NewNPC(null, UnistarPosX, UnistarPosY, ModContent.NPCType<Unistar>(), Target:UnistarTargetWhoAmI);
					break;
			}
        }
	}

	public class TenebrousPlayer : ModPlayer
	{
    	public bool noManaCostForStarSet;
    	public override void ResetEffects() {
        	noManaCostForStarSet = false;
   		}
	}
}