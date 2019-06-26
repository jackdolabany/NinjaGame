using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace NinjaGame
{
    public class Enemy1 : Enemy
    {

        float shootTimer = 0;
        AnimationDisplay animations;

        public Enemy1(ContentManager content, int cellX, int cellY, Player player, Camera camera)
            : base(content, cellX, cellY, player, camera)
        {
            animations = new AnimationDisplay();
            this.DisplayComponent = animations;
            var idleTexture = content.Load<Texture2D>(@"Textures\EnemyIdle");
            var idle = new AnimationStrip(idleTexture, 32, "idle");

            idle.LoopAnimation = true;
            idle.FrameLength = 0.6f;
            animations.Add(idle);

            animations.Play("idle");
            
            isEnemyTileColliding = true;
            Attack = 1;
            Health = 3;
            IsAffectedByGravity = true;

            SetCenteredCollisionRectangle(16, 26);
        }

        public override void Kill()
        {
            EffectsManager.AddBigBloodEffect(this.WorldCenter);
            EffectsManager.EnemyPop(this.WorldCenter, 10, Color.Red, 50f);

            this.Enabled = false;
            base.Kill();
        }

        public override void PlayDeathSound()
        {
            SoundManager.PlaySound("bookClose");
        }

        public override void PlayTakeHitSound()
        {
            SoundManager.PlaySound("swordMetal7");
        }

        public override void Update(GameTime gameTime, float elapsed)
        {

            var prevOnGround = OnGround;

            if (Alive)
            {
                if (animations.currentAnimationName == "idle")
                {
                    this.velocity.X = 20;
                    if (flipped)
                    {
                        this.velocity.X *= -1;
                    }
                }
            }

            base.Update(gameTime, elapsed);

            if (Alive && animations.currentAnimationName == "idle")
            {
                if ((!flipped && onRightWall) || (flipped && onLeftWall))
                {
                    flipped = !flipped;
                }
            }

        }

    }
}
