using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;


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
        private TimeSpan playTime;
        //A tabla meretei
        private int RowCount;
        private int ColumnCount;
        private Random Random; //A tipus nevet is hasznalhatom valtozonevkent
        private Random rndFood;
        //Az etelek nyilvantartasa
        private Foods foods;
        //a megevett etelek szama
        private int foodsHaveEatenCount;

        public Arena(MainWindow view)
        {
            this.View = view;

            //A jatekszabalyok megjelenitese
            View.GamePlayTextBlock.Visibility = System.Windows.Visibility.Visible;

            snake = new Snake(10, 10);

            StartPendulum();

            isStarted = false;

            playTime = TimeSpan.FromSeconds(120);
            pendulumClock = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Clockshock, Application.Current.Dispatcher);
            pendulumClock.Stop();

            //az arena mereteinek beallitasa
            //todo: az arena meretezeset atvenni a Window Gridbol (ArenaGrid), nem pedig bedrotozni a meretet.
            RowCount = 20;
            ColumnCount = 20;

            Random = new Random();
            rndFood = new Random();

            foods = new Foods();

            foodsHaveEatenCount = 0;
        }

        private void StartPendulum()
        {
            //ha fut az ingaoram, akkor megallitjuk
            if (pendulum != null && pendulum.IsEnabled)
            {
                pendulum.Stop();
            }

            //ujrainditjuk, vagy elinditjuk a kigyo hosszanak megfeleloen
            var interval = 2000 / snake.Length;
            pendulum = new DispatcherTimer(TimeSpan.FromMilliseconds(interval), DispatcherPriority.Normal,
                        ItsTimeForDisplay, Application.Current.Dispatcher);
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
            var neck = new ArenaPosition(snake.HeadPosition.RowPosition, snake.HeadPosition.ColumnPosition);

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

            //Megvan a fej uj pozicioja. Meg a megjelenites elott ellenorizni kell, hogy:
            //falnak mentunk-e?
            if (snake.HeadPosition.RowPosition < 0 ||
                snake.HeadPosition.RowPosition > RowCount - 1 ||
                snake.HeadPosition.ColumnPosition < 0 ||
                snake.HeadPosition.ColumnPosition > ColumnCount - 1)
            {//Jatek vege                
                EndOfGame();
                //Mivel vege a jateknak, nem csinalunk mar semmit
                return;
            }

            //Sajat farkunkba haraptunk -e?

            //old school
                    //var collision = false;
                    //foreach (var tailitem in snake.Tail)
                    //{//Vegigmengyunk a farokpontokon, ellenorizzuk, hogy az uj fej poz, egyezik -e?
                    //    if (tailitem.RowPosition == snake.HeadPosition.RowPosition && tailitem.ColumnPosition == snake.HeadPosition.ColumnPosition)
                    //    {
                    //        collision = true;
                    //    }
                    //}
                    //if (collision)
                    //{//utkoztunk
                    //    EndOfGame();
                    //}

            //LINQ megoldas
            if (snake.Tail.Any(x => x.RowPosition == snake.HeadPosition.RowPosition
                                 && x.ColumnPosition == snake.HeadPosition.ColumnPosition))
            {//utkoztunk
                EndOfGame();
                return;
            }

            //ellenorini, hogy ettunk -e? Kigyo feje vs etelek listaja
            //Todo: helyezzuk at ezt az ellenorzest a Remove fgv-be, es adja vissza, hogy 
            //true: letezett es torolte, false: nem letezik
            if (foods.FoodPositions.Any(x => x.RowPosition == snake.HeadPosition.RowPosition
                                        && x.ColumnPosition == snake.HeadPosition.ColumnPosition))
            {//ettunk: a kigyo feje el fogja tuntetni az etelt, 
                //igy csak adminisztralnunk 
                //toroljuk az etelt az etelek kozul
                foods.Remove(snake.HeadPosition.RowPosition, snake.HeadPosition.ColumnPosition);

                foodsHaveEatenCount++;

                //egyek nojon a kigyo hossza
                snake.Length++;

                //es gyorsuljon is

                //megjelenitjuk, hogy mennyit ettunk
                View.NumberOfMealsTextBlock.Text = foodsHaveEatenCount.ToString();

                //generalunk uj eteleket
                GetNewFood(2);
            }



            ShowSnakeHead(snake.HeadPosition.RowPosition, snake.HeadPosition.ColumnPosition);

            //A kigyo fejebol nyak lesz, ennek megfeleloen kell megjeleniteni
            ShowSnakeNeck(neck.RowPosition, neck.ColumnPosition);
            //Viszont, a farok adataihoz a nyaknak hozza kell adodnia
            snake.Tail.Add(new ArenaPosition(neck.RowPosition, neck.ColumnPosition));

            //Amig a kigyo hossza kisebb, mint aminek lennie kellene
            if (snake.Tail.Count < snake.Length)
            {//addig nem csinalunk semmit, hadd "novekedjen"

            }
            else
            {//Mar megvan a teljes hossz, ne legyen hosszabb; a kigyo legveget torolni kell, ami az elso elem a listaban
                //Meg torles elott kell az info, hogy melyik ArenaPozicioban van, hogy oda visszarakjuk az eredeti racsot
                var end = snake.Tail[0];
                ShowEmptyArenaPosition(end.RowPosition, end.ColumnPosition);
                //majd az adatk kozul is toroljuk 
                snake.Tail.RemoveAt(0);
            }
        }

        private void EndOfGame()
        {            
            pendulum.Stop();
            pendulumClock.Stop();
            //Todo: ki kell irni, hogy vege van           
            //Todo: lehetoseget adni az ujrajatszasra
        }

        //todo: ebbol a 4 fgv-bol el lehetne tuntetni a duplikaciot
        private void ShowEmptyArenaPosition(int rowPosition, int columnPosition)
        {
            var image = GetImage(rowPosition, columnPosition);
            image.Icon = FontAwesome.WPF.FontAwesomeIcon.SquareOutline;
            image.Foreground = Brushes.Black;
        }

        private void ShowSnakeNeck(int rowPosition, int columnPosition)
        {
            var image = GetImage(rowPosition, columnPosition);
            image.Icon = FontAwesome.WPF.FontAwesomeIcon.Circle;
            image.Foreground = Brushes.Gray;
        }

        private void ShowSnakeHead(int rowPosition, int columnPosition)
        {
            //Ki kell rajzolni a kovetkezo poziciora a kigyo fejet            
            var image = GetImage(rowPosition, columnPosition);
            //ennek mar el lehet kerni az ikon tulajdonsagat
            //A kigyofej megjelenitese Circle ikonnal
            image.Icon = FontAwesome.WPF.FontAwesomeIcon.Circle;
        }

        private void ShowNewFood(int rowPosition, int columnPosition)
        {
            var image = GetImage(rowPosition, columnPosition);
            image.Icon = FontAwesome.WPF.FontAwesomeIcon.Apple;
            image.Foreground = Brushes.Red;

            var paint = new Ellipse();

            //a megjelenites utan az aktualis meretet egy elemnek az "ActualHeight"-tal lehet lekerdezni.
            paint.Height = View.ArenaCanvas.ActualHeight / RowCount;
            paint.Width = View.ArenaCanvas.ActualWidth / ColumnCount;

            paint.Fill = Brushes.Red;

            //A kirajzolando etel koordinatainak szamitasa
            Canvas.SetTop(paint, rowPosition * paint.Height);
            Canvas.SetLeft(paint, columnPosition * paint.Width);

            //Hozzadjuk a Canvas-hoz, ezzel megjelenitjuk
            View.ArenaCanvas.Children.Add(paint);
        }

        private FontAwesome.WPF.ImageAwesome GetImage(int rowPosition, int columnPosition)
        {
            //A grid-ben az elemek sorban vannak, mint egy listaban. Ez a gyujtemeny a Children.
            //Viszont ez egy altalanos, UIElement tipusu elem lesz, nem ikon
            var cell = View.ArenaGrid.Children[rowPosition * RowCount + columnPosition];
            var image = (FontAwesome.WPF.ImageAwesome)cell;
            return image;
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

            //A jatekido visszaszamlalo inditasa
            pendulumClock.Start();

            GetNewFood(1);

        }
        /// <summary>
        /// A fgv feladata, hogy generaljon egy uj etelt, olyat, ami nem a kigyo fejenek vagy farkanak helyen van,
        /// es jelenitse is meg.
        /// </summary>
        private void GetNewFood(int foodNum)
        {
            //A kigyora nem rakhatjuk, ezert addig generalunk uj etelt, amig vegul nem esik ra
            int row;
            int col;

            foodNum = rndFood.Next(1, foodNum+1);
            
            for (int i = 0; i < foodNum; i++)
            {
                do
                {
                    //Etel kiosztasa
                    //Veletlenszeruen
                    row = Random.Next(0, RowCount - 1);
                    col = Random.Next(0, ColumnCount - 1);

                } while (snake.HeadPosition.RowPosition == row && snake.HeadPosition.ColumnPosition == col
                                        || snake.Tail.Any(x => x.RowPosition == row && x.ColumnPosition == col)
                                        || foods.FoodPositions.Any(x => x.RowPosition == row && x.ColumnPosition == col)
                                        );
                //|| snake.Tail.Any(x => x == new ArenaPosition(row, col))) );

                //adminisztraljuk az adatokat

                //ezzel is jo, de...
                //foods.FoodPositions.Add(new ArenaPosition(row, col));
                //Ez meg jobb: Csinalunk egz sajat Add metodust a foods osztalyban, ami a fenti sort implementalja...
                foods.Add(row, col);

                //megjelenitjuk az uj etelt
                ShowNewFood(row, col); 
            }
        }
    }
}
