using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.Projectiles.StarContentPreH;

public class NightskyStar : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 22;
		Projectile.height = 24;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 3;
		Projectile.timeLeft = 600;
		Projectile.tileCollide = false;
		Projectile.scale = 1f;
	}

	
	public override void AI()
	{
		Projectile.frameCounter++;
		if (Projectile.frameCounter > 80)
		{
			Projectile.scale -= 0.03f;
		}
		
        Lighting.AddLight(Projectile.Center, Color.LightYellow.ToVector3());
		
		Projectile.velocity *= 0.98f;
		if (Projectile.scale <= 0)
		{
			Projectile.Kill();
		}

		if (Projectile.velocity.Length() > 10)
		{
			Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 10;
		}

		for (int i = 0; i < Main.maxNPCs; i++)
		{
			if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != NPCID.TargetDummy && Main.npc[i].life > 0)
			{
				NPC npc = Main.npc[i];
				if (npc.Center.Distance(Projectile.Center) < 300)
				{
					Projectile.velocity += (npc.Center - Projectile.Center) / 50;
				}
				else
				{
					continue;
				}
			}
		}
		
		Projectile.rotation = Projectile.velocity.ToRotation();
	}
}