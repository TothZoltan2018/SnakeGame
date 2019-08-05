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
        internal void KeyDown(KeyEventArgs e)
        {
            //a jatek kezdesehez a 4 nyilbillentyu valamelyiket kell leutni
            switch (e.Key)
            {
                case Key.Left:
                case Key.Up:
                case Key.Right:
                case Key.Down:
                    //Console.WriteLine($"A lenyomott bill: {e.Key}"); //Ez nekem nem mukodott...
                    Debug.WriteLine($"A lenyomott bill: {e.Key}");
                    break;
            }
            
        }
    }
}
