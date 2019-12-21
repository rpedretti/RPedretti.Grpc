using System;
using System.Collections.Generic;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using RPedretti.Grpc.Uwp.Client.Models;
using RPedretti.Grpc.Client.Shared.Services;
using SharedModels = RPedretti.Grpc.Client.Shared.Models;
using Windows.UI.Xaml.Controls;
using RJPSoft.HelperExtensions;

#nullable enable
namespace RPedretti.Grpc.Uwp.Client.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IMovieService _movieService;
        public MainPageModel Model { get; } = new MainPageModel();

        public MainViewModel(IMovieService movieService)
        {
            _movieService = movieService;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsSearchEnabled));
        }

        public bool IsSearchEnabled
        {
            get => !string.IsNullOrEmpty(Model.Title) || Model.ReleaseDate.HasValue;
        }

        public async void Search()
        {
            Model.Movies = await _movieService.FindByCriteria(new SharedModels.SearchCriteria
            {
                Title = Model.Title ?? "",
                ReleaseDate = Model.ReleaseDate.Let(d => DateTime.SpecifyKind(d, DateTimeKind.Utc))
            });
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            Model.PropertyChanged += Model_PropertyChanged;
            base.OnNavigatedTo(e, viewModelState);
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Model.PropertyChanged -= Model_PropertyChanged;
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }
    }
}
#nullable restore
