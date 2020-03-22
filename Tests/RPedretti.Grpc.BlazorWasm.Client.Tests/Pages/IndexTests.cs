using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using RPedretti.Grpc.BlazorWasm.Client.Pages;
using RPedretti.Grpc.Client.Shared.Models;
using RPedretti.Grpc.Client.Shared.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RPedretti.Grpc.BlazorWasm.Client.Tests.Pages
{
    public class IndexTests
    {
        private readonly TestHost host = new TestHost();
        private readonly Mock<IMovieService> MovieServiceMock;
        private readonly Mock<ILogger<Index>> LoggerMock;

        public IndexTests()
        {
            MovieServiceMock = new Mock<IMovieService>();
            LoggerMock = new Mock<ILogger<Index>>();
            host.AddService(MovieServiceMock.Object);
            host.AddService(LoggerMock.Object);
        }

        [Fact(DisplayName = "Search button should be disabled if no serach term")]
        public void ShouldBeDisabledIfNoSearch()
        {
            MovieServiceMock.Setup(m => m.FindByCriteriaAsync(It.IsAny<Grpc.Client.Shared.Models.SearchCriteria>()))
                .ReturnsAsync(new List<MovieModel>());
            var component = host.AddComponent<Index>();
            var button = component.Find(".btn.btn-primary");

            button.Attributes.Contains("disabled").Should().BeTrue();
        }

        [Fact(DisplayName = "Search button should be enabled if serach by title")]
        public async Task ShouldBeEnabledIfSearchByTitle()
        {
            var component = host.AddComponent<Index>();
            var titleSearch = component.Find("#movieTitle");

            await titleSearch.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = "Movie Title"
            });

            component.Find(".btn.btn-primary").Attributes.Contains("disabled").Should().BeFalse();
        }

        [Fact(DisplayName = "Search button should be enabled if serach by date")]
        public async Task ShouldBeEnabledIfSearchByDate()
        {
            var component = host.AddComponent<Index>();
            var dateSearch = component.Find("#releaseDate");
            await dateSearch.ChangeAsync("2020-01-02");

            component.Find(".btn.btn-primary").Attributes.Contains("disabled").Should().BeFalse();
        }

        [Fact(DisplayName = "Search button should be disabled while searching")]
        public async Task ButtonSearchShouldBeDisabledWhileSearching()
        {
            var findByCriteriaAsyncMutex = new Mutex(true);

            var searchTask = new Task<IEnumerable<MovieModel>>(() =>
            {
                findByCriteriaAsyncMutex.WaitOne();
                return new List<MovieModel>();
            });
            MovieServiceMock.Setup(m => m.FindByCriteriaAsync(It.IsAny<Grpc.Client.Shared.Models.SearchCriteria>()))
                .Returns(searchTask);

            var component = host.AddComponent<Index>();
            var titleSearch = component.Find("#movieTitle");

            await titleSearch.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = "Movie Title"
            });

            var button = component.Find(".btn.btn-primary");

            button.Click();
            searchTask.Start();

            button = component.Find(".btn.btn-primary");
            button.Attributes.Contains("disabled").Should().BeTrue();
            button.SelectSingleNode("./span[@class='spinner-border spinner-border-sm']").Should().NotBeNull();

            host.WaitForNextRender(() => findByCriteriaAsyncMutex.ReleaseMutex());

            button = component.Find(".btn.btn-primary");
            button.Attributes.Contains("disabled").Should().BeFalse();
            button.SelectSingleNode("./span[@class='spinner-border spinner-border-sm']").Should().BeNull();

            findByCriteriaAsyncMutex.Dispose();
        }

        [Theory(DisplayName = "Should render movie with correct date format")]
        [InlineData("pt-BR", "02/01/2020")]
        [InlineData("en-us", "1/2/2020")]
        public async Task ShouldRenderMovie(string culture, string expectedDate)
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
            var component = host.AddComponent<Index>();
            var titleSearch = component.Find("#movieTitle");

            await titleSearch.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = "Movie Title"
            });

            var button = component.Find(".btn.btn-primary");
            button.Attributes.Contains("disabled").Should().BeFalse();
            button.ChildNodes.Should().HaveCount(1);

            button.Click();

            MovieServiceMock.Verify(m => m.FindByCriteriaAsync(It.IsAny<Grpc.Client.Shared.Models.SearchCriteria>()), Times.Once);

            component.Find(".list-group").Should().NotBeNull();
            Assert.Collection(
                component.FindAll(".list-group .list-group-item"),
                item => item.InnerText.Should().Be($"Movie Title Complete ({expectedDate})"),
                item => item.InnerText.Should().Be($"Other Movie Title ({expectedDate})")
            ); ;
        }
    }
}
