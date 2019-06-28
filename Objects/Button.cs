using System;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using Match3.Misc;

namespace Match3.Objects
{
    public class Button : GameObject, ICanHandleMouse
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
        public bool IsHovered { get; private set; } = false;

        #endregion

        public Button(Sprite sprite, float x, float y)
        {
            drawable = sprite;
            drawable.Position = new Vector2f(x, y);
        }

        #region Callbacks

        public override void Draw()
        {
            base.Draw();
            GameManager.Window.Draw(drawable);
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

        private bool IsOverButton(int x, int y)
        {
            return x > X && x < X + Width && y > Y && y < Y + Height;
        }

        #endregion
    }
}
