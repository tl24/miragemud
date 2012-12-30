using Mirage.Game.World.Query;

namespace Mirage.Game.World
{
    public class MobReset : ResetBase
    {
        private string _roomUri;
        private Room _targetRoom;

        public MobReset()
        {
        }

        public string RoomUri
        {
            get { return this._roomUri; }
            set
            {
                this._roomUri = value;
                _targetRoom = null;
            }
        }

        public Room GetRoom() 
        {
            if (_targetRoom == null)
            {
                MudWorld world = MudFactory.GetObject<MudWorld>();

                // absolute link
                _targetRoom = (Room)world.ResolveUri(_roomUri);

            }
            return _targetRoom;

        }
    }
}
