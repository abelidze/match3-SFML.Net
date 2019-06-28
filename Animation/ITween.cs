using System;

namespace Match3.Animation
{
    public interface ITween
    {
        event Action<float> OnUpdated;
        event Action OnPaused;
        event Action OnResumed;
        event Action OnFinished;

        bool IsPlaying { get; }
        
        void Update(float deltaTime);

        ITween Toggle();
        ITween Pause();
        ITween Resume();
    }
}
