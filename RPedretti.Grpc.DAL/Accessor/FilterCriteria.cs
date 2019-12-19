using System;

namespace RPedretti.Grpc.DAL.Accessor
{
    public class FilterCriteria
    {
        public string? Title { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Title) || ReleaseDate.HasValue;
        }
    }
}