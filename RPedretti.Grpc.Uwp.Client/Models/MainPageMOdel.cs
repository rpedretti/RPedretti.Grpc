using Prism.Mvvm;
using RPedretti.Grpc.Client.Shared.Models;
using System;
using System.Collections.Generic;

#nullable enable
namespace RPedretti.Grpc.Uwp.Client.Models
{
    public class MainPageModel : BindableBase
    {
        private string? _titleSearch;
        private DateTime? _releaseDate;
        private IEnumerable<MovieModel>? _movies;

        public string? Title
        {
            get => _titleSearch;
            set => SetProperty(ref _titleSearch, value);
        }

        public DateTime? ReleaseDate
        {
            get => _releaseDate;
            set => SetProperty(ref _releaseDate, value);
        }

        public IEnumerable<MovieModel>? Movies {
            get => _movies;
            set => SetProperty(ref _movies, value); }
    }
}
#nullable restore
