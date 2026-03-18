using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaTenebrous.Content.NPCs.Snow.FrostDigger
{
    internal class FrostDiggerHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<FrostDiggerBody>();
        public override int TailType => ModContent.NPCType<FrostDiggerTail>();

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.aiStyle = -1;
            NPC.lifeMax = 60;
            NPC.damage = 14;
            NPC.defense = 2;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void Init()
        {
            MinSegmentLength = 20;
            MaxSegmentLength = 32;
            MoveSpeed = 8.5f;
            Acceleration = 0.2f;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.15f, 0.62f, 1.1f);

            base.AI();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, hit.HitDirection, -1f, 0, default, 1.2f);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            bool surfaceSnow = spawnInfo.Player.ZoneSnow && spawnInfo.SpawnTileY < Main.rockLayer && !Main.dayTime;
            bool undergroundSnow = spawnInfo.Player.ZoneSnow && spawnInfo.SpawnTileY > Main.rockLayer;

            if (surfaceSnow || undergroundSnow)
                return 0.05f;

            return 0f;
        }

        public override void OnKill()
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Main.rand.Next(-3, 4), Main.rand.Next(-3, 4), 0, default, 1.5f);
            }
        }
    }

    internal class FrostDiggerBody : WormBody
    {
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.aiStyle = -1;
            NPC.lifeMax = 60;
            NPC.damage = 14;
            NPC.defense = 2;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void Init()
        {
            MoveSpeed = 8.5f;
            Acceleration = 0.2f;
        }

        internal override void BodyTailAI()
        {
            Lighting.AddLight(NPC.Center, 0.15f, 0.62f, 1.1f);
            base.BodyTailAI();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, hit.HitDirection, -1f, 0, default, 1.2f);
        }
    }

    internal class FrostDiggerTail : WormTail
    {
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.aiStyle = -1;
            NPC.lifeMax = 60;
            NPC.damage = 14;
            NPC.defense = 2;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void Init()
        {
            MoveSpeed = 8.5f;
            Acceleration = 0.2f;
        }

        internal override void BodyTailAI()
        {
            Lighting.AddLight(NPC.Center, 0.15f, 0.62f, 1.1f);
            base.BodyTailAI();
        }


        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, hit.HitDirection, -1f, 0, default, 1.2f);
        }
    }
}
