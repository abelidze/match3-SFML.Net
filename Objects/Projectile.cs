using System;
using System.Collections.Generic;
using SFML.System;
using Match3.Misc;
using Match3.Rooms;
using Match3.Animation;
using Match3.Effects;

namespace Match3.Objects
{
    public class Projectile : GameObject, IHasRoomAccess
    {
        #region Fields

        private ShapeEffect explosion;
        private AnimatedSprite drawable;
        private ITween movement;
        private string type;

        #endregion
        
        #region Properties

        public string Type
        {
            get => type;
            set
            {
                type = value;
                var pos = drawable?.Position ?? new Vector2f(0f, 0f);
                drawable = ResourceManager.LoadSprite(type);
                drawable.Position = pos;
            }
        }

        public float Width => drawable.TextureRect.Width;
        public float Height => drawable.TextureRect.Height;
        public float X => drawable.Position.X;
        public float Y => drawable.Position.Y;
        public Vector2f Size => new Vector2f(Width, Height);
        public Vector2f Position => drawable.Position;

        #endregion

        public Projectile(int type, Vector2f from, Vector2f to) : this(ResourceManager.Tiles[type], from, to)
        {
            // ...
        }

        public Projectile(string type, Vector2f from, Vector2f to)
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
            void DestoryNode()
            {
                room.Remove(node);
                room.OnLeave -= DestoryNode;
            };
            room.OnLeave += DestoryNode;
            movement.OnFinished += () => {
                IsHidden = true;
                // Ugly hard-coded stuff
                explosion = new ShapeEffect("particle", Position - Size * 2, Size * 4, 1f, true) {
                    Offset = Size * 2.5f,
                    DurationOffset = GameManager.Rand.Next(10000)
                };
                OnUpdate += explosion.Update;
                OnDraw += explosion.Draw;
                explosion.OnFinished += () => {
                    OnDraw -= explosion.Draw;
                    OnUpdate -= explosion.Update;
                    DestoryNode();
                    explosion = null;
                };
                Destroy();
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
