using Google.Protobuf.WellKnownTypes;
using RJPSoft.HelperExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace RPedretti.Grpc.Wpf.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool isSearchEnabled = false;
        private readonly Movies.MoviesClient _moviesClient;

        private IEnumerable<MovieModel> _movies;

        public IEnumerable<MovieModel> Movies
        {
            get { return _movies; }
            set
            {
                if (value != _movies)
                {
                    _movies = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSearchEnabled
        {
            get => isSearchEnabled;
            set
            {
                if (value != isSearchEnabled)
                {
                    isSearchEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow(Movies.MoviesClient moviesClient)
        {
            InitializeComponent();
            _moviesClient = moviesClient;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var criteria = new SearchCriteria
            {
                Name = TitleInput.Text
            };
            ReleaseDateInput.SelectedDate.Run(d =>
            {
                var utc = DateTime.SpecifyKind(d, DateTimeKind.Utc);
                criteria.ReleaseDate = Timestamp.FromDateTime(utc);
            });

            var movie = await _moviesClient.SearchByCriteriaAsync(criteria);
            Movies = movie.Movies.Select(m => new MovieModel
            {
                Title = m.Title,
                ReleaseDate = m.ReleaseDate.ToDateTime()
            });
        }

        private void TitleInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateIsSearchEnabled();
        }

        private void ReleaseDateInput_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateIsSearchEnabled();
        }

        private void UpdateIsSearchEnabled()
        {
            IsSearchEnabled = !string.IsNullOrEmpty(TitleInput.Text) || ReleaseDateInput.SelectedDate != null;
        }
    }
}
