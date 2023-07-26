
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Data
{
    public class MoviesDbContext : IdentityDbContext<ApplicationUser>
    {
        public MoviesDbContext(DbContextOptions<MoviesDbContext> options) : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Genre> Genres { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.MovieId, mg.GenreId });

            builder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId);

            builder.Entity<MovieGenre>()
                 .HasOne(mg => mg.Genre)
                 .WithMany(g => g.MovieGenres)
                 .HasForeignKey(mg => mg.GenreId);

            builder.Entity<MovieActor>()
           .HasKey(mg => new { mg.MovieId, mg.ActorId });

            builder.Entity<MovieActor>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(mg => mg.MovieId);

            builder.Entity<MovieActor>()
                 .HasOne(mg => mg.Actor)
                 .WithMany(g => g.MovieActors)
                 .HasForeignKey(mg => mg.ActorId);

            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("Users", "security");
            builder.Entity<IdentityRole>().ToTable("Roles", "security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "security");
        }

    }
}
