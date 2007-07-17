using System;
using System.Collections.Generic;
using System.Text;
using Mirage.IO;
using Mirage.Communication;

namespace MirageGUI.Code
{

    public class MudResponse
    {
        private AdvancedClientTransmitType _type;
        private string _name;
        private object _data;
        
        public MudResponse(AdvancedClientTransmitType type, string name, object data)
        {
            this._type = type;
            this._name = name;
            this._data = data;
        }

        public AdvancedClientTransmitType Type
        {
            get { return this._type; }
        }

        public string Name
        {
            get { return this._name; }
        }

        public object Data
        {
            get { return this._data; }
            set { _data = value; }
        }

    }
}
