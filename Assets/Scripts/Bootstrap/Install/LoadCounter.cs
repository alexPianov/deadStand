using UnityEngine;

namespace Playstel
{
    public static class LoadCounter 
    {
        private static int iterations;
        public static bool isLoaded { get; private set; }

        public static void NewCounter()
        {
            isLoaded = false;
            iterations = 0;
        }

        public static void AddIteration(float maxIterations)
        {
            iterations++;

            if (iterations >= maxIterations)
            {
                isLoaded = true;
            }
        }
    }
}
