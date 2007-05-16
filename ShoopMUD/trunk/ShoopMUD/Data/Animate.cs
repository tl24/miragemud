using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Shoop.Communication;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Shoop.Data
{
    /// <summary>
    ///     The sex for an animate
    /// </summary>
    public enum SexType {
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
    public abstract class Animate : BaseData, IViewable, IContainable
    {
        private string _title;
        private int _level;
        private SexType _sex;
        private string _shortDescription;
        private string _longDescription;
        private Room _room;

        public Animate()
        {
            _level = 1;
            _sex = SexType.Other;
            //TODO: Allow this to be any container
            _uriProperties.Add("Room", _room);
        }
        /// <summary>
        ///     The level of this animate
        /// </summary>

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        ///     The sex of this animate
        /// </summary>
        public SexType Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        public abstract void Write(Message message);

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

        [JsonIgnore]
        [XmlIgnore]
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
