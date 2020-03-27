using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Flakesheet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string GetSaveLocation(string tilesPath, string destinationPath)
        {
            string folderName = Path.GetFileName(tilesPath);
            string tilesheetName = $@"{destinationPath}\{folderName}.png";
            bool fileExists = false;
            int i = 0;
            string tilesheetNameTemp = tilesheetName;
            while (!fileExists)
            {
                if (!File.Exists(tilesheetNameTemp))
                {
                    tilesheetName = tilesheetNameTemp;
                    fileExists = true;
                }
                else
                {
                    i++;
                    tilesheetNameTemp = $@"{destinationPath}\{folderName} ({i}).png";
                }
            }
            return tilesheetName;
        }

        void DisposeList(List<Bitmap> list)
        {
            foreach (var i in list)
            {
                i.Dispose();
            }
        }

        void MakeTilesheet(List<Bitmap> tiles, Graphics graphics)
        {
            int x = 0;
            int y = 0;
            int step = 0;
            foreach (var tile in tiles)
            {
                int width = tile.Width;
                int height = tile.Height;
                graphics.DrawImage(tile, new System.Drawing.Point(x, y));
                x += width;
                step++;
                if (step >= Math.Ceiling(Math.Sqrt(tiles.Count())))
                {
                    step = 0;
                    x = 0;
                    y += width;
                }
            }
        }

        void AddTilestoList(List<Bitmap> tiles, string tilesPath) //Function to add tiles to list
        {
            string[] extensions = new[] { ".jpg", ".png", ".bmp" };
            DirectoryInfo dir = new DirectoryInfo(tilesPath);
            foreach (var tile in dir.GetFiles()
                                    .Where(f => extensions.Contains(f.Extension.ToLower()))
                                    .ToList())
            {
                Bitmap sheetTile = new Bitmap($@"{tilesPath}\{tile}");
                tiles.Add(sheetTile);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string tilesPath = TileLabel.Content.ToString();
            string destinationPath = DestinationLabel.Content.ToString();

            if (Directory.Exists(tilesPath) && Directory.Exists(destinationPath))
            {
                List<Bitmap> tiles = new List<Bitmap>();

                AddTilestoList(tiles, tilesPath);

                Console.WriteLine(tiles.Count.ToString());

                int tilesheetWidth = tiles[0].Width * Convert.ToInt32(Math.Ceiling(Math.Sqrt(tiles.Count())));
                int tilesheetHeight = tiles[0].Height * Convert.ToInt32(Math.Ceiling(Math.Sqrt(tiles.Count())));

                Image tilesheet = new Bitmap(tilesheetWidth, tilesheetHeight);
                Graphics graphics = Graphics.FromImage(tilesheet);

                MakeTilesheet(tiles, graphics);


                tilesheet.Save(GetSaveLocation(tilesPath, destinationPath));
                tilesheet.Dispose();
                DisposeList(tiles);
                tiles.Clear();

                Console.WriteLine("Sheeting done!");
            }
        }



        private void TileButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            dlg.Description = "Select a folder where your tiles locate.";
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TileLabel.Content = dlg.SelectedPath;                
            }
        }

        private void DestinationButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            dlg.Description = "Select a folder where you want to save your tilesheet.";
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DestinationLabel.Content = dlg.SelectedPath;
            }
        }
    }
}
