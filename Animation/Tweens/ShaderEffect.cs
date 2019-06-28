using System;
using SFML.System;
using SFML.Graphics;
using Match3.Misc;

namespace Match3.Animation
{
    public class ShaderEffect : ITween
    {
        #region Events

        public event Action<float> OnUpdated;
        public event Action OnPaused;
        public event Action OnResumed;
        public event Action OnFinished;
        public event Action OnDraw;

        #endregion
        
        #region Fields
        
        private RenderStates renderSettings;
        private Shape shape;
        private float time;
        private float duration;
        private bool reloaded;

        #endregion
        
        #region Properties

        public bool IsPlaying { get; private set; } = true;

        #endregion

        public ShaderEffect(string name, Vector2f pos, Vector2f size, float duration = 1f, bool reload = false)
        {
            time = 32.0f;
            reloaded = reload;
            this.duration = 32f + duration;
            shape = new RectangleShape(size) {
                Position = pos
            };
            renderSettings = new RenderStates() {
                Shader = reload ? new Shader(null, null, $"Assets/Shaders/{name}.frag") : ResourceManager.LoadShader(name),
                BlendMode = BlendMode.Add,
                Transform = Transform.Identity
            };
            renderSettings.Shader.SetUniform("resolution", new SFML.Graphics.Glsl.Vec2(size));
        }

        #region Callbacks

        public void Update(float deltaTime)
        {
            if (!IsPlaying) return;
            OnUpdated?.Invoke(deltaTime);

            time += deltaTime;
            renderSettings.Shader.SetUniform("time", time);

            if (OnFinished != null && time > duration) {
                OnFinished();
                IsPlaying = false;
                if (reloaded) {
                    renderSettings.Shader.Dispose();
                }
            }
        }

        public void Draw()
        {
            if (!IsPlaying) return;
            OnDraw?.Invoke();
            GameManager.Window.Draw(shape, renderSettings);
        }

        #endregion
        
        #region Utils

        public ITween Toggle()
        {
            return this;
        }

        public ITween Pause()
        {
            OnPaused?.Invoke();
            return this;
        }

        public ITween Resume()
        {
            OnResumed?.Invoke();
            return this;
        }

        #endregion
    }
}
