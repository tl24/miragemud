using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

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
    public class Animate : BaseData
    {
        private string _title;
        private int _level;
        private SexType _sex;

        public Animate()
        {
            _level = 1;
            _sex = SexType.Other;
        }
        /// <summary>
        ///     The level of this animate
        /// </summary>

        public int level
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        ///     The sex of this animate
        /// </summary>
        public SexType sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        /// <summary>
        ///     The name of this animate
        /// </summary>
        public string title
        {
            get { return _title; }
            set { _title = value; }
        }
    }
}
