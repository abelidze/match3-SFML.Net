using System;
using SFML.System;
using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace Match3.Effects
{
    public class ShapeEffect : ShaderEffect
    {
        #region Fields

        protected Shape shape;
        protected Vector2f offset = Vector2f.Zero;

        #endregion
        
        #region Properties

        public Vector2f Offset
        {
            get => offset;
            set
            {
                offset = value;
                Position = shape.Position;
            }
        }

        public Vector2f Position
        {
            get => shape.Position;
            set
            {
                shape.Position = value;
                States.Shader?.SetUniform("position", new Vec2(
                    value.X + Offset.X,
                    Settings.Height - value.Y - Offset.Y
                ));
            }
        }

        #endregion

        public ShapeEffect(string name, Vector2f pos, Vector2f size, float duration = 1f, bool reload = false)
            : base(name, duration, reload)
        {
            shape = new RectangleShape(size);
            states.Shader?.SetUniform("size", new Vec2(size));
            Position = pos;
        }

        #region Callbacks

        public override void Draw()
        {
            base.Draw();
            if (!IsPlaying) return;
            GameManager.Window.Draw(shape, states);
        }

        #endregion
    }
}
