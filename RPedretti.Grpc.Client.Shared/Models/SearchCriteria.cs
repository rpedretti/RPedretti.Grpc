using System;

namespace RPedretti.Grpc.Client.Shared.Models
{
    public class SearchCriteria
    {
        public string Title { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}
