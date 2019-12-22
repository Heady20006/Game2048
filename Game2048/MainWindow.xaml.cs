using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Game2048
{
    public partial class MainWindow
    {
        private int _scores;

        private int _fieldSize = 4;
        private int _tileSize = 96;

        private readonly List<Label> _labelToRemove = new List<Label>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            LblScoresInput.Content = "0";
            _scores = 0;
            GrdField.ShowGridLines = true;
            GrdField.RowDefinitions.Clear();
            GrdField.ColumnDefinitions.Clear();
            for (int i = 0; i < _fieldSize; i++)
            {
                ColumnDefinition newC = new ColumnDefinition { Width = new GridLength(_tileSize, GridUnitType.Star) };
                GrdField.ColumnDefinitions.Add(newC);
                RowDefinition newR = new RowDefinition { Height = new GridLength(_tileSize, GridUnitType.Star) };
                GrdField.RowDefinitions.Add(newR);
            }
            GrdField.Children.Clear();
            CreateNewTile();
            CreateNewTile();
        }

        private void CreateNewTile()
        {
            var randomizer = new Random();
            string content;
            var rCol = randomizer.Next(0, 4);
            var rRow = randomizer.Next(0, 4);

            if (randomizer.Next(0, 2) == 1)
                content = "2";
            else
                content = "4";

            Label newTile = new Label
            {
                Content = content,
                Width = _tileSize,
                Height = _tileSize,
                Background = GetColor(content),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Foreground = Brushes.White,
                FontSize = _tileSize / 2
            };
            if (IsFieldFree(rRow, rCol))
            {
                Grid.SetColumn(newTile, rCol);
                Grid.SetRow(newTile, rRow);
                GrdField.Children.Add(newTile);
            }
        }

        private static Brush GetColor(string number)
        {
            switch (number)
            {
                case "2":
                    return Brushes.SkyBlue;
                case "4":
                    return Brushes.CornflowerBlue;
                case "8":
                    return Brushes.DodgerBlue;
                case "16":
                    return Brushes.RoyalBlue;
                case "32":
                    return Brushes.SlateBlue;
                case "64":
                    return Brushes.DarkSlateBlue;
                case "128":
                    return Brushes.BlueViolet;
                case "256":
                    return Brushes.DarkViolet;
                case "512":
                    return Brushes.MediumVioletRed;
                case "1024":
                    return Brushes.PaleVioletRed;
                case "2048":
                    return Brushes.Violet;
                default:
                    return Brushes.White;
            }
        }

        private void MoveTile(string direction)
        {
            if (!IsGameOver())
            {
                for (int i = 0; i < _fieldSize; i++)
                {

                    int xOffset = 0;
                    int yOffset = 0;
                    switch (direction)
                    {
                        case "up":
                            xOffset = -1;
                            break;
                        case "down":
                            xOffset = 1;
                            break;
                        case "left":
                            yOffset = -1;
                            break;
                        case "right":
                            yOffset = 1;
                            break;
                    }
                    foreach (Label labelToMove in GrdField.Children)
                    {
                        int xCoord = Grid.GetRow(labelToMove);
                        int yCoord = Grid.GetColumn(labelToMove);

                        bool ableToMove = xCoord + xOffset >= 0;
                        if (xCoord + xOffset >= 0)
                            ableToMove = true;

                        if (ableToMove)
                        {
                            if (IsFieldFree(xCoord + xOffset, yCoord + yOffset))
                            {
                                if (!((xCoord + xOffset >= 0 && yCoord + yOffset >= 0) &
                                      (xCoord + xOffset <= _fieldSize - 1 && yCoord + yOffset <= _fieldSize - 1))) continue;
                                Grid.SetRow(labelToMove, xCoord + xOffset);
                                Grid.SetColumn(labelToMove, yCoord + yOffset);
                            }
                            else if (CanCombineTiles(xCoord, xOffset, yCoord, yOffset, labelToMove))
                            {
                                Grid.SetRow(labelToMove, xCoord + xOffset);
                                Grid.SetColumn(labelToMove, yCoord + yOffset);
                                if (Convert.ToInt32(labelToMove.Content) * 2 == 2048)
                                {
                                    MessageBox.Show("Glückwunsch du hast das Spiel geschafft");
                                    this.Close();
                                }
                                labelToMove.Content = Convert.ToInt32(labelToMove.Content) * 2;
                                labelToMove.Background = GetColor(labelToMove.Content.ToString());
                            }
                        }
                    }
                    foreach (Label toRemove in _labelToRemove)
                    {
                        GrdField.Children.Remove(toRemove);
                    }
                }
            }
        }

        private bool CanCombineTiles(int x, int xOffset, int y, int yOffset, Label movingTile)
        {
            foreach (Label offsetTile in GrdField.Children)
            {
                if (Grid.GetRow(offsetTile) == x + xOffset)
                {
                    if (Grid.GetColumn(offsetTile) == y + yOffset)
                    {
                        if (movingTile.Content.ToString() == offsetTile.Content.ToString())
                        {
                            _labelToRemove.Add(offsetTile);
                            _scores += Convert.ToInt32(offsetTile.Content);
                            LblScoresInput.Content = _scores;
                            offsetTile.Content = "";
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsFieldFree(int x, int y)
        {
            foreach (Label itemInField in GrdField.Children)
            {
                if (Grid.GetRow(itemInField) != x) continue;
                if (Grid.GetColumn(itemInField) == y)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsGameOver()
        {
            return GrdField.Children.Count == _fieldSize * _fieldSize;
        }

        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void GetPressedKey(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    MoveTile("up");
                    break;
                case Key.S:
                    MoveTile("down");
                    break;
                case Key.A:
                    MoveTile("left");
                    break;
                case Key.D:
                    MoveTile("right");
                    break;
                case Key.Up:
                    MoveTile("up");
                    break;
                case Key.Down:
                    MoveTile("down");
                    break;
                case Key.Left:
                    MoveTile("left");
                    break;
                case Key.Right:
                    MoveTile("right");
                    break;
            }
        }

        private void GetReleasedKey(object sender, KeyEventArgs e)
        {
            CreateNewTile();
        }

        private void SetGridSize(object sender, RoutedEventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "Rdb4X4":
                    _fieldSize = 4;
                    _tileSize = 96;
                    break;
                case "Rdb6X6":
                    _fieldSize = 6;
                    _tileSize = 60;
                    break;
                case "Rdb8X8":
                    _fieldSize = 8;
                    _tileSize = 45;
                    break;
            }
            StartGame();
        }
    }
}