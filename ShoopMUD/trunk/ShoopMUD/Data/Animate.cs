using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Shoop.Communication;

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
    public abstract class Animate : BaseData, IViewable
    {
        private string _title;
        private int _level;
        private SexType _sex;
        private string _shortDescription;
        private string _longDescription;

        public Animate()
        {
            _level = 1;
            _sex = SexType.Other;
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
    }
}
