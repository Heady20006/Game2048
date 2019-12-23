using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Game2048
{
    public partial class MainWindow
    {
        private int _scores;
        private Key _lastKey;
        private bool _gameOver;

        private double _fieldSize = 4;
        private double _tileSize = 96;
        private bool _playMore = false;
        private bool _bigMode;
        private bool _ableToAnimate;

        private readonly List<Label> _labelToRemove = new List<Label>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            LblScoresInput.Content = "0";
            _scores = 0;
            _playMore = false;
            _gameOver = false;
            GrdField.ShowGridLines = true;
            GrdField.RowDefinitions.Clear();
            GrdField.ColumnDefinitions.Clear();
            for (int i = 0; i < _fieldSize; i++)
            {
                var newC = new ColumnDefinition { Width = new GridLength(_tileSize, GridUnitType.Star) };
                GrdField.ColumnDefinitions.Add(newC);
                var newR = new RowDefinition { Height = new GridLength(_tileSize, GridUnitType.Star) };
                GrdField.RowDefinitions.Add(newR);
            }
            GrdField.Children.Clear();
            CreateNewTile();
            CreateNewTile();
        }

        private void CreateNewTile()
        {
            while (true)
            {
                var randomizer = new Random();
                string content;
                var rCol = randomizer.Next(0, 4);
                var rRow = randomizer.Next(0, 4);
                var ranNumber = randomizer.Next(0, 3);

                if (_bigMode)
                {
                    if (ranNumber == 1)
                        content = "2";
                    else if (ranNumber == 2)
                        content = "4";
                    else
                        content = "8";
                }
                else
                {
                    if (ranNumber == 1)
                        content = "2";
                    else
                        content = "4";
                }
                if (!IsFieldFree(rRow, rCol))
                {
                    break;
                }

                Label newTile = new Label
                {
                    Content = content,
                    Width = _tileSize,
                    Height = _tileSize,
                    Background = GetColor(content),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.White,
                    FontSize = _tileSize / 4
                };

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
                case "4096":
                    return Brushes.PaleVioletRed;
                case "8192":
                    return Brushes.OrangeRed;
                case "16384":
                    return Brushes.IndianRed;
                case "32768":
                    return Brushes.DarkRed;
                default:
                    return Brushes.White;
            }
        }

        private void MoveTile(string direction)
        {
            if (!IsGameOver())
            {
                if (GrdField.Children.Count == _fieldSize * _fieldSize)
                {
                    _gameOver = true;
                }
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

                                WidenObject(10, TimeSpan.FromSeconds(.3), labelToMove);
                                Grid.SetRow(labelToMove, xCoord + xOffset);
                                Grid.SetColumn(labelToMove, yCoord + yOffset);

                                _gameOver = false;
                            }
                            else if (CanCombineTiles(xCoord, xOffset, yCoord, yOffset, labelToMove))
                            {
                                WidenObject(10, TimeSpan.FromSeconds(.3), labelToMove);
                                Grid.SetRow(labelToMove, xCoord + xOffset);
                                Grid.SetColumn(labelToMove, yCoord + yOffset);

                                if ((Convert.ToInt32(labelToMove.Content, CultureInfo.CurrentCulture) * 2 == 2048) & !_playMore)
                                {
                                    MessageBoxResult dialogResult = MessageBox.Show("Möchtest du weiterspielen ? ", "Glückwunsch du hast gewonnen", MessageBoxButton.YesNo);
                                    if (dialogResult == MessageBoxResult.Yes)
                                        _playMore = true;
                                }
                                labelToMove.Content = Convert.ToInt32(labelToMove.Content, CultureInfo.CurrentCulture) * 2;
                                labelToMove.Background = GetColor(labelToMove.Content.ToString());
                                _gameOver = false;
                            }
                        }
                    }
                    foreach (Label toRemove in _labelToRemove)
                    {
                        GrdField.Children.Remove(toRemove);
                    }
                }
            }
            else
            {
                MessageBox.Show("Du hast verloren du Loser (beachte es ist die Beta ^-^)");
            }
        }

        private void Minimize_Completed(object obj, EventArgs e, Label animObject)
        {
            WidenObject(_tileSize, TimeSpan.FromSeconds(.3), animObject);
        }

        private void WidenObject(double newWidth, TimeSpan duration, Label objectToWiden)
        {
            DoubleAnimation animation = new DoubleAnimation(newWidth, duration);
            animation.Completed += new EventHandler((s, e) => Minimize_Completed(s, e, objectToWiden));
            objectToWiden.BeginAnimation(WidthProperty, animation);
            objectToWiden.BeginAnimation(HeightProperty, animation);
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
                            _scores += Convert.ToInt32(offsetTile.Content, CultureInfo.CurrentCulture);
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
            //if (GrdField.Children.Count == _fieldSize * _fieldSize)
            //{
            //    foreach (Label labelToCheck in GrdField.Children)
            //    {
            //        for (int i = 0; i < 4; i++)
            //        {
            //            for (int ii = 0; ii < 4; ii++)
            //            {
            //                bool canCombineOneMore = CanCombineTiles(i, i, ii, ii, labelToCheck);
            //                if (!canCombineOneMore)
            //                {
            //                    return false;
            //                }
            //            }
            //        }
            //    }
            //}
            return _gameOver;
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
            if (_lastKey != e.Key)
            {
                CreateNewTile();
                _lastKey = e.Key;
            }
        }

        private void SetGridSize(object sender, RoutedEventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "Rdb4X4":
                    _fieldSize = 4;
                    _tileSize = 96;
                    _bigMode = false;
                    break;
                case "Rdb6X6":
                    _fieldSize = 6;
                    _tileSize = 60;
                    _bigMode = false;
                    break;
                case "Rdb8X8":
                    _fieldSize = 8;
                    _tileSize = 45;
                    _bigMode = true;
                    break;
            }
            StartGame();
        }
    }
}
