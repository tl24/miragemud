﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Command;

namespace SampleMud
{
    /// <summary>
    /// Placeholder for the class that contains commands
    /// Which allows for localized argument conversion
    /// </summary>
    public class ReflectedCommandGroup : IReflectedCommandGroup
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
                _instance = Activator.CreateInstance(_groupType);
            }
            return _instance;
        }
    }
}
