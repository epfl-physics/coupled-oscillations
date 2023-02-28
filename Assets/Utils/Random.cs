using UnityEngine;

namespace Utils
{
    public static class Random
    {
        // Marsaglia polar method for sampling from a Gaussian distribution
        public static float NormalValue(float mu, float sigma)
        {
            float x1, x2, s;
            do
            {
                x1 = 2f * UnityEngine.Random.value - 1f;
                x2 = 2f * UnityEngine.Random.value - 1f;
                s = x1 * x1 + x2 * x2;
            } while (s == 0f || s >= 1f);

            s = Mathf.Sqrt(-2f * Mathf.Log(s) / s);

            return mu + x1 * s * sigma;

            //float y = Mathf.Sqrt(-2f * Mathf.Log(x1)) * Mathf.Sin(2f * Mathf.PI * x2);
            //return y * sigma + mu;
        }
    }
}
