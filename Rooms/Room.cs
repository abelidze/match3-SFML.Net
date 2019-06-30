using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Window;
using Match3.Misc;
using Match3.Objects;

namespace Match3.Rooms
{
    public abstract class Room
    {
        #region Events

        public event Action OnInit;
        public event Action OnEnter;
        public event Action OnLeave;
        public event Action<float> OnUpdate;
        public event Action OnDraw;

        #endregion
        
        #region Fields

        protected readonly LinkedList<GameObject> objects = new LinkedList<GameObject>();

        #endregion

        public Room()
        {
            Init();
        }

        #region Callbacks

        protected virtual void Init()
        {
            OnInit?.Invoke();
        }

        public virtual void Update(float deltaTime)
        {
            foreach (var obj in objects.ToArray()) {
                obj.Update(deltaTime);
            }
            OnUpdate?.Invoke(deltaTime);
        }

        public virtual void Draw()
        {
            foreach (var obj in objects.ToArray()) {
                obj.Draw();
            }
            OnDraw?.Invoke();
        }

        public virtual void Enter()
        {
            OnEnter?.Invoke();
        }

        public virtual void Leave()
        {
            OnLeave?.Invoke();
        }

        public virtual void MouseDown(MouseButtonEventArgs e)
        {
            foreach(var obj in objects.OfType<IMouseListener>()) {
                obj.MouseDown(e);
            }
        }

        public virtual void MouseUp(MouseButtonEventArgs e)
        {
            foreach(var obj in objects.OfType<IMouseListener>()) {
                obj.MouseUp(e);
            }
        }

        public virtual void KeyDown(KeyEventArgs e)
        {
            //
        }

        #endregion
        
        #region Utils

        public LinkedListNode<GameObject> Add(GameObject obj)
        {
            if (obj == null) {
                throw new ArgumentNullException($"Trying to add null object of type '{nameof(obj)}' to Room");
            }
            var acceptor = obj as IHasRoomAccess;
            var node = objects.AddLast(obj);
            acceptor?.Created(this, node);
            return node;
        }

        public LinkedListNode<GameObject> Add<T>(params object[] args) where T : GameObject
        {
            var obj = (T) Activator.CreateInstance(typeof(T), args);
            return Add(obj);
        }

        public void Remove(LinkedListNode<GameObject> node)
        {
            if (node.List == objects) {
                objects.Remove(node);
            }
        }

        public void Clear()
        {
            objects.Clear();
        }

        #endregion
    }
}
