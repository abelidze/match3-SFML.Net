using SFML.System;
using SFML.Graphics;
using Match3.Misc;

namespace Match3.Effects
{
    public class FadeEffect : ShaderEffect
    {
        public enum Type
        {
            In,
            Out
        }

        #region Fields

        private Type type;
        private float opacity;

        #endregion

        #region Properties

        public Type Mode
        {
            get => type;
            set
            {
                type = value;
                switch (type) {
                    case Type.In:
                        opacity = 0f;
                        break;

                    case Type.Out:
                        opacity = 1f;
                        break;
                }
                states.Shader?.SetUniform("opacity", opacity);
            }
        }

        #endregion

        public FadeEffect(Type mode, float duration = 2f, bool reusable = false)
            : base("fade", duration, true, !reusable)
        {
            Mode = mode;
            states.BlendMode = BlendMode.Alpha;

            OnUpdated += UpdateOpacity;
            OnRestart += () => Mode = mode;
        }

        #region Callbacks

        private void UpdateOpacity(float deltaTime)
        {
            switch (type) {
                case Type.In:
                    opacity += deltaTime / duration;
                    break;

                case Type.Out:
                    opacity -= deltaTime / duration;
                    break;
            }
            states.Shader?.SetUniform("opacity", opacity);
        }

        #endregion
    }
}
