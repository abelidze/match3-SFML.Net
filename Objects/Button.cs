using System;
using System.Reflection;
using System.Globalization;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using Match3.Misc;
using Match3.Effects;

namespace Match3.Objects
{
    public class Button : GameObject, IMouseListener, IHasEffect
    {
        #region Events

        public event Action<MouseButtonEventArgs> OnMouseDown;
        public event Action<MouseButtonEventArgs> OnMouseUp;
        public event Action OnClicked;

        #endregion

        #region Fields

        private Sprite drawable;

        #endregion

        #region Properties

        public float Width => drawable.Texture.Size.X;
        public float Height => drawable.Texture.Size.Y;
        public float X => drawable.Position.X;
        public float Y => drawable.Position.Y;
        public Vector2f Size => new Vector2f(Width, Height);
        public Vector2f Position => drawable.Position;
        public bool IsEffectActive => Effect != null;
        
        public bool IsHovered { get; private set; } = false;
        public Effect Effect { get; private set; }

        #endregion

        public Button(Sprite sprite, float x, float y)
        {
            drawable = sprite;
            drawable.Position = new Vector2f(x, y);
            OnDraw += ButtonDraw;
        }

        #region Callbacks

        public void ButtonDraw()
        {
            if (IsEffectActive) {
                GameManager.Window.Draw(drawable, Effect.States);
            } else {
                GameManager.Window.Draw(drawable);
            }
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            OnMouseDown?.Invoke(e);
            IsHovered = e.Button == Mouse.Button.Left && IsOverButton(e.X, e.Y);
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
            OnMouseUp?.Invoke(e);
            if (!IsHovered || e.Button != Mouse.Button.Left || !IsOverButton(e.X, e.Y)) {
                return;
            }
            OnClicked?.Invoke();
        }

        #endregion

        #region Utils
        
        public Effect ApplyEffect<T>(params object[] args) where T : Effect
        {
            // BindingFlags.Public | BindingFlags.Instance;
            var binding = BindingFlags.OptionalParamBinding | BindingFlags.CreateInstance;
            var obj = (T) Activator.CreateInstance(typeof(T), binding, null, args, CultureInfo.CurrentCulture);
            return ApplyEffect(obj);
        }

        public Effect ApplyEffect(Effect effect)
        {
            effect.Accept(this);
            //effect.States.Shader?.SetUniform("texture", drawable.Texture);
            Effect = effect;
            return effect;
        }

        private bool IsOverButton(int x, int y)
        {
            return x > X && x < X + Width && y > Y && y < Y + Height;
        }

        #endregion
    }
}
