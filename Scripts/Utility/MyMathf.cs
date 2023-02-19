using UnityEngine;

namespace Utility
{
    public static class MyMathf
    {
        /// <summary>
        /// aをminからmaxの値に丸めて返す
        /// </summary>
        /// <param name="a"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float InRange(float a,float min,float max)
        {
            if (min>max)
            {
                Debug.LogError($"Usage Miss");
            }
            float aMax=Mathf.Min(a, max);
            float aMin = Mathf.Max(aMax, min);

            return aMin;
        }
    }
}