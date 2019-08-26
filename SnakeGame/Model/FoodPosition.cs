using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SnakeGame.Model
{
    class FoodPosition : CanvasPosition
    {
        //Az eteleket szertnem a jatekido mulasaval oregiteni
        public TimeSpan BornTime { get; set; }
        public FoodAgeEnum Maturity { get; set; }

        public FoodPosition(int rowPosition, int columnPosition, UIElement paint, TimeSpan bornTime, FoodAgeEnum maturity)
            : base(rowPosition, columnPosition, paint) //ezzel a hatterben letrejovo ososztaly peldanyositasa tortenik meg
        {
            BornTime = bornTime;
            Maturity = maturity;
        }
    }

}

