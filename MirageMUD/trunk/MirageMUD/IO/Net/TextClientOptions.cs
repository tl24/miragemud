using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.IO.Net
{
    public class TextClientOptions
    {
        public TextClientOptions()
        {            
        }

        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }

        public string TerminalType { get; set; }
    }
}
