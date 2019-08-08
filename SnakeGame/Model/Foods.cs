using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Model
{
    class Foods
    {
        public Foods()
        {
            //null objcet pattern
            FoodPositions = new List<ArenaPosition>();
        }
        public List<ArenaPosition> FoodPositions { get; set; }

        internal void Add(int row, int col)
        {
            FoodPositions.Add(new ArenaPosition(row, col));
        }
    }
}
