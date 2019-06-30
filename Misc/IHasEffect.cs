using System;
using SFML.System;
using Match3.Effects;

namespace Match3.Misc
{
    public interface IHasEffect
    {
        event Action<float> OnUpdate;
        event Action OnDraw;

        Effect Effect { get; }
        bool IsEffectActive { get; }
        bool IsHidden { get; }
        Vector2f Size { get; }
        Vector2f Position { get; }

        Effect ApplyEffect(Effect effect);
        Effect ApplyEffect<T>(params object[] args) where T : Effect;
    }
}
