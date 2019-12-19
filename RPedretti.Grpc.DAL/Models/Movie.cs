using System;

namespace RPedretti.Grpc.DAL.Models
{
    public class Movie : BaseEntity
    {
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
