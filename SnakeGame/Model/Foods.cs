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
            //null object pattern
            FoodPositions = new List<FoodPosition>();
        }
        public List<FoodPosition> FoodPositions { get; set; }

        internal void Add(int row, int col, TimeSpan currentPlayTime, FoodAgeEnum maturity)
        {
            FoodPositions.Add(new FoodPosition(row, col, currentPlayTime, maturity));                        
        }

        /// <summary>
        /// Egy elem torlese az etelek kozul
        /// </summary>
        /// <param name="rowPosition"></param>
        /// <param name="columnPosition"></param>
        /// <returns>Azzal az etellel ter vissza, amit torolt</returns>
        internal FoodPosition Remove(int rowPosition, int columnPosition)
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
