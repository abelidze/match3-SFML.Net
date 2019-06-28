using System;
using System.Linq;
using System.Collections.Generic;
using Match3.Rooms;

namespace Match3
{
    public sealed class RoomManager
    {
        #region Singleton

        private static object syncRoot = new Object();
        private static RoomManager instance;
        public static RoomManager Instance
        {
            get
            {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null) {
                            instance = new RoomManager();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion
        
        #region Fields

        private static Room currentRoom;

        #endregion

        #region Properties

        public static Room CurrentRoom
        {
            get => currentRoom;
            private set
            {
                currentRoom?.Leave();
                currentRoom = value;
                currentRoom?.Enter();
            }
        }

        public List<Room> RoomList { get; private set; } = new List<Room>();

        #endregion

        private RoomManager()
        {
            // ...
        }
        
        #region Callbacks

        public static void Start()
        {
            //if (Instance.RoomList.Count == 0) {
            //    throw new InvalidOperationException("RoomList is empty, no starting room is specified");
            //}
            LoadRoom<MenuRoom>();
        }

        #endregion

        #region Utils

        public static void LoadRoom<T>(bool addToList = true) where T : Room, new()
        {
            CurrentRoom = Instance.RoomList.Find(x => x.GetType().Equals(typeof(T)));
            if (CurrentRoom != null) return;

            CurrentRoom = new T();
            if (addToList) {
                Instance.RoomList.Add(CurrentRoom);
            }
        }

        public static void LoadRoom(int index)
        {
            // TODO: throw exception if not found
            CurrentRoom = Instance.RoomList.ElementAtOrDefault(index);
        }

        #endregion
    }
}
