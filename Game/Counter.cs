using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class Counter
    {
        #region Basic Frame Counter
        private static int lastTick;
        private static int lastFrameRate;
        private static int frameRate;

        public static int CalculateFrameRate()
        {
            if (System.Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
            return lastFrameRate;
        }

        #endregion
    }
}
