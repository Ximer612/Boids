using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boids
{
    static class RandomGenerator
    {
        private static Random rand;

        static RandomGenerator()
        {
            rand = new Random();
        }

        public static int GetRandomInt(int min, int max)
        {
            return rand.Next(min, max);
        }

        public static float GetRandomFloat()
        {
            return (float)rand.NextDouble();
        }

        public static int GetRandomSign()
        {
            int sign;
            do
            {
                sign = Math.Sign(GetRandomInt(-1, 2));
            }while (sign==0);
            return sign;
        }
    }
}
