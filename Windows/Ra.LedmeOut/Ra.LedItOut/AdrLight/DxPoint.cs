using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedItOut
{
    [DebuggerDisplay("DxPoint {X}, {Y}")]
    class DxPoint
    {

        public DxPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

    }
}
