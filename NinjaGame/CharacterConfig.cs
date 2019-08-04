using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaGame
{



    /// <summary>
    /// This class can be serialized to/from JSON to configure characters for use by non programmers
    /// </summary>
    public class CharacterConfig
    {
        //Load a file and read the CharacterConfig.
        public static CharacterConfig GetCharacterConfig(string path)
        {
            var json = System.IO.File.ReadAllText(path);
            var config = JsonConvert.DeserializeObject<CharacterConfig>(json);
            return config;
        }

        public static AnimationDisplay GetAnimationDisplay(CharacterConfig config, ContentManager content)
        {
            var ad = new AnimationDisplay();
            foreach (var animation in config.Animations)
            {
                var image = content.Load<Texture2D>(@"Textures\" + animation.TexturePath);
                var animationStrip = new AnimationStrip(image, animation.FrameWidth, animation.Name);
                animationStrip.LoopAnimation = animation.Loop;
                animationStrip.Oscillate = animation.Oscillate;
                animationStrip.FrameLength = 1f / animation.Fps;
                ad.Add(animationStrip);
            }
            return ad;
        }

        public float Speed { get; set; }
        public float JumpSpeed { get; set; }
        public IEnumerable<AnimationConfig> Animations { get; set; }
    }

    public class AnimationConfig
    {
        public string Name { get; set; }
        public string TexturePath { get; set; }

        /// <summary>
        /// The width of each animation frame in pixels.
        /// </summary>
        public int FrameWidth { get; set; }

        /// <summary>
        /// Frames per second.
        /// </summary>
        public int Fps { get; set; }
        public bool Loop { get; set; }
        public bool Oscillate { get; set; }
    }
}
