using System;
using SSPLibrary.Models;
using SSPLibrary.Tests.Models;
using Xunit;
using System.Linq;

namespace SSPLibrary.Tests
{

    public class ExpressionBuilderShould : IClassFixture<ExpressionBuilderFixture>
    {

        private readonly ExpressionBuilderFixture _fixture;

        public ExpressionBuilderShould(ExpressionBuilderFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Should_Apply_Paging_And_Return_Data()
        {
            var paged = _fixture.Repository.GetTasks().ToPagedResults(_fixture.QueryParameters);
            Assert.IsType<PagedResults<TodoTaskEntity>>(paged);

            Assert.True(paged.Items.Count() <= _fixture.QueryParameters.PagingParameters.Limit);

            var pagedCollection = paged.ToPagedCollection(_fixture.QueryParameters);
            Assert.IsType<PagedCollection<TodoTaskEntity>>(pagedCollection);

            Assert.NotNull(pagedCollection.First);
            Assert.NotNull(pagedCollection.Next);
            Assert.NotNull(pagedCollection.Last);
            Assert.NotNull(pagedCollection.Previous);   
        }

        [Fact]
        public void Should_Apply_Sorting_And_Return_Data()
        {
            var sortQuery = "-Id,IsDone";

            _fixture.QueryParameters.ApplyQueryParameters(sortQuery, null);

            Assert.Equal(2, _fixture.QueryParameters.SortOptions.SortTerms.Count());

            var sorted = _fixture.Repository.GetTasks().ApplySorting(_fixture.QueryParameters).ToArray();

            Assert.True(sorted[0].Id > sorted[1].Id);
        }

        [Fact]
        public void Should_Apply_Searching_And_Return_Data()
        {
            var searchQuery = "Name==Task 10|Task 21";

            _fixture.QueryParameters.ApplyQueryParameters(null, searchQuery);

            Assert.Single(_fixture.QueryParameters.SearchOptions.SearchTerms);
            Assert.Equal(2, _fixture.QueryParameters.SearchOptions.SearchTerms.FirstOrDefault()?.Value.Count());

            var searched = _fixture.Repository.GetTasks().ApplySearching(_fixture.QueryParameters).ToArray();

            Assert.Equal(2, searched.Count());
        }
    }
}
