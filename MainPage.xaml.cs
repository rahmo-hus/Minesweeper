using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Navigation;
using Minesweeper.Models;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Minesweeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Tile> tileList = new List<Tile>();
        private List<Button> buttonList = new List<Button>();
        private SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
        private SolidColorBrush lightSlateGray = new SolidColorBrush(Colors.LightSlateGray);
        private SolidColorBrush black = new SolidColorBrush(Colors.Black);
        private SolidColorBrush red = new SolidColorBrush(Colors.Red);

        private int rows;
        private int columns;
        private int mineTotal;
        private int mineTotalOriginal;

        private string gameState;
        private bool gameActive = true; //is game active

        private int difficultySelected = 0;

        public MainPage()
        {
            this.InitializeComponent();
            this.RequestedTheme = ElementTheme.Dark;
            this.Background = new SolidColorBrush(Colors.DarkSlateGray);
            restartGame();
            InitiateEasyGame();
        }
        private void restartGame_Clicked(object sender, RoutedEventArgs e) {
            if (difficultySelected == 2)
                InitiateHardGame();
            else if (difficultySelected == 1)
                InitiateMediumGame();
            else InitiateEasyGame();
        }
        private void restartGame()
        {
            smileyButton.Content = new Image
            {
                Source = new BitmapImage(new Uri(BaseUri, "Images/smileyRegular.jpg"))
            };
            statusTextBlock.Text = "";
            gameActive = true;
            if (tileList.Count != 0)
            {
                tileList.Clear();
            }
            if (buttonList.Count != 0)
            {
                buttonList.Clear();
            }
            if (gameGrid.ColumnDefinitions.Count() != 0 && gameGrid.RowDefinitions.Count() != 0)
            {
                gameGrid.Children.Clear();
            }
        }
        private void InitiateEasyGame()
        {
            rows = columns = 9;
            mineTotal = 10;
            mineTotalOriginal = mineTotal;
            mineCounterTextBlock.Text = $"{mineTotal}";
            restartGame();
            tileCreator();
            GridCreator();
        }
        private void InitiateMediumGame()
        {
            rows = columns = 16;
            mineTotal = 40;
            mineCounterTextBlock.Text = $"{mineTotal}";
            restartGame();
            tileCreator();
            GridCreator();
        }
        private void InitiateHardGame()
        {
            rows = 16;
            columns = 30;
            mineTotal = 99;
            mineCounterTextBlock.Text = $"{mineTotal}";
            restartGame();
            tileCreator();
            GridCreator();
        }

        private void SetButtonContent(Button b, int minesAround)
        {
            b.Foreground = minesAround == 1 ? new SolidColorBrush(Colors.Blue) : minesAround == 2 ? new SolidColorBrush(Colors.Green) :
                minesAround == 3 ? new SolidColorBrush(Colors.Red) : minesAround == 4 ? new SolidColorBrush(Colors.Purple) : new SolidColorBrush(Colors.Brown);
            b.Content = $"{minesAround}";
        }
        private void easyFlyoutItem_Click(object sender, RoutedEventArgs e) {
            difficultySelected = 0;
            InitiateEasyGame();
        }

        private void mediumFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            difficultySelected = 1;
            InitiateMediumGame();
        }

        private void hardFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            difficultySelected = 2;
            InitiateHardGame();
        }
        #region TileCreator
        private void tileCreator()
        {
            populateTileList();
            createTileMines();
            createTileSurroundings();
        }

        private void populateTileList()
        {
            Tile tile;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    tile = new Tile(i, j);
                    tileList.Add(tile);
                }
        }
        private void createTileMines()
        {
            Random random = new Random();
            while (tileList.FindAll(x => x.IsMine).Count() < mineTotal)
            {
                int rowRnd = random.Next(rows);
                int columnRnd = random.Next(columns);
                if (!(tileList.Find(x => x.Row == rowRnd && x.Column == columnRnd).IsMine))
                {
                    tileList.Find(x => x.Row == rowRnd && x.Column == columnRnd).IsMine = true;
                }

            }
        }

        private void createTileSurroundings()
        {
            Tile tile;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    tile = tileList.Find(x => x.Row == i && x.Column == j);
                    //below of tile
                    if (i + 1 < rows)
                    {
                        tile.Surroundings.Add(tileList.Find(x => x.Row == i + 1 && x.Column == j));
                    }
                    //above
                    if (i - 1 >= 0)
                    {
                        tile.Surroundings.Add(tileList.Find(x => x.Row == i - 1 && x.Column == j));
                    }
                    //right
                    if (j + 1 < columns)
                    {
                        tile.Surroundings.Add(tileList.Find(x => x.Row == i && x.Column == j + 1));
                    }
                    //left of tile
                    if (j - 1 >= 0)
                    {
                        tile.Surroundings.Add(tileList.Find(x => x.Row == i && x.Column == j - 1));
                    }
                    //bottom right diagonal
                    if (i + 1 < rows && j + 1 < columns)
                    {
                        tile.Surroundings.Add(tileList.Find(x => x.Row == i + 1 && x.Column == j + 1));
                    }
                    //top right diagonal
                    if (i - 1 >=0 && j + 1 < columns)
                    {
                        tile.Surroundings.Add(tileList.Find(x => x.Row == i - 1 && x.Column == j + 1));
                    }
                    //bottom left diagonal
                    if (i + 1 < rows && j - 1 >= 0)
                    {
                        tile.Surroundings.Add(tileList.Find(x => x.Row == i + 1 && x.Column == j - 1));
                    }
                    //top left diagonal of tile
                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        tile.Surroundings.Add(tileList.Find(x => x.Row == i - 1 && x.Column == j - 1));
                    }


                }
        }
        #endregion
        #region Grid Creator

        #region Button Creator
        private void buttonCreator()
        {

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    Button b = new Button();
                    b.Tapped += B_Tapped;
                    b.RightTapped += B_RightTapped;
                    buttonList.Add(b);
                    gameGrid.Children.Add(b);
                    Grid.SetColumn(b, j);
                    Grid.SetRow(b, i);
                }
        } 
        #endregion
        private void GridCreator() //create dynamic grid
        {
            RowDefinition rowDefinition;
            ColumnDefinition columnDefinition;
            
            for(int i=0; i<rows; i++)
            {
                rowDefinition = new RowDefinition();
                gameGrid.RowDefinitions.Add(rowDefinition);
            }
            for(int i=0; i<columns; i++)
            {
                columnDefinition = new ColumnDefinition();
                gameGrid.ColumnDefinitions.Add(columnDefinition);
            }

            buttonCreator();

            //show mines only
            //buttonList.ForEach(button =>
            //{
            //    if (tileList.ElementAt(buttonList.IndexOf(button)).IsMine)
            //        button.Background = new SolidColorBrush(Colors.Red);
            //});
        }
        #endregion
        #region Button Events
        private void B_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = (Button)sender;
            int index = buttonList.IndexOf(button);
            Tile t = tileList.ElementAt(index);

            if(gameActive && !t.IsFlagged && !t.IsAmbigous &&!t.IsClicked)
            {
                if (!t.IsMine)
                {
                    NonMineClick((Button)sender,t);
                }
                else
                {
                    MineClick((Button)sender, index);
                }

            }
        }
        private void B_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Button b = (Button)sender;
            int index = buttonList.IndexOf(b);
            Tile t = tileList.ElementAt(index);

            if(gameActive && !t.IsClicked)
            {
                if(!t.IsFlagged && !t.IsAmbigous)
                {
                    b.Content = new Image
                    {
                        Source = new BitmapImage(new Uri(this.BaseUri, "Images/Flag.png")),
                        Width = 35,
                        Height = 40
                    };
                    //b.Content = "F";  //do image
                    t.IsFlagged = true;
                    mineTotal--;
                    mineCounterTextBlock.Text = $"{mineTotal}";
                }
                else if(t.IsFlagged && !t.IsAmbigous)
                {
                    b.Content = new Image
                    {
                        Source = new BitmapImage(new Uri(this.BaseUri, "Images/question.png")),
                        Width = 35,
                        Height = 30
                    };
                    t.IsFlagged = false;
                    t.IsAmbigous = true;
                    mineTotal++;
                    mineCounterTextBlock.Text = $"{mineTotal}";
                }
                else
                {
                    b.Content = "";
                    t.IsFlagged = false;
                    t.IsAmbigous = false;
                }
            }
            if(t.IsClicked && t.Surroundings.Count(x=>x.IsAmbigous) == 0 && t.MinesAround != 0 && t.IsMine && t.Surroundings.Count(x=>x.IsFlagged) == t.MinesAround && gameActive)
            {
                tileList.ForEach(item =>
                {
                    if (item.MinesAround != 0 && !item.IsFlagged)
                    {
                        buttonList.ElementAt(tileList.IndexOf(item)).Background = gray;
                        SetButtonContent(buttonList.ElementAt(tileList.IndexOf(item)), item.MinesAround);
                        item.IsClicked = true;

                    }
                    else if (item.MinesAround == 0)
                    {
                        buttonList.ElementAt(tileList.IndexOf(item)).Background = gray;
                    }
                });
            }
            else if(t.IsClicked && t.Surroundings.Count(x=>x.IsAmbigous) == 0 && !t.IsMine && t.MinesAround != 0 && t.Surroundings.Count(x => x.IsMine) != 0 && gameActive)
            {
                t.Surroundings.ForEach(item =>
                {
                    if (item.IsMine)
                        MineClick(buttonList.ElementAt(tileList.IndexOf(item)), tileList.IndexOf(item)); 
                });
            }
        }
        #endregion
        #region B_Tapped Logic
        private void FloodFill(Tile t) // if fails put for each
        {
            foreach(var item in t.Surroundings) 
            {
                if(item.Row >= 0 && t.Row < rows && t.Column >= 0 && t.Column < columns && !item.IsClicked)
                {
                    if(item.MinesAround == 0)
                    {
                        buttonList.ElementAt(tileList.IndexOf(item)).Background = gray;
                        item.IsClicked = true;
                        FloodFill(item);
                    }
                    else if(item.MinesAround > 0)
                    {
                        buttonList.ElementAt(tileList.IndexOf(item)).Background = gray;
                        SetButtonContent(buttonList.ElementAt(tileList.IndexOf(item)), item.MinesAround);                     
                        item.IsClicked = true;

                    }
                } 
            }
        }

        private void WinCheck()
        {
            gameState = "Game Won";
            statusTextBlock.Text = gameState;
            smileyButton.Content = new Image
            {
                Source = new BitmapImage(new Uri(BaseUri, "Images/smileyWon.jpg"))
            };
            gameActive = false;
        }
        private void NonMineClick(Button b, Tile t)
        {
            b.Background = gray;
            if(t.MinesAround != 0)
            {
                //b.Content = $"{t.MinesAround}";
                SetButtonContent(b, t.MinesAround);
                t.IsClicked = true;
            }
            else if(t.MinesAround ==0)
            {
                FloodFill(t);
            }
            if(tileList.FindAll(x=>x.IsClicked && !x.IsMine).Count() == tileList.Count() - mineTotalOriginal)
                WinCheck();
        }


        private void MineClick(Button b, int index)
        {
            //for (int i = 0; i < tileList.Count; i++)
            //{
            //    if (tileList.ElementAt(i).IsMine)
            //    {
            //        buttonList.ElementAt(i).Background = black;
            //    }
            //    else if(tileList.ElementAt(i).MinesAround!= 0 && tileList.ElementAt(i).IsMine)
            //    {
            //        buttonList.ElementAt(i).Background = lightGray;
            //        buttonList.ElementAt(i).Content = $"{tileList.ElementAt(i).MinesAround}";
            //    }
            //}
            tileList.ForEach(item =>
            {
                int localIndex = tileList.IndexOf(item);
                if (item.IsMine)
                {
                    BitmapImage bmp = new BitmapImage(new Uri(this.BaseUri, "Images/mine.png"));
                    Image image = new Image
                    {
                        Source = bmp,
                        Width = 35,
                        Height = 35
                    };
                    buttonList.ElementAt(localIndex).Content = image;

                    buttonList.ElementAt(localIndex).Background = red;
                }
                else if(item.MinesAround != 0 && item.IsMine)
                {
                    buttonList.ElementAt(localIndex).Background = gray;
                    SetButtonContent(buttonList.ElementAt(localIndex), item.MinesAround);
                }
                else
                {
                   // buttonList.ElementAt(localIndex).Background = gray;
                }
            });
            gameState = "Game over";
            smileyButton.Content = new Image
            {
                Source = new BitmapImage(new Uri(BaseUri, "Images/smileyLoser.jpg"))
            };
            statusTextBlock.Text = gameState;
            gameActive = false;
        }
        #endregion
    }
}
