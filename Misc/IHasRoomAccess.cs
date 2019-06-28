using System.Collections.Generic;
using Match3.Rooms;
using Match3.Objects;

namespace Match3.Misc
{
    public interface IHasRoomAccess
    {
        void Created(Room room, LinkedListNode<GameObject> node);
    }
}
