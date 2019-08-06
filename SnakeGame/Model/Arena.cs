using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SnakeGame.Model
{
    /// <summary>
    /// A jatekmenet logikajat tartalmazza
    /// </summary>
    class Arena
    {
        private MainWindow View;

        public Arena(MainWindow view)
        {
            this.View = view;

            //A jatekszabalyok megjelenitese
            View.GamePlayTextBlock.Visibility = System.Windows.Visibility.Visible;

            //A kigyofej megjelenitese
            //A grid-ben az elemek sorban vannak, mint egy listaban. Itt a 10. sor 10. elemet indexeljuk (0-tol)
            //Viszont ez egy altalanos, UIElement tipusu elem lesz, nem ikon
            var cell = View.ArenaGrid.Children[10 * 20 + 10];
            var image = (FontAwesome.WPF.ImageAwesome)cell;
            //ennek mar el lehet kerni az ikon tulajdonsagat
            image.Icon = FontAwesome.WPF.FontAwesomeIcon.Circle;
        }

        internal void KeyDown(KeyEventArgs e)
        {
            //a jatek kezdesehez a 4 nyilbillentyu valamelyiket kell leutni
            switch (e.Key)
            {
                case Key.Left:
                case Key.Up:
                case Key.Right:
                case Key.Down:
                    //Eltuntetjuk a jatekszabalyokat
                    View.GamePlayBorder.Visibility = System.Windows.Visibility.Hidden;
                    View.NumberOfMealsTextBlock.Visibility = System.Windows.Visibility.Visible;
                    View.ArenaGrid.Visibility = System.Windows.Visibility.Visible;

                    //Console.WriteLine($"A lenyomott bill: {e.Key}"); //Ez nekem nem mukodott...
                    Debug.WriteLine($"A lenyomott bill: {e.Key}");
                    break;
            }
            
        }
    }
}
