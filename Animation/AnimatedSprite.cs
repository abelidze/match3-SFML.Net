using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Match3.Animation
{
    public class AnimatedSprite : Sprite
    {
        #region Fields

        private Spritesheet spritesheet;

        #endregion
        
        #region Properties

        public Dictionary<string, Anim> Animations { get; private set; }
        public string CurrentAnim { get; private set; }
        public bool IsAnimating { get; private set; } = false;
        public int CurrentFrame => Animations[CurrentAnim].CurrentFrameIndex;

        #endregion

        public AnimatedSprite(Spritesheet sheet)
            : this(sheet, new Dictionary<string, Anim>())
        {
            // ...
        }

        public AnimatedSprite(Spritesheet sheet, Dictionary<string, Anim> animations)
            : base(sheet.Texture, sheet.Frames[0])
        {
            spritesheet = sheet;
            Animations = animations;
        }
        
        #region Callbacks

        public void Update(float deltaTime)
        {
            if (IsAnimating && CurrentAnim != null && Animations[CurrentAnim].IsChanged(deltaTime)) {
                UpdateRect();
            }
        }

        #endregion

        #region Utils

        public AnimatedSprite Play(string name)
        {
            if (!Animations.ContainsKey(name)) {
                throw new InvalidOperationException($"Animation '{name}' is missing!");
            }
            IsAnimating = true;
            CurrentAnim = name;
            Animations[CurrentAnim].Reset();
            UpdateRect();
            return this;
        }

        public AnimatedSprite Play()
        {
            IsAnimating = true;
            Animations[CurrentAnim].Reset();
            UpdateRect();
            return this;
        }

        public AnimatedSprite Stop()
        {
            IsAnimating = false;
            Animations[CurrentAnim].Reset();
            UpdateRect();
            return this;
        }

        public Anim Add(string name, List<int> frames, float delay = 0f)
        {
            return Add(name, frames.ToArray(), delay);
        }

        public Anim Add(string name, int[] frames, float delay = 0f)
        {
            if (Animations.ContainsKey(name)) {
                throw new InvalidOperationException($"Animation '{name}' already exists!");
            }
            var anim = new Anim(frames, delay);
            Animations[name] = anim;
            return anim;
        }

        private void UpdateRect()
        {
            TextureRect = spritesheet.Frames[Math.Min(CurrentFrame, spritesheet.Frames.Count - 1)];
        }

        #endregion
    }
}
