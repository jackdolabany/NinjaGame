using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NinjaGame
{
    public class AnimationStrip
    {
        private float frameTimer = 0f;

        public int currentFrame;
        
        public int FrameWidth { get; set; }

        public int FrameHeight { get; set; }

        public Texture2D Texture { get; set; }

        public string Name { get; set; }

        public string NextAnimation { get; set; }

        public bool LoopAnimation { get; set; }

        public bool Oscillate { get; set; }

        public bool FinishedPlaying { get; private set; }

        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        public float FrameLength { get; set; }

        public bool Reverse { get; set; }

        public Rectangle FrameRectangle
        {
            get
            {

                int realCurrentFrame = currentFrame;
                if (Reverse)
                {
                    realCurrentFrame = FrameCount - 1 - currentFrame;
                }

                return new Rectangle(
                    realCurrentFrame * FrameWidth,
                    0,
                    FrameWidth,
                    FrameHeight);
            }
        }

        public AnimationStrip(Texture2D texture, int frameWidth, string name)
        {
            this.Texture = texture;
            this.FrameWidth = frameWidth;
            this.FrameHeight = texture.Height;
            this.Name = name;
            this.FrameLength = 0.05f;
        }

        public AnimationStrip Play(int currentFrame)
        {
            this.currentFrame = currentFrame;
            this.frameTimer = 0;
            FinishedPlaying = false;
            return this;
        }

        public AnimationStrip Play()
        {
            return Play(0);
        }

        public AnimationStrip FollowedBy(string animationName)
        {
            this.NextAnimation = animationName;
            return this;
        }

        public void Update(float elapsed)
        {
            frameTimer += elapsed;

            if (frameTimer >= FrameLength)
            {
                currentFrame++;
                if (currentFrame >= FrameCount)
                {
                    if (LoopAnimation)
                    {
                        currentFrame = 0;
                        if (Oscillate)
                        {
                            Reverse = !Reverse;
                        }
                    }
                    else
                    {
                        currentFrame = FrameCount - 1;
                        FinishedPlaying = true;
                    }
                }

                frameTimer = 0f;
            }
        }

        public object Clone()
        {
            AnimationStrip clone = new AnimationStrip(this.Texture, this.FrameWidth, this.Name);
            clone.currentFrame = this.currentFrame;
            clone.FrameHeight = this.FrameHeight;
            clone.FrameLength = this.FrameLength;
            clone.FrameWidth = this.FrameWidth;
            clone.LoopAnimation = this.LoopAnimation;
            clone.NextAnimation = this.NextAnimation;
            clone.Oscillate = this.Oscillate;
            clone.Reverse = this.Reverse;
            return clone;
        }
    }
}
