using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class ScattererValidator_RoomSize : ScattererValidator
    {
        public int minRoomSize = 9;

        public override bool Allows(IntVec3 c, Map map)
        {
            Room room = c.GetRoom(map);
            return room != null && room.CellCount > minRoomSize;
        }
    }
}
