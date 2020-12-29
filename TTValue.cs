using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andantino
{
    public class TTValue
    {
        public Board state;
        public int value;
        public int depth;
        public int flag;
        public int moveX;
        public int moveY;
        public AI ai;

        public TTValue(Board state, int value, int depth, int flag, int moveX, int moveY, AI ai)
        {
            this.state = state;
            this.value = value;
            this.depth = depth;
            this.flag = flag;
            this.moveX = moveX;
            this.moveY = moveY;
            this.ai = ai;
        }
    }
}
