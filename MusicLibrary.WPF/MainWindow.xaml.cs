using Microsoft.Win32;
using MusicLibrary.Core;
using MusicLibrary.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace MusicLibrary.WPF
{
    public partial class MainWindow : Window
    {
        private MusicCollection collection;
        private readonly MusicRepository repository;

        public MainWindow()
        {
            InitializeComponent();

            collection = new MusicCollection();
            repository = new MusicRepository();

            AddTestData();
            UpdateGenreList();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            var query = collection.AsEnumerable();

            string search = SearchTextBox.Text;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                    || x.Artist.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (GenreComboBox.SelectedItem != null && GenreComboBox.SelectedItem.ToString() != "Все")
            {
                string genre = GenreComboBox.SelectedItem.ToString();
                query = query.Where(x => x is Track && ((Track)x).Genre == genre);
            }
            query = query.OrderByDescending(x => x.Rating);

            MusicDataGrid.ItemsSource = query.ToList();
        }

        private void UpdateGenreList()
        {

            var genres = collection
                .OfType<Track>()
                .Select(x => x.Genre)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            GenreComboBox.Items.Clear();
            GenreComboBox.Items.Add("Все");

            foreach (string genre in genres)
            {
                GenreComboBox.Items.Add(genre);
            }

            GenreComboBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int year;
                double rating;

                if (!int.TryParse(YearTextBox.Text, out year))
                    throw new MusicItemException("Год должен быть целым числом.");

                if (!double.TryParse(RatingTextBox.Text, out rating))
                    throw new MusicItemException("Оценка должна быть числом.");

                if (rating < 0 || rating > 10)
                    throw new MusicItemException("Оценка должна находиться от 0 до 10.");

                int id = collection.Count == 0 ? 1 : collection.Max(x => x.Id) + 1;

                Track track = new Track
                {
                    Id = id,
                    Artist = ArtistTextBox.Text,
                    Title = TitleTextBox.Text,
                    Album = AlbumTextBox.Text,
                    Genre = GenreTextBox.Text,
                    Year = year,
                    Rating = rating
                };

                collection.Add(track);

                UpdateGenreList();
                RefreshGrid();

                ClearInputFields();
            }
            catch (MusicItemException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message, "Неизвестная ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Track selected = MusicDataGrid.SelectedItem as Track;

            if (selected == null)
            {
                MessageBox.Show("Выберите композицию.");
                return;
            }

            collection.Remove(selected);

            UpdateGenreList();
            RefreshGrid();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "XML files (*.xml)|*.xml";
            dialog.FileName = "music.xml";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    repository.Save(collection, dialog.FileName);

                    MessageBox.Show("Коллекция сохранена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка сохранения");
                }
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {Filter = "XML files (*.xml)|*.xml"};

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    collection = repository.Load(dialog.FileName);

                    UpdateGenreList();
                    RefreshGrid();

                    MessageBox.Show("Коллекция загружена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка загрузки");
                }
            }
        }

        private void SaveTextButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                FileName = "music.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    StringBuilder builder = new StringBuilder();

                    foreach (MusicItem item in collection)
                    {
                        builder.AppendLine(
                            item.Artist + " - " +
                            item.Title + " (" +
                            item.Year + "), " +
                            item.Rating);
                    }

                    File.WriteAllText(dialog.FileName, builder.ToString(), Encoding.UTF8);

                    MessageBox.Show("Файл сохранён.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка сохранения");
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            RefreshGrid();
        }

        private void GenreComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MusicDataGrid != null)
                RefreshGrid();
        }

        private void ClearInputFields()
        {
            ArtistTextBox.Clear();
            TitleTextBox.Clear();
            AlbumTextBox.Clear();
            GenreTextBox.Clear();
            YearTextBox.Clear();
            RatingTextBox.Clear();
        }

        private void AddTestData()
        {
            collection.Add(new Track
            {
                Id = 1,
                Artist = "Hoshimachi Suisei",
                Title = "Stellar Stellar",
                Album = "Still Still Stellar",
                Genre = "J-Pop",
                Year = 2021,
                Rating = 10
            });

            collection.Add(new Track
            {
                Id = 2,
                Artist = "Snail's House",
                Title = "Pixel Galaxy",
                Album = "Pixel Galaxy",
                Genre = "Electronic",
                Year = 2017,
                Rating = 9
            });

            collection.Add(new Track
            {
                Id = 3,
                Artist = "Example Artist",
                Title = "Example Song",
                Album = "Example Album",
                Genre = "Example Genre",
                Year = 2000,
                Rating = 5
            });
        }

        private void MusicDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}