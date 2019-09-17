using System;
using System.Collections.Generic;
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
        private bool isGameOver;
        //public TimeSpan startPlayTime => TimeSpan.FromSeconds(130); //csak olvashato property

        //Az app.config.xml-bol konfigolhato 
        private TimeSpan startPlayTime = Properties.Settings.Default.Playtime;
        private int foodUnmaturedTime = Properties.Settings.Default.FoodUnmaturedTime;
        private int foodMaturedTime = Properties.Settings.Default.FoodMaturedTime;
        private int foodWellmaturedTime = Properties.Settings.Default.FoodWellMaturedTime;
        private int foodRothingTime = Properties.Settings.Default.FoodRothingTime;

        private TimeSpan playTime;
        //A tabla meretei
        private int RowCount;
        private int ColumnCount;
        private Random Random; //A tipus nevet is hasznalhatom valtozonevkent
        private Random rndFood;
        //Az etelek nyilvantartasa
        private Foods foods;
        //A megevett etelek utan jaro pontok
        private int score;
        //Ide szabad tenni uj etelet (vagy barmi mast)
        private List<ArenaPosition> EmptyTableArenapositions;
        public Arena(MainWindow view)
        {
            this.View = view;

            InitializeGame();

            Random = new Random();
            rndFood = new Random();            
        }

        /// <summary>
        /// Egy ArenaPosition tpusu lisat hoz letre, amiben a teljes tabla benne van. Ebbol fogjuk majd levonni a mar foglalt 
        /// poyiciokat, ha uj etelet (vagy barmi mast) akarunk megjeleniteni.
        /// </summary>
        /// <returns></returns>
        List<ArenaPosition> GetEmptyTablePositions()
        {
            List<ArenaPosition> emptyArenaPositions = new List<ArenaPosition>();
            
            for (int rowPosition = 0; rowPosition < RowCount; rowPosition++)
            {
                for (int columnPosition = 0; columnPosition < ColumnCount; columnPosition++)
                {            
                    emptyArenaPositions.Add(new ArenaPosition(rowPosition, columnPosition));
                }
            }

            return emptyArenaPositions;
        }

        private void InitializeGame()
        {
            foods = new Foods();
            //az arena meretezeset atveszi a Window Gridbol (ArenaGrid)
            RowCount = View.ArenaGrid.RowDefinitions.Count;
            ColumnCount = View.ArenaGrid.ColumnDefinitions.Count;

            EmptyTableArenapositions = GetEmptyTablePositions();
                        
            //Ha ujrajatszas van, akkor torolni kell a tablat, pl. ures elemekkel a Grid eseteben
            if (isGameOver)
            {
                for (int rowPosition = 0; rowPosition < RowCount; rowPosition++)
                {
                    for (int columnPosition = 0; columnPosition < ColumnCount; columnPosition++)
                    {
                        PaintOnGrid(rowPosition, columnPosition, VisibleElementTypeEnum.EmptyArenaPosition);                        
                    }
                }
                //es eltuntetni a gameover feliratot
                View.EndResultBorder.Visibility = Visibility.Hidden;
                View.EndResultTextBlock.Visibility = Visibility.Hidden;                
            }            

            //A jatekszabalyok megjelenitese
            View.GamePlayTextBlock.Visibility = System.Windows.Visibility.Visible;
            View.GamePlayBorder.Visibility = System.Windows.Visibility.Visible;

            snake = new Snake(RowCount/2, ColumnCount/2);

            StartPendulum();

            isStarted = false;
            isGameOver = false;            
            playTime = startPlayTime;
            pendulumClock = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Clockshock, Application.Current.Dispatcher);
            pendulumClock.Stop();
                       
            score = 0;

            //megjelenitjuk, hogy mennyit ettunk            
            View.NumberOfMealsTextBlock.Text = score.ToString();
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
                //Vege a jateknak
                EndOfGame();
            }

            View.LabelPlaytime.Content = $"{playTime.Minutes:00}:{playTime.Seconds:00}";
        }

        private void ItsTimeForDisplay(object sender, EventArgs e)
        {
            if (!isStarted || isGameOver)
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
            //megprobaljuk torolni az etelt az etelek kozul
            var foodToDelete = foods.Remove(snake.HeadPosition.RowPosition, snake.HeadPosition.ColumnPosition);

            if (foodToDelete != null)
            {
                //ettunk: a kigyo feje el fogja tuntetni az etelt a gridrol
                //igy csak adminisztralnunk kell
                Scoring(foodToDelete);

                //egyek nojon a kigyo hossza
                snake.Length++;

                //es gyorsuljon is
                StartPendulum();
                //megjelenitjuk, hogy mennyit ettunk                
                View.NumberOfMealsTextBlock.Text = score.ToString();

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
                ShowEmptyArenaPosition(end.RowPosition, end.ColumnPosition );
                //majd az adatk kozul is toroljuk 
                snake.Tail.RemoveAt(0);
            }
            UpdateFoods();
        }

        private void Scoring(FoodPosition foodToDelete)
        {
            switch (foodToDelete.Maturity)
            {
                case FoodAgeEnum.UnMatured:
                    score += 100;
                    break;
                case FoodAgeEnum.Matured:
                    score += 200;
                    break;
                case FoodAgeEnum.WellMatured:
                    score += 500;
                    break;
                case FoodAgeEnum.Rothing:
                    score -= 1000;
                    break;
                default:
                    break;
            }
        }

        private void EndOfGame()
        {            
            pendulum.Stop();
            pendulumClock.Stop();
            //ki kell irni, hogy vege van   
            View.EndResultTextBlock.Text = $"Game Over! Press space bar to start a new game!\nScore: {score}";
            View.EndResultTextBlock.Visibility = Visibility.Visible;
            View.EndResultBorder.Visibility = Visibility.Visible;
                                                    
            isStarted = false;
            isGameOver = true;

        }
        
        private void  ShowEmptyArenaPosition(int rowPosition, int columnPosition)
        {
            PaintOnGrid(rowPosition, columnPosition, VisibleElementTypeEnum.EmptyArenaPosition);            
        }

        private void ShowSnakeNeck(int rowPosition, int columnPosition)
        {
            PaintOnGrid(rowPosition, columnPosition, VisibleElementTypeEnum.SnakeNeck);
         }

        private void ShowSnakeHead(int rowPosition, int columnPosition)
        {
            PaintOnGrid(rowPosition, columnPosition, VisibleElementTypeEnum.SnakeHead);

        }

        private void ShowFood(int rowPosition, int columnPosition, FoodAgeEnum foodAge = FoodAgeEnum.UnMatured)
        {
            //Rajzolas a Grid-re
            PaintOnGrid(rowPosition, columnPosition, VisibleElementTypeEnum.Food, foodAge);
            
       }

        /// <summary>
        /// az aktualis jatekido es a szuletesi ido alapjan frissiti a foodPositions-ban az erettseg merteket
        /// </summary>
        void UpdateFoods()
        {
            for (int i = 0; i < foods.FoodPositions.Count; i++)
            {
                var age = foods.FoodPositions[i].BornTime - playTime;
                if (age.Seconds < foodUnmaturedTime)
                {
                    foods.FoodPositions[i].Maturity = FoodAgeEnum.UnMatured;                    
                }
                else if (age.Seconds < foodMaturedTime)
                {
                    foods.FoodPositions[i].Maturity = FoodAgeEnum.Matured;
                }
                else if (age.Seconds < foodWellmaturedTime)
                {
                    foods.FoodPositions[i].Maturity = FoodAgeEnum.WellMatured;
                }
                else if (age.Seconds < foodRothingTime)
                {
                    foods.FoodPositions[i].Maturity = FoodAgeEnum.Rothing;
                }
                else if (foods.FoodPositions.Count > 1)
                {
                    //remove from foodpositions                    
                    PaintOnGrid(foods.FoodPositions[i].RowPosition, foods.FoodPositions[i].ColumnPosition, VisibleElementTypeEnum.EmptyArenaPosition);
                    foods.Remove(foods.FoodPositions[i].RowPosition, foods.FoodPositions[i].ColumnPosition);
                    Debug.WriteLine(foods.FoodPositions.Count);
                    score -=  25; //pazaroltunk...
                    return; //a most torolt etelet nehogy megjelenitsuk az if agak utan                    
                }
                ShowFood(foods.FoodPositions[i].RowPosition, foods.FoodPositions[i].ColumnPosition, foods.FoodPositions[i].Maturity);
            }             
        }

        private void PaintOnGrid(int rowPosition, int columnPosition, VisibleElementTypeEnum visibleType, FoodAgeEnum foodAge = FoodAgeEnum.UnMatured)
        {            
            var image = GetImage(rowPosition, columnPosition);
            switch (visibleType)
            {
                case VisibleElementTypeEnum.SnakeHead:
                    image.Icon = FontAwesome.WPF.FontAwesomeIcon.Circle;
                    image.Foreground = Brushes.Black;
                    image.Opacity = 1; //atallitjuk lathatora, hogy ahol korabban a kigyo mar jartm ott latszodjon
                    break;
                case VisibleElementTypeEnum.SnakeNeck:
                    image.Icon = FontAwesome.WPF.FontAwesomeIcon.Circle;
                    image.Foreground = Brushes.Gray;
                    image.Opacity = 1;
                    break;
                case VisibleElementTypeEnum.Food:
                    switch (foodAge)
                    {
                        case FoodAgeEnum.UnMatured:
                            image.Foreground = Brushes.Green;
                            break;
                        case FoodAgeEnum.Matured:
                            image.Foreground = Brushes.Yellow;
                            break;
                        case FoodAgeEnum.WellMatured:
                            image.Foreground = Brushes.Red;
                            break;
                        case FoodAgeEnum.Rothing:
                            image.Foreground = Brushes.SaddleBrown;
                            break;
                        default:
                            break;
                    }
                    image.Icon = FontAwesome.WPF.FontAwesomeIcon.Apple;                    
                    image.Opacity = 1;
                    break;
                case VisibleElementTypeEnum.EmptyArenaPosition:                    
                    image.Icon = FontAwesome.WPF.FontAwesomeIcon.SquareOutline;
                    //image.Icon = FontAwesome.WPF.FontAwesomeIcon.None; //ez nem mukudik, kis teglalapot hagy hatra...
                    //image.Foreground = Brushes.White; //A mogotte levo szamon (megevett etelek szama) nyomot hagy, ezert ez sem jo
                    image.Opacity = 0; //atallitjuk altlatszora, igy nem latszik.
                    break;
                default:
                    break;
            }
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
            //A jatek kezdese
            if ( !isGameOver && !isStarted &&
                (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down))
            {
                StartNewGame();
            }

            //Ujrakezdes a jatek vege utan
            if (isGameOver && !isStarted && e.Key == Key.Space)
            {
                InitializeGame();
            }

            //Normal jatekmenet: Le kell kezelni a billentyuleuteseket a kigyo iranyitasahoz
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
        }

        private void StartNewGame()
        {
            //Eltuntetjuk a jatekszabalyokat
            View.GamePlayBorder.Visibility = System.Windows.Visibility.Hidden;
            View.NumberOfMealsTextBlock.Visibility = System.Windows.Visibility.Visible;
            View.EndResultBorder.Visibility = Visibility.Hidden;
            View.EndResultTextBlock.Visibility = Visibility.Hidden;

            View.ArenaGrid.Visibility = System.Windows.Visibility.Visible;
            isStarted = true;
            //isGameOver = false;

            //A jatekido visszaszamlalo inditasa
            pendulumClock.Start();

            GetNewFood(1);

        }

        /// <summary>
        /// A fgv feladata, hogy generaljon uj eteleket (max foodNum darabot - veletlenszeruen), amik nem a kigyo fejenek, farkanak 
        /// Todo: meglevo kajanak, vagy kakinak helyen vannak
        /// </summary>
        /// <param name="foodNum"></param>
        private void GetNewFood(int foodNum)
        {            
            int row;
            int col;

            //var limterOfNewFoods = rndFood.Next(0, 2);
            foodNum = rndFood.Next(1, foodNum + 1);

            List<ArenaPosition> snakeArenaPosition = new List<ArenaPosition>(snake.Tail);
            snakeArenaPosition.Add(snake.HeadPosition);

            for (int i = 0; i < foodNum; i++)
            {
                //Mivel a kigyo es a kaja nem tud (legalabbis ez a szandek) azonos poziciot kapni, ezert nem kellene a Select-tel biztositani azt, hogy
                //a Union valoban egyszer szerepeltesse az eredmenyben a ket halmaz elemeit...
                var occupiedArenaPoitions = snakeArenaPosition.Select(x => new { x.RowPosition, x.ColumnPosition })
                                .Union(foods.FoodPositions.Select(x => new { x.RowPosition, x.ColumnPosition }));

                var freeArenaPositions = EmptyTableArenapositions.Select(x => new { x.RowPosition, x.ColumnPosition })
                                .Except(occupiedArenaPoitions.Select(x => new { x.RowPosition, x.ColumnPosition }));

                int aFreeArenaPositionIndex = Random.Next(0, freeArenaPositions.Count() - 1);
                row = freeArenaPositions.ToArray()[aFreeArenaPositionIndex].RowPosition; //.[aFreeArenaPositionIndex].RowPosition;
                col = freeArenaPositions.ToArray()[aFreeArenaPositionIndex].ColumnPosition;
 
                //megjelenitjuk az uj etelt
                ShowFood(row, col);
                //ezzel is jo,
                //foods.FoodPositions.Add(new ArenaPosition(row, col));
                //de...ez meg jobb: Csinalunk egz sajat Add metodust a foods osztalyban, ami a fenti sort implementalja...
                foods.Add(row, col, playTime, FoodAgeEnum.UnMatured);//Todo: folyt kov
            }
        }
    }
}
