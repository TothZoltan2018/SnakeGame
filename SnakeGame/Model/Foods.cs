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

        /// <summary>
        /// Egy elem torlese az etelek kozul
        /// </summary>
        /// <param name="rowPosition"></param>
        /// <param name="columnPosition"></param>
        /// <returns>Azzal az etellel ter vissza, amit torolt</returns>
        internal CanvasPosition Remove(int rowPosition, int columnPosition)
        {
            //Az x a FoodPositions lista egy eleme
            //a Single() akkor fut le, ha letezik pontosan egy elem, amire a feltetel igaz!
            //Ha nincs ilyen, vagyh tobb ilyen van, akkor a program elszall
            //var foodToDelete = FoodPositions.Single(x => x.RowPosition == rowPosition 
            //                                        && x.ColumnPosition == columnPosition);
            //Ha a kereses sikertelen, akkor nullaval ter vissza. Ha tobb van, akkor kivetelt dob
            //Most nem lehet tobb elem ugyanazon a helyen, Errol a GetNewFood gondoskodik
            var foodToDelete = FoodPositions.SingleOrDefault(x => x.RowPosition == rowPosition
                                                                && x.ColumnPosition == columnPosition);
            FoodPositions.Remove(foodToDelete);

            return foodToDelete;
        }
    }
}
