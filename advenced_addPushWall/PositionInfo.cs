using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace advenced
{
    struct PositionInfo
    {
        public int X;
        public int Y;

        public PositionInfo(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
