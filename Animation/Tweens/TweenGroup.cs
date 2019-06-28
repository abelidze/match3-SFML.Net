using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Match3.Animation
{
    public class TweenGroup : ITween
    {
        #region Events

        public event Action<float> OnUpdated;
        public event Action OnPaused;
        public event Action OnResumed;
        public event Action OnFinished;

        #endregion
        
        #region Fields

        private List<ITween> tweens;
        private int processingTweens;

        #endregion
        
        #region Properties

        public int Count => tweens.Count;
        public bool IsPlaying { get; private set; } = true;

        #endregion

        public TweenGroup()
        {
            tweens = new List<ITween>();
            processingTweens = 0;
        }
        
        #region Callbacks

        public void Update(float deltaTime)
        {
            if (!IsPlaying) return;
            OnUpdated?.Invoke(deltaTime);

            if (processingTweens == 0) {
                IsPlaying = false;
                OnFinished?.Invoke();
            } else foreach (var tween in tweens) {
                tween.Update(deltaTime);
            }
        }

        #endregion
        
        #region Utils

        public void Add(ITween tween)
        {
            ++processingTweens;
            OnUpdated += tween.Update;
            OnResumed += () => tween.Resume();
            OnPaused += () => tween.Pause();
            tween.OnFinished += () => {
                OnUpdated -= tween.Update;
                OnResumed -= () => tween.Resume();
                OnPaused -= () => tween.Pause();
                --processingTweens;
            };
            tweens.Add(tween);
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