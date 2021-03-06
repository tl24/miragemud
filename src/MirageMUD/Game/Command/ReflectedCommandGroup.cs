using System;
using Mirage.Game.World;

namespace Mirage.Game.Command
{
    /// <summary>
    /// Placeholder for the class that contains commands
    /// Which allows for localized argument conversion
    /// </summary>
    public class ReflectedCommandGroup : Mirage.Core.Command.IReflectedCommandGroup
    {
        private Type _groupType;
        private object _instance;

        /// <summary>
        /// Constructs a group for the specified group type.  The group type
        /// is the class that holds the commands.
        /// </summary>
        /// <param name="groupType">the type of the group</param>
        public ReflectedCommandGroup(Type groupType)
        {
            this._groupType = groupType;
        }

        public object GetInstance()
        {
            if (_instance == null)
            {
                MudFactory.RegisterService(_groupType);
                _instance = MudFactory.GetObject(_groupType);
            }
            return _instance;
        }
    }


}

