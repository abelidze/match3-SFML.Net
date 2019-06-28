using System;
using System.Collections.Generic;
using SFML.System;
using Match3.Misc;
using Match3.Rooms;
using Match3.Animation;

namespace Match3.Objects
{
    public class Projectile : GameObject, IHasRoomAccess
    {
        #region Events

        public event Action OnDestroyed;

        #endregion

        #region Fields

        private ShaderEffect explosion;
        private AnimatedSprite drawable;
        private ITween movement;
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
                drawable.Position = pos;
            }
        }

        public float Width => drawable.TextureRect.Width;
        public float Height => drawable.TextureRect.Height;
        public float X => drawable.Position.X;
        public float Y => drawable.Position.Y;
        public Vector2f Position => drawable.Position;

        #endregion

        public Projectile(int type, Vector2f from, Vector2f to)
        {
            Type = type; // init drawable
            drawable.Position = from;

            //var speed = Helpers.Length(to - from) * 0.8f;
            var speed = Settings.Width * 0.25f;
            movement = new LinearMovement(drawable, to, speed);

            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
            if (Math.Abs(dx) > Math.Abs(dy)) {
                dy = 0f;
            } else {
                dx = 0f;
            }
            drawable.Play( DirectionToString(Math.Sign(dx), Math.Sign(dy)) );
        }

        #region Callbacks

        public void Created(Room room, LinkedListNode<GameObject> node)
        {
            room.OnLeave += () => room.Remove(node);
            movement.OnFinished += () => {
                IsHidden = true;
                // Ugly hard-coded stuff
                explosion = new ShaderEffect("particle", new Vector2f(X - 48f, Y - 48f), new Vector2f(200f, 200f), 1f, true);
                OnUpdate += explosion.Update;
                OnDraw += explosion.Draw;
                explosion.OnFinished += () => room.Remove(node);
                OnDestroyed?.Invoke();
            };
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            drawable.Update(deltaTime);
            movement.Update(deltaTime);
        }

        public override void Draw()
        {
            base.Draw();
            if (IsHidden) return;

            GameManager.Window.Draw(drawable);
        }

        #endregion

        #region Utils

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
