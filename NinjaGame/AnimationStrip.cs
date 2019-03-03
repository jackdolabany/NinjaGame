using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NinjaGame
{
    public class AnimationStrip
    {
        private Texture2D texture;
        private int frameWidth;
        private int frameHeight;

        private float frameTimer = 0f;
        private float frameDelay = 0.05f;
        public int currentFrame;
        private bool loopAnimation = true;
        private bool finishedPlaying = false;

        private string name;
        private string nextAnimation;

        public int FrameWidth
        {
            get { return frameWidth; }
            set { frameWidth = value; }
        }

        public int FrameHeight
        {
            get { return frameHeight; }
            set { frameHeight = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string NextAnimation
        {
            get { return nextAnimation; }
            set { nextAnimation = value; }
        }

        public bool LoopAnimation
        {
            get { return loopAnimation; }
            set { loopAnimation = value; }
        }

        public bool Oscillate { get; set; }

        public bool FinishedPlaying
        {
            get { return finishedPlaying; }
        }

        public int FrameCount
        {
            get { return texture.Width / frameWidth; }
        }

        public float FrameLength
        {
            get { return frameDelay; }
            set { frameDelay = value; }
        }

        private bool _reverse = false;
        public bool Reverse
        {
            get { return _reverse; }
            set { _reverse = value; }
        }

        public Rectangle FrameRectangle
        {
            get
            {

                int realCurrentFrame = currentFrame;
                if (_reverse)
                {
                    realCurrentFrame = FrameCount - 1 - currentFrame;
                }

                return new Rectangle(
                    realCurrentFrame * frameWidth,
                    0,
                    frameWidth,
                    frameHeight);
            }
        }

        public AnimationStrip(Texture2D texture, int frameWidth, string name)
        {
            this.texture = texture;
            this.frameWidth = frameWidth;
            this.frameHeight = texture.Height;
            this.name = name;
        }

        public void Play(int currentFrame)
        {
            this.currentFrame = currentFrame;
            this.frameTimer = 0;
            finishedPlaying = false;
        }

        public void Play()
        {
            Play(0);
        }

        public void Update(float elapsed)
        {
            frameTimer += elapsed;

            if (frameTimer >= frameDelay)
            {
                currentFrame++;
                if (currentFrame >= FrameCount)
                {
                    if (loopAnimation)
                    {
                        currentFrame = 0;
                        if (Oscillate)
                        {
                            _reverse = !_reverse;
                        }
                    }
                    else
                    {
                        currentFrame = FrameCount - 1;
                        finishedPlaying = true;
                    }
                }

                frameTimer = 0f;
            }
        }

        public object Clone()
        {
            AnimationStrip clone = new AnimationStrip(this.Texture, this.frameWidth, this.Name);
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
