using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.IO
{
    public class TextClientOptions
    {
        public TextClientOptions()
        {
            this.EchoOn = true;
        }

        public bool EchoOn { get; set; }

        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }

    }
}
