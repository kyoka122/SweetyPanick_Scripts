using MyApplication;
using UnityEngine;

namespace Utility
{
    public static class SquareRangeCalculator
    {
        public static bool InSquareRange(Vector2 viewPortPos,SquareRange range)
        {
            bool inSquareRangeX = range.canExistRangeXMin < viewPortPos.x && viewPortPos.x < range.canExistRangeXMax;
            bool inSquareRangeY = range.canExistRangeYMin < viewPortPos.y && viewPortPos.y < range.canExistRangeYMax;
            
            return inSquareRangeX && inSquareRangeY;
        }
        
        public static bool InRangeWithOutBottom(Vector2 viewPortPos,SquareRange range)
        {
            bool inSquareRangeX = range.canExistRangeXMin < viewPortPos.x && viewPortPos.x < range.canExistRangeXMax;
            bool inUpRange =  viewPortPos.y < range.canExistRangeYMax;
            
            return inSquareRangeX && inUpRange;
        }

        public static bool InYRange(Vector2 viewPortPos,SquareRange range)
        {
            bool inSquareRangeY = range.canExistRangeYMin < viewPortPos.y && viewPortPos.y < range.canExistRangeYMax;
            
            return inSquareRangeY;
        }
        
        public static bool InXRange(Vector2 viewPortPos,SquareRange range)
        {
            bool inSquareRangeX = range.canExistRangeXMin < viewPortPos.x && viewPortPos.x < range.canExistRangeXMax;
            
            return inSquareRangeX ;
        }

        public static float GetNearRateOfTargetView(Vector2 viewPortPos,SquareRange range)
        {
            if (InSquareRange(viewPortPos, range))
            {
                return 1;
            }

            float xRate=1;
         
            if (viewPortPos.x<=range.canExistRangeXMin)
            {
                xRate = viewPortPos.x / range.canExistRangeXMin;
            }
            else if(viewPortPos.x>=range.canExistRangeXMax)
            {
                xRate = (1 - viewPortPos.x) / (1 - range.canExistRangeXMax);
            }

            float yRate=1;
            if (viewPortPos.y<=range.canExistRangeYMin)
            {
                yRate = viewPortPos.y / range.canExistRangeYMin;
            }
            else if (viewPortPos.y>=range.canExistRangeYMax)
            {
                xRate = (1 - viewPortPos.y) / (1 - range.canExistRangeYMax);
            }

            return (xRate + yRate) / 2f;
        }
    }
}