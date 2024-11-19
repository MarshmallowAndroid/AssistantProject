using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooeyWpf.Services
{
    internal class RandomService : Singleton<RandomService>
    {
        private readonly Random random;

        public RandomService()
        {
            random = Random.Shared;
        }

        public int Next(int minValue, int maxValue)
        {
            if (maxValue == minValue)
            {
                maxValue++;
            }
            return random.Next(minValue, maxValue);
        }
    }
}
