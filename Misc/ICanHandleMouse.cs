using System;
using SFML.Window;

namespace Match3.Misc
{
    public interface ICanHandleMouse
    {
         event Action<MouseButtonEventArgs> OnMouseDown;
         event Action<MouseButtonEventArgs> OnMouseUp;

         void MouseDown(MouseButtonEventArgs e);
         void MouseUp(MouseButtonEventArgs e);
    }
}
