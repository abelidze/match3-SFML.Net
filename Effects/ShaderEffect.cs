using System;
using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace Match3.Effects
{
    public class ShaderEffect : Effect
    {
        #region Fields

        protected float time;
        protected float duration;
        protected float durationOffset = 1f;

        #endregion

        #region Properties

        public float DurationOffset
        {
            get => durationOffset;
            set
            {
                time += value - durationOffset;
                durationOffset = value;
            }
        }

        #endregion

        public ShaderEffect(string name, float duration, bool reload = false, bool finalize = true)
        {
            this.duration = duration;
            time = durationOffset;
            IsRestartable = !reload || !finalize;
            states = new RenderStates() {
                Shader = reload ? new Shader(null, null, $"Assets/Shaders/{name}.frag") : ResourceManager.LoadShader(name),
                BlendMode = BlendMode.Add,
                Transform = Transform.Identity
            };
            states.Shader.SetUniform("resolution", new Vec2(Settings.Width, Settings.Height));

            OnUpdated += UpdateTime;
            OnRestart += () => time = durationOffset;
        }

        #region Callbacks

        private void UpdateTime(float deltaTime)
        {
            time += deltaTime;
            states.Shader?.SetUniform("time", time);

            if (HasFinishedHandler && time > duration + durationOffset) {
                Finished();
                if (!IsRestartable) {
                    states.Shader?.Dispose();
                    states.Shader = null;
                }
            }
        }

        #endregion
    }
}
