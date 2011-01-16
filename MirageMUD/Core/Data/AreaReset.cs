using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
{
    public enum ResetType
    {
        MobReset,
        ItemReset
    }

    public class AreaReset
    {
        private int _id;
        private ResetType _resetType;
        private string _objectUri;
        private string _targetUri;
    }
}
