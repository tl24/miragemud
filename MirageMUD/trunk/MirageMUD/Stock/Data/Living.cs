using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Mirage.Core.Communication;
using System.Xml.Serialization;
using Mirage.Core.Data.Query;
using JsonExSerializer;
using Mirage.Core.Data;

namespace Mirage.Stock.Data
{
    /// <summary>
    ///     The gender for a Living (Player or Mobile)
    /// </summary>
    public enum GenderType {
        /// <summary>
        ///     Male
        /// </summary>
        Male,

        /// <summary>
        ///     Female
        /// </summary>
        Female,

        /// <summary>
        ///     Other or Unknown
        /// </summary>
        Other
    }

    /// <summary>
    ///     A base class for living breathing things such as players
    /// and mobiles.
    /// </summary>
    public abstract class Living : BaseData, IViewable, IContainable, IActor
    {
        private string _title;
        private int _level;
        private GenderType _gender;
        private string _shortDescription;
        private string _longDescription;
        private Room _room;

        public Living()
        {
            _level = 1;
            _gender = GenderType.Other;
        }
        /// <summary>
        ///     The player or mobile's level
        /// </summary>

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        ///     The object's gender
        /// </summary>
        public GenderType Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        public abstract void Write(IMessage message);

        #region IViewable Members

        /// <summary>
        ///     The name of this animate
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// A short description of this animate
        /// </summary>
        public string ShortDescription
        {
            get { return _shortDescription; }
            set { _shortDescription = value; }
        }

        /// <summary>
        /// A long description of this animate
        /// </summary>
        public string LongDescription
        {
            get { return _longDescription; }
            set { _longDescription = value; }
        }

        #endregion

        #region IContainable Members

        [XmlIgnore]
        [JsonExIgnore]
        public IContainer Container
        {
            get
            {
                return _room;
            }
            set
            {
                _room = (Room)value;
            }
        }

        public bool CanBeContainedBy(IContainer container)
        {
            return (container is Room);
        }

        #endregion
    }
}
