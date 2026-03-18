using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.Projectiles.StarContentPreH;

public class UnistarProjectile : ModProjectile
{
    private float constantRotation;
    private int damage;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 120;
    }

    public override void SetDefaults()
    {
        Projectile.width = 22;
        Projectile.height = 24;
        Projectile.damage = 7;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.tileCollide = false;
        Projectile.scale = 1.6f;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        damage = (int)(Projectile.damage * Projectile.scale);
        Projectile.velocity *= 0.99f;

        if (Projectile.timeLeft < 60)
            Projectile.scale *= 0.96f;

        constantRotation += 0.25f;
        Projectile.rotation = constantRotation;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteBatch sb = Main.spriteBatch;
        Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
        Vector2 origin = tex.Size() / 2f;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        float pulse = 1f + (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 8f) * 0.08f;
        Color auraColor = new Color(255, 120, 200);
        Color mainColor = Color.Yellow;

        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp,
            DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 oldDraw = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
            float fade = 1f - i / (float)Projectile.oldPos.Length;
            sb.Draw(tex, oldDraw, null, auraColor * 0.25f * fade, constantRotation,
                origin, Projectile.scale * pulse * fade, SpriteEffects.None, 0f);
        }

        sb.Draw(tex, drawPos, null, auraColor * 0.35f, constantRotation,
            origin, Projectile.scale * pulse * 1.15f, SpriteEffects.None, 0f);

        sb.Draw(tex, drawPos, null, mainColor, constantRotation,
            origin, Projectile.scale * pulse, SpriteEffects.None, 0f);

        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
            DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        return false;
    }
}
