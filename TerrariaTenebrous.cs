using Terraria.ModLoader;

namespace TerrariaTenebrous
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class TerrariaTenebrous : Mod
	{
		
	}

	public class TenebrousPlayer : ModPlayer
	{
    	public bool noManaCostForStarSet;
    	public override void ResetEffects() {
        	noManaCostForStarSet = false;
   		}
	}
}