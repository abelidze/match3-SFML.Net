using System;
using SFML.Graphics;

namespace Match3.Animation
{
    public class SwapMovement : ITween
    {
        #region Events

        public event Action<float> OnUpdated;
        public event Action OnPaused;
        public event Action OnResumed;
        public event Action OnFinished;

        #endregion
        
        #region Fields

        private ITween firstTween;
        private ITween secondTween;
        private int processingTweens;

        #endregion
        
        #region Properties

        public bool IsPlaying { get; private set; } = true;

        #endregion

        public SwapMovement(Transformable obj1, Transformable obj2, float speed)
        {
            processingTweens = 2;
            firstTween = new LinearMovement(obj1, obj2.Position, speed);
            secondTween = new LinearMovement(obj2, obj1.Position, speed);

            OnPaused += () => firstTween.Pause();
            OnPaused +=  () => secondTween.Pause();
            OnResumed += () => firstTween.Resume();
            OnResumed += () => secondTween.Resume();
            firstTween.OnFinished += () => --processingTweens;
            secondTween.OnFinished += () => --processingTweens;
        }

        #region Callbacks

        public void Update(float deltaTime)
        {
            if (!IsPlaying) return;
            OnUpdated?.Invoke(deltaTime);
            
            firstTween.Update(deltaTime);
            secondTween.Update(deltaTime);
            if (processingTweens <= 0) {
                IsPlaying = false;
                OnFinished?.Invoke();
            }
        }

        #endregion
        
        #region Utils

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
