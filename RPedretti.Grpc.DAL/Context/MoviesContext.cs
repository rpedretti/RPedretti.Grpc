using Microsoft.EntityFrameworkCore;
using RPedretti.Grpc.DAL.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RPedretti.Grpc.DAL.Context
{
    public class MoviesContext : DbContext
    {
        public DbSet<Movie>? Movies { get; set; } = null;

        public MoviesContext() : base()
        {

        }

        public MoviesContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Movie>()
                .Property(m => m.ReleaseDate)
                .HasConversion(d => d, d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        }
    }
}
