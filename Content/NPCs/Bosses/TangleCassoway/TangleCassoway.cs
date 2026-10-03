using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.Items.Consumables.BossMinionSummons;

namespace TerrariaTenebrous.Content.NPCs.Bosses.TangleCassoway
{
    public enum ActionState
    {
        Walk,
        Jump,
        Roar,
        LayEggs
    }

    [AutoloadBossHead]
    public class TangleCassoway : ModNPC
    {
        public ActionState State
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }
        public ref float Timer => ref NPC.ai[1];
        public ref float CurrentState => ref NPC.ai[2];

        public ref float EggTimer => ref NPC.localAI[0];

        public float acceleration = 0.25f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 48;

            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.TrailCacheLength[NPC.type] = 30;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
            
        public override void SetDefaults()
        {
            NPC.width = 128;
            NPC.height = 298;
            NPC.damage = 50;
            NPC.defense = 15;
            NPC.lifeMax = 5000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.boss = true;
            NPC.noGravity = false;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;

            CurrentState = (float)ActionState.Walk;

            EggTimer = 0f;

        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

// --- State Selector ---------------------------------------------------------------------------------------

            float distanceSq = Vector2.DistanceSquared(new Vector2(NPC.Center.X, NPC.Bottom.Y), new Vector2(player.Center.X, player.Bottom.Y));
            Timer++;

            if (distanceSq <= 30 * 30)
            {
                Timer = 0f;
                State = ActionState.LayEggs;
            }

            else if (distanceSq > 100 * 100 && State == (float)ActionState.Walk)
            {
                Timer = 0f;
                State = ActionState.Walk;
            }   
           

// --- State Behaviors --------------------------------------------------------------------------------------------
            if (State == ActionState.Walk)
            {
                WalkAI();
            } 

            if(State == ActionState.LayEggs)
            {
                LayEggsAI();
            }
        }

// --- Helper AI Methods ---------------------------------------------------------------------------------------------
        public void WalkAI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            NPC.velocity.X += acceleration * NPC.direction; 
        }

        public void LayEggsAI()
        {
            NPC.velocity.X= 0f;
            if (EggTimer < 56f)
            {
                EggTimer++;
            }
            if (EggTimer == 56f)
            {
                int eggChance = Main.rand.Next(1, 6);
                if (eggChance < 4) 
                {
                    Main.NewText("The Tangle Cassoway has laid an egg!", 255, 255, 0);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int eggIndex = NPC.NewNPC(
                            NPC.GetSource_FromAI(),
                            (int)NPC.Center.X,
                            (int)NPC.Bottom.Y - 20,
                            ModContent.NPCType<TangleCassoway_Egg>()
                        );

                        if (Main.netMode == NetmodeID.Server && eggIndex < Main.maxNPCs)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: eggIndex);
                        }
                    }
                }
                else if(eggChance >= 4)
                {
                    Main.NewText("Failed to lay an egg.", 255, 0, 0);
                }
                EggTimer = -42f;
            }
        }

// --- Hitbox Handling ---------------------------------------------------------------------------------------------

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {

            Rectangle CassowayHitBox = GetFrameBodyHitbox();

            if (!CassowayHitBox.Intersects(target.Hitbox))
            {
                    return false;
            }
            else {return true;}
        }

        private Rectangle GetFrameBodyHitbox()
        {
            int frameHeight = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];
            int currentFrame = NPC.frame.Y / frameHeight;

            int bodyYOffset = 0;
            int bodyHeight = (int)(NPC.height * 0.58f);

            if (State == ActionState.LayEggs) 
            {
                if(currentFrame == 35 && currentFrame < 37)
                {
                    bodyYOffset = 40;
                }
                if (currentFrame == 38 && currentFrame < 40) 
                {
                    bodyYOffset = 70;
                }
                if (currentFrame == 40 && currentFrame < 42)
                {
                    bodyYOffset = 120;
                }
                if (currentFrame == 42 && currentFrame < 44)
                {
                    bodyYOffset = 90;
                }
                if(currentFrame == 44 && currentFrame < 46)
                {
                    bodyYOffset = 60;
                }
                if(currentFrame == 46 && currentFrame < 48)
                {
                    bodyYOffset = 10;
                }
            }

            return new Rectangle(
                 (int)NPC.position.X,
                 (int)NPC.position.Y + bodyYOffset,
                 NPC.width,
                  bodyHeight
            );
        }

// --- Animation Handling ---------------------------------------------------------------------------------------------
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            switch (State)
            {
                case ActionState.Walk:
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;

                        if (NPC.frame.Y >= 7 * frameHeight)
                        {
                            NPC.frame.Y = 0;
                        }
                    }
                    break;

                case ActionState.Jump:
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;

                        if (NPC.frame.Y < 7 * frameHeight || NPC.frame.Y >= 19 * frameHeight)
                        {
                            NPC.frame.Y = 7 * frameHeight;
                        }
                    }
                    break;

                case ActionState.Roar:
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;

                        if (NPC.frame.Y < 19 * frameHeight || NPC.frame.Y >= 35 * frameHeight)
                        {
                            NPC.frame.Y = 19 * frameHeight;
                        }
                    }
                    break;
                case ActionState.LayEggs:
                    if (NPC.frameCounter >= 7)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y < 35 * frameHeight || NPC.frame.Y >= 48 * frameHeight)
                        {
                            NPC.frame.Y = 35 * frameHeight;
                        }
                        if(NPC.frame.Y == 47 * frameHeight)
                        {
                            State = ActionState.Walk;
                        }

                    }
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, (texture.Height / Main.npcFrameCount[NPC.type]) * 0.5f);

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 drawPos = NPC.oldPos[i] - screenPos + drawOrigin + new Vector2(0f, NPC.gfxOffY);

                float progress = (float)(NPC.oldPos.Length - i) / NPC.oldPos.Length;
                Color trailColor = NPC.GetAlpha(drawColor) * progress * 0.3f;

                float rotation = NPC.oldRot[i];

                SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(
                    texture,
                    drawPos,
                    NPC.frame,
                    trailColor,
                    rotation,
                    drawOrigin,
                    NPC.scale,
                    effects,
                    0f
                );
            }

            return true;
        }
    }
}