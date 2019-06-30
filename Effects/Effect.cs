using System;
using SFML.Graphics;
using Match3.Misc;
using Match3.Animation;

namespace Match3.Effects
{
    public abstract class Effect : ITween
    {
        #region Events
    
        public event Action<IHasEffect> OnApply;
        public event Action<float> OnUpdated;
        public event Action OnPaused;
        public event Action OnResumed;
        public event Action OnFinished;
        public event Action OnRestart;
        public event Action OnDraw;

        #endregion

        #region Properties

        protected RenderStates states;
        public RenderStates States => states;
        public bool HasFinishedHandler => OnFinished != null;
        public bool IsPlaying { get; protected set; } = true;
        public bool IsRestartable { get; protected set; } = true;

        #endregion
        
        #region Callbacks

        public virtual void Update(float deltaTime)
        {
            if (!IsPlaying) return;
            OnUpdated?.Invoke(deltaTime);
        }

        public virtual void Draw()
        {
            if (!IsPlaying) return;
            OnDraw?.Invoke();
        }

        protected virtual void Finished()
        {
            IsPlaying = false;
            OnFinished?.Invoke();
        }

        #endregion
        
        #region Utils

        public virtual void Restart()
        {
            if (!IsRestartable) return; // TODO: throw exception
            IsPlaying = true;
            OnRestart?.Invoke();
        }

        public virtual void Accept(IHasEffect obj)
        {
            IsPlaying = true;
            obj.OnUpdate += Update;
            obj.OnDraw += Draw;
            OnFinished += () => {
                obj.OnUpdate -= Update;
                obj.OnDraw -= Draw;
            };
            OnApply?.Invoke(obj);
        }

        public ITween Toggle()
        {
            return IsPlaying ? Pause() : Resume();
        }

        public ITween Pause()
        {
            IsPlaying = false;
            OnPaused?.Invoke();
            return this;
        }

        public ITween Resume()
        {
            IsPlaying = true;
            OnResumed?.Invoke();
            return this;
        }

        #endregion
    }
}
