using System;
using System.Collections.Generic;
using SFML.System;
using Match3.Animation;

namespace Match3.Objects
{
    public class Tile : GameObject
    {
        #region Fields

        private AnimatedSprite drawable;
        private int type;

        #endregion
        
        #region Properties

        public int Type
        {
            get => type;
            set
            {
                type = value;
                var pos = drawable?.Position ?? new Vector2f(0f, 0f);
                drawable = ResourceManager.LoadSprite(ResourceManager.Tiles[type]);
                drawable.Play("down").Stop();
                drawable.Position = pos;
            }
        }

        public float X => drawable.Position.X;
        public float Y => drawable.Position.Y;
        public Vector2f Position => drawable.Position;

        #endregion

        public Tile(int type, float x, float y)
        {
            Type = type; // init drawable
            drawable.Position = new Vector2f(x, y);
        }
        
        #region Callbacks

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            drawable.Update(deltaTime);
        }

        public override void Draw()
        {
            base.Draw();
            GameManager.Window.Draw(drawable);
        }

        #endregion
        
        #region Utils

        public void PlayRandom()
        {
            var keys = new List<string>(drawable.Animations.Keys);
            drawable.Play(keys[GameManager.Rand.Next(keys.Count)]);
        }

        public ITween MoveTo(Vector2f target, bool immediate = false)
        {
            if (immediate) {
                drawable.Position = target;
                return null;
            }

            var dx = Math.Sign(target.X - X);
            var dy = Math.Sign(target.Y - Y);
            drawable.Play(DirectionToString(dx, dy));
            var tween = new LinearMovement(drawable, target, drawable.TextureRect.Height * 2.5f);
            tween.OnFinished += () => {
                drawable.Play("down").Stop();
            };
            return tween;
        }

        public ITween Swap(Tile other, bool immediate = false)
        {
            if (immediate) {
                var pos = drawable.Position;
                drawable.Position = other.drawable.Position;
                other.drawable.Position = pos;
                return null;
            }

            var dx = Math.Sign(other.X - X);
            var dy = Math.Sign(other.Y - Y);
            drawable.Play(DirectionToString(dx, dy));
            other.drawable.Play(DirectionToString(-dx, -dy));
            var tween = new SwapMovement(drawable, other.drawable, drawable.TextureRect.Height * 2.5f);
            tween.OnFinished += () => {
                drawable.Play("down").Stop();
                other.drawable.Play("down").Stop();
            };
            return tween;
        }

        private string DirectionToString(int dx, int dy)
        {
            var dir = (dx + 1) + (dy + 1) * 3;
            switch (dir) {
                case 1: return "up";
                case 3: return "left";
                case 5: return "right";
                default: return "down";
            }
        }

        #endregion
    }
}
