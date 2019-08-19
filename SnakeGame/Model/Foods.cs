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
            FoodPositions = new List<CanvasPosition>();
        }
        public List<CanvasPosition> FoodPositions { get; set; }

        internal void Add(int row, int col, System.Windows.UIElement paint)
        {
            FoodPositions.Add(new CanvasPosition(row, col, paint));
        }

        internal void Remove(int rowPosition, int columnPosition)
        {
            //Az x a FoodPositions lista egy eleme
            //ez a sor akkor fut le, ha letezik pontosan egy elem, amire a feltetel igaz!
            //Ha nincs ilyen, vagyh tobb ilyen van, akkor a program elszall
            var foodToDelete = FoodPositions.Single(x => x.RowPosition == rowPosition 
                                                    && x.ColumnPosition == columnPosition);
            FoodPositions.Remove(foodToDelete);
        }
    }
}
