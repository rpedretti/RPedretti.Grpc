using Microsoft.AspNetCore.Components;
using RPedretti.Grpc.Client.Shared.Services;
using SearchCriteriaModel = RPedretti.Grpc.Client.Shared.Models.SearchCriteria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPedretti.Grpc.Client.Shared.Models;
using Microsoft.Extensions.Logging;

namespace RPedretti.Grpc.BlazorWasm.Client.Pages
{
    public partial class Index
    {
        [Inject] private IMovieService MovieService { get; set; }
        [Inject] private ILogger<Index> Logger { get; set; }

        private IEnumerable<MovieModel> Movies = new List<MovieModel>();

        private string Title { get; set; }

        private DateTime? ReleaseDate { get; set; }
        private bool Error { get; set; }

        protected async Task Search()
        {
            Error = false;
            Searched = true;
            Searching = true;

            try
            {
                Movies = await MovieService.FindByCriteriaAsync(new SearchCriteriaModel
                {
                    Title = Title,
                    ReleaseDate = ReleaseDate
                });
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                Error = true;
            }

            Searching = false;
        }

        private bool IsDisalbed => string.IsNullOrEmpty(Title) && ReleaseDate == null || Searching;
        private bool Searched { get; set; }
        private bool Searching { get; set; }
        private string ButtonText => Searching ? "Searching..." : "Search";
    }
}
