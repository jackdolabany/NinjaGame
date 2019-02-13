﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NinjaGame
{
    public class AnimationDisplay : DisplayComponent
    {

        protected DrawObject drawObject;
        public Dictionary<string, AnimationStrip> animations = new Dictionary<string, AnimationStrip>();

        public AnimationDisplay()
            : base()
        {
            this.drawObject = new DrawObject();
        }

        public void Add(string key, AnimationStrip animation)
        {
            animations.Add(key, animation);
        }

        public void Add(AnimationStrip animation)
        {
            animations.Add(animation.Name, animation);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (drawObject.Texture != null)
            {
                spriteBatch.Draw(
                    drawObject.Texture,
                    new Vector2((int)drawObject.Position.X, (int)drawObject.Position.Y),
                    drawObject.SourceRectangle,
                    this.TintColor,
                    this.Rotation,
                    this.RotationAndDrawOrigin,
                    this.Scale,
                    drawObject.Effect,
                    this.DrawDepth);
            }
        }

        public override void Update(GameTime gameTime, float elapsed, Vector2 position, bool flipped)
        {
            base.Update(gameTime, elapsed, position, flipped);

            // Update the animation
            if (!animations.ContainsKey(currentAnimationName)) return;

            var anim = animations[currentAnimationName];
            if (anim.FinishedPlaying)
            {
                Play(anim.NextAnimation);
            }
            else
            {
                anim.Update(elapsed);
            }

            // Setup the DrawObject
            //drawObject.Position = position;

            SpriteEffects effect = SpriteEffects.None;
            if (flipped)
            {
                effect = SpriteEffects.FlipHorizontally;
            }
            drawObject.Effect = effect;

            if (!animations.ContainsKey(currentAnimationName)) return;

            drawObject.Texture = anim.Texture;
            drawObject.SourceRectangle = anim.FrameRectangle;

            var center = GetWorldCenter(ref position);
            var drawPosition = center - new Vector2(drawObject.SourceRectangle.Width / 2, drawObject.SourceRectangle.Height / 2) * Scale;
            drawObject.Position = RotateAroundOrigin(drawPosition, GetWorldCenter(ref position), Rotation);

        }

        public override Vector2 GetWorldCenter(ref Vector2 worldLocation)
        {
            var currentAnimation = this.animations[this.currentAnimationName];
            return new Vector2(
              worldLocation.X,
              worldLocation.Y - ((float)currentAnimation.FrameHeight / 2f));
        }

        public void Play(string name, int startFrame)
        {
            if (!(name == null) && animations.ContainsKey(name))
            {
                currentAnimationName = name;
                animations[name].Play(startFrame);
            }
        }

        public virtual void Play(string name)
        {
            Play(name, 0);
        }
    }
}