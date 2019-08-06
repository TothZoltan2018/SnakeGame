using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using SnakeGame;


namespace SnakeGame.Model
{
    /// <summary>
    /// A jatekmenet logikajat tartalmazza
    /// </summary>
    class Arena
    {
        private MainWindow View;
        private Snake snake;
        private DispatcherTimer pendulum, pendulumClock;
        private bool isStarted;
        private TimeSpan playTime = TimeSpan.FromSeconds(120); //Todo Attenni a konstruktorba

        public Arena(MainWindow view)
        {
            this.View = view;

            //A jatekszabalyok megjelenitese
            View.GamePlayTextBlock.Visibility = System.Windows.Visibility.Visible;

            snake = new Snake(10, 10);

            pendulum = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal,
                        ItsTimeForDisplay, Application.Current.Dispatcher);

            isStarted = false;

            pendulumClock = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Clockshock, Application.Current.Dispatcher);
            pendulumClock.Stop();
       }

        private void Clockshock(object sender, EventArgs e)
        {
            playTime -= TimeSpan.FromSeconds(1);
            if (playTime == TimeSpan.FromSeconds(0))
            {
                //Todo Vege a jateknak
                
            }
            
            View.LabelPlaytime.Content = $"{playTime.Minutes:00}:{playTime.Seconds:00}";
        }

        private void ItsTimeForDisplay(object sender, EventArgs e)
        {
            if (!isStarted)
            {
                return;
            }
            //Meg kell jegyezni, hogy a kigyo feje hol van
            //Ez hibas megoldas, mert a currentPosition ugyanarra az objektumra fog mutatni, azaz nem taroltuk el az eredeti erteket!!!
            //var currentPosition = snake.HeadPosition;

            //Igy mar helyesen mukodik a regi ertek mentese egy uj peldanyba
            var currentPosition = new ArenaPosition(snake.HeadPosition.RowPosition, snake.HeadPosition.ColumnPosition);

            //Ki kell szamolni a kovetkezo poziciot a fej iranya alapjan
            switch (snake.Heading)
            {
                case SnakeHeadingEnum.Up:
                    snake.HeadPosition.RowPosition -= 1;
                    break;
                case SnakeHeadingEnum.Down:
                    snake.HeadPosition.RowPosition += 1;
                    break;
                case SnakeHeadingEnum.Left:
                    snake.HeadPosition.ColumnPosition -= 1;
                    break;
                case SnakeHeadingEnum.Right:
                    snake.HeadPosition.ColumnPosition += 1;
                    break;
                case SnakeHeadingEnum.InPlace:
                    break;
                default:
                    break;
            }
            //Ki kell rajzolni a kovetkezo poziciora a kigyo fejet
            //A kigyofej megjelenitese
            //A grid-ben az elemek sorban vannak, mint egy listaban. Ez a gyujtemeny a Children.
            //Viszont ez egy altalanos, UIElement tipusu elem lesz, nem ikon
            var cell = View.ArenaGrid.Children[snake.HeadPosition.RowPosition * 20 + snake.HeadPosition.ColumnPosition];
            var image = (FontAwesome.WPF.ImageAwesome)cell;
            //ennek mar el lehet kerni az ikon tulajdonsagat
            image.Icon = FontAwesome.WPF.FontAwesomeIcon.Circle;
                        
            //El kell tuntetni a korabbi poziciorol
            cell = View.ArenaGrid.Children[currentPosition.RowPosition * 20 + currentPosition.ColumnPosition];
            image = (FontAwesome.WPF.ImageAwesome)cell;
            image.Icon = FontAwesome.WPF.FontAwesomeIcon.SquareOutline;
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
                    if (!isStarted)
                    {
                        StartNewGame();
                    }

                    //Le kell kezelni a billentyuleuteseket
                    switch (e.Key)
                    {
                        case Key.Left:
                            snake.Heading = SnakeHeadingEnum.Left;
                            break;
                        case Key.Up:
                            snake.Heading = SnakeHeadingEnum.Up;
                            break;
                        case Key.Right:
                            snake.Heading = SnakeHeadingEnum.Right;
                            break;
                        case Key.Down:
                            snake.Heading = SnakeHeadingEnum.Down;
                            break;
                    }
                    //Console.WriteLine($"A lenyomott bill: {e.Key}"); //Ez nekem nem mukodott...
                    Debug.WriteLine($"A lenyomott bill: {e.Key}");
                    break;
            }            
        }

        private void StartNewGame()
        {
            //Eltuntetjuk a jatekszabalyokat
            View.GamePlayBorder.Visibility = System.Windows.Visibility.Hidden;
            View.NumberOfMealsTextBlock.Visibility = System.Windows.Visibility.Visible;
            View.ArenaGrid.Visibility = System.Windows.Visibility.Visible;
            isStarted = true;
            pendulumClock.Start();
        }
    }
}
