using MyApplication;
using UnityEngine;

namespace Utility
{
    public static class SquareRangeCalculator
    {
        public static bool InSquareRange(Vector2 viewPortPos,SquareRange range)
        {
            bool inSquareRangeX = range.xMin < viewPortPos.x && viewPortPos.x < range.xMax;
            bool inSquareRangeY = range.yMin < viewPortPos.y && viewPortPos.y < range.yMax;
            
            return inSquareRangeX && inSquareRangeY;
        }
        
        public static bool InRangeWithOutBottom(Vector2 viewPortPos,SquareRange range)
        {
            bool inSquareRangeX = range.xMin < viewPortPos.x && viewPortPos.x < range.xMax;
            bool inUpRange =  viewPortPos.y < range.yMax;
            
            return inSquareRangeX && inUpRange;
        }

        public static bool InYRange(Vector2 viewPortPos,SquareRange range)
        {
            bool inSquareRangeY = range.yMin < viewPortPos.y && viewPortPos.y < range.yMax;
            
            return inSquareRangeY;
        }
        
        public static bool InXRange(Vector2 viewPortPos,SquareRange range)
        {
            bool inSquareRangeX = range.xMin < viewPortPos.x && viewPortPos.x < range.xMax;
            
            return inSquareRangeX ;
        }

        public static float GetNearRateOfTargetView(Vector2 viewPortPos,SquareRange range)
        {
            if (InSquareRange(viewPortPos, range))
            {
                return 1;
            }

            float xRate=1;
         
            if (viewPortPos.x<=range.xMin)
            {
                xRate = viewPortPos.x / range.xMin;
            }
            else if(viewPortPos.x>=range.xMax)
            {
                xRate = (1 - viewPortPos.x) / (1 - range.xMax);
            }

            float yRate=1;
            if (viewPortPos.y<=range.yMin)
            {
                yRate = viewPortPos.y / range.yMin;
            }
            else if (viewPortPos.y>=range.yMax)
            {
                xRate = (1 - viewPortPos.y) / (1 - range.yMax);
            }

            return (xRate + yRate) / 2f;
        }
    }
}