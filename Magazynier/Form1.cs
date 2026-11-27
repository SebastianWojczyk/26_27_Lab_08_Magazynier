using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* 1. Mapa (ściany, magazynier, paczki, premie) i wczytywanie
 * 2. Rysowanie mapy
 * 3. Magazynier i chodzenie
 * 4. Kolizje ze ścianami
 * 5. Przepychanie paczek
 * 6. Przebieg gry - zebranie premii i następny poziom
 * 7. Zakończenie gry / restart
 */

namespace Magazynier
{
    public partial class Form1 : Form
    {
        private enum FieldType
        {
            Empty,
            Box,
            Coin,
            Wall
        }
        private FieldType[,] map;
        private int totalCoins;
        private Point Player;
        private const int FieldSize = 80;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            ReadMap(Properties.Resources.Level1);
            Invalidate();
        }

        private void ReadMap(string level1)
        {
            totalCoins = 0;
            string[] lines = level1.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            this.map = new FieldType[lines.Length, lines[0].Length];

            for (int y = 0; y < lines.Length; y++) // y = wiersz (Y)
            {
                for (int x = 0; x < lines[y].Length; x++) // x = kolumna (X)
                {
                    switch (lines[y][x])
                    {
                        case ' ':
                            this.map[y, x] = FieldType.Empty;
                            break;
                        case 'P':
                            this.map[y, x] = FieldType.Empty;
                            Player = new Point(x, y);
                            break;
                        case 'B':
                            this.map[y, x] = FieldType.Box;
                            break;
                        case '*':
                            this.map[y, x] = FieldType.Coin;
                            totalCoins++;
                            break;
                        case '#':
                            this.map[y, x] = FieldType.Wall;
                            break;
                        default:
                            this.map[y, x] = FieldType.Empty;
                            break;
                    }
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            this.ClientSize = new Size(map.GetLength(1) * FieldSize,
                                       map.GetLength(0) * FieldSize);

            e.Graphics.Clear(Color.LightGray);

            // Mapa
            for (int y = 0; y < map.GetLength(0); y++) // Y
            {
                for (int x = 0; x < map.GetLength(1); x++) // X
                {
                    switch (map[y, x])
                    {
                        case FieldType.Wall:
                            e.Graphics.FillRectangle(Brushes.Red, x * FieldSize, y * FieldSize, FieldSize, FieldSize);
                            break;
                        case FieldType.Coin:
                            e.Graphics.FillEllipse(Brushes.Gold, x * FieldSize, y * FieldSize, FieldSize, FieldSize);
                            break;
                        case FieldType.Box:
                            e.Graphics.FillRectangle(Brushes.Brown, x * FieldSize, y * FieldSize, FieldSize, FieldSize);
                            e.Graphics.DrawRectangle(Pens.Black, x * FieldSize, y * FieldSize, FieldSize, FieldSize);
                            break;
                    }
                }
            }
            e.Graphics.FillEllipse(Brushes.Green, Player.X * FieldSize, Player.Y * FieldSize, FieldSize, FieldSize);

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Point newPlayerLocation = GetNewLocation(e.KeyCode, Player);

            //zwraca Point.Empty gdy wcisnięto inny klawisz niż strzłkę
            if (newPlayerLocation != Point.Empty)
            {
                //pusty ruch
                if (map[newPlayerLocation.Y, newPlayerLocation.X] == FieldType.Empty)
                {
                    Player = newPlayerLocation;
                }
                //ruch + moneta
                else if (map[newPlayerLocation.Y, newPlayerLocation.X] == FieldType.Coin)
                {
                    Player = newPlayerLocation;
                    map[newPlayerLocation.Y, newPlayerLocation.X] = FieldType.Empty;
                    totalCoins--;
                    if (totalCoins == 0)
                    {
                        MessageBox.Show("Brawo");
                        //tutaj można zrobić kolejne levele
                        ReadMap(Properties.Resources.Level2);
                    }
                }
                //przepchnięcie paczki gdy "wchodzę" na Box
                else if (map[newPlayerLocation.Y, newPlayerLocation.X] == FieldType.Box)
                {
                    //wyznaczenie potencjalnej pozycji paczki
                    Point newBoxLocation = GetNewLocation(e.KeyCode, newPlayerLocation);
                    //sprawdzenie czy potencjalna pozycja paczki jest pusta
                    if (map[newBoxLocation.Y, newBoxLocation.X] == FieldType.Empty)
                    {
                        //aktualizacja pozycji gracza
                        Player = newPlayerLocation;
                        //zabranie paczki spod gracza
                        map[newPlayerLocation.Y, newPlayerLocation.X] = FieldType.Empty;
                        //postawienie paczki w nowej pozycji
                        map[newBoxLocation.Y, newBoxLocation.X] = FieldType.Box;
                    }
                }
                Invalidate();
            }
        }

        private Point GetNewLocation(Keys key, Point oldLocation)
        {
            Point newLocation = Point.Empty;
            switch (key)
            {
                case Keys.Up:
                    newLocation = new Point(oldLocation.X, oldLocation.Y - 1);
                    break;
                case Keys.Down:
                    newLocation = new Point(oldLocation.X, oldLocation.Y + 1);
                    break;
                case Keys.Left:
                    newLocation = new Point(oldLocation.X - 1, oldLocation.Y);
                    break;
                case Keys.Right:
                    newLocation = new Point(oldLocation.X + 1, oldLocation.Y);
                    break;
            }
            return newLocation;
        }
    }
}
