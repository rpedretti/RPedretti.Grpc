using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.XPath;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RPedretti.Grpc.BlazorWasm.Client.Pages;
using RPedretti.Grpc.Client.Shared.Models;
using RPedretti.Grpc.Client.Shared.Services;
using Xunit;

namespace RPedretti.Grpc.BlazorWasm.Client.Tests.Pages
{
    public class IndexTestBUnit : ComponentTestFixture
    {
        private readonly Mock<IMovieService> MovieServiceMock;
        private readonly Mock<ILogger<Index>> LoggerMock;

        public IndexTestBUnit()
        {
            MovieServiceMock = new Mock<IMovieService>();
            LoggerMock = new Mock<ILogger<Index>>();
            Services.AddSingleton(MovieServiceMock.Object);
            Services.AddSingleton(LoggerMock.Object);
        }

        [Fact(DisplayName = "Search button should be disabled if no search term")]
        public void ShouldBeDisabledIfNoSearch()
        {
            MovieServiceMock.Setup(m => m.FindByCriteriaAsync(It.IsAny<Grpc.Client.Shared.Models.SearchCriteria>()))
                .ReturnsAsync(new List<MovieModel>());
            var component = RenderComponent<Index>();
            var button = component.Find(".btn.btn-primary");

            button.HasAttribute("disabled").Should().BeTrue();
        }

        [Fact(DisplayName = "Search button should be enabled if search by title")]
        public void ShouldBeEnabledIfSearchByTitle()
        {
            var component = RenderComponent<Index>();
            var titleSearch = component.Find("#movieTitle");

            titleSearch.Input("Movie Title");

            component.Find(".btn.btn-primary").HasAttribute("disabled").Should().BeFalse();
        }

        [Fact(DisplayName = "Search button should be enabled if search by date")]
        public void ShouldBeEnabledIfSearchByDate()
        {
            var component = RenderComponent<Index>();
            var dateSearch = component.Find("#releaseDate");
            dateSearch.Change("2020-01-02");

            component.Find(".btn.btn-primary").HasAttribute("disabled").Should().BeFalse();
        }

        [Fact(DisplayName = "Search button should be disabled while searching")]
        public void ButtonSearchShouldBeDisabledWhileSearching()
        {
            var movieServiceLock = new SemaphoreSlim(1, 1);

            var searchTask = new Task<IEnumerable<MovieModel>>(() =>
            {
                movieServiceLock.Wait();
                return new List<MovieModel>();
            });

            MovieServiceMock.Setup(m => m.FindByCriteriaAsync(It.IsAny<Grpc.Client.Shared.Models.SearchCriteria>()))
                .Returns(searchTask);

            movieServiceLock.Wait();
            searchTask.Start();

            var component = RenderComponent<Index>();
            var titleSearch = component.Find("#movieTitle");

            titleSearch.Input("Movie Title");

            var button = component.Find(".btn.btn-primary");

            button.Click();

            // assert is loading
            button.HasAttribute("disabled").Should().BeTrue();

            WaitForState(() =>
            {
                return button.SelectSingleNode("./span[@class='spinner-border spinner-border-sm']") != null;
            });

            movieServiceLock.Release();

            WaitForAssertion(() =>
            {
                button.HasAttribute("disabled").Should().BeFalse();
                button.SelectSingleNode("./span[@class='spinner-border spinner-border-sm']").Should().BeNull();
            });

        }

        [Theory(DisplayName = "Should render movie with correct date format")]
        [InlineData("pt-BR", "02/01/2020")]
        [InlineData("en-us", "1/2/2020")]
        public void ShouldRenderMovie(string culture, string expectedDate)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
            MovieServiceMock.Setup(m => m.FindByCriteriaAsync(It.IsAny<Grpc.Client.Shared.Models.SearchCriteria>()))
                .ReturnsAsync(new List<MovieModel> {
                    new MovieModel
                    {
                        Title = "Movie Title Complete",
                        ReleaseDate = new System.DateTime(2020, 1, 2)
                    },
                    new MovieModel
                    {
                        Title = "Other Movie Title",
                        ReleaseDate = new System.DateTime(2020, 1, 2)
                    }
                });
            var component = RenderComponent<Index>();
            var titleSearch = component.Find("#movieTitle");

            titleSearch.Input("Movie Title");

            var button = component.Find(".btn.btn-primary");
            button.HasAttribute("disabled").Should().BeFalse();
            button.ChildNodes.Should().HaveCount(1);

            button.Click();

            MovieServiceMock.Verify(m => m.FindByCriteriaAsync(It.IsAny<Grpc.Client.Shared.Models.SearchCriteria>()), Times.Once);

            component.Find(".list-group").Should().NotBeNull();
            Assert.Collection(
                component.FindAll(".list-group .list-group-item"),
                item => item.TextContent.Should().Be($"Movie Title Complete ({expectedDate})"),
                item => item.TextContent.Should().Be($"Other Movie Title ({expectedDate})")
            ); ;
        }
    }
}
