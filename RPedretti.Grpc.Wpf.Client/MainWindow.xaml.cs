using RJPSoft.HelperExtensions;
using RPedretti.Grpc.Client.Shared.Models;
using RPedretti.Grpc.Client.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private IEnumerable<MovieModel>? _movies;
        private readonly IMovieService _movieService;

        public IEnumerable<MovieModel>? Movies
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

        public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow(IMovieService movieService)
        {
            InitializeComponent();
            _movieService = movieService;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var criteria = new Grpc.Client.Shared.Models.SearchCriteria
            {
                Title = TitleInput.Text,
                ReleaseDate = ReleaseDateInput.SelectedDate.Let(d =>
                {
                    var utc = DateTime.SpecifyKind(d, DateTimeKind.Utc);
                    return utc;
                })
            };

            Movies = await _movieService.FindByCriteria(criteria);
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
