using Microsoft.EntityFrameworkCore;
using TicTacToe.Core.Models;

namespace TicTacToe.Persistence
{
    public class TicTacToeContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }

        public TicTacToeContext(DbContextOptions<TicTacToeContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Game>().ToTable("Games");
            builder.Entity<Game>().Property(g => g.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Game>().Property(g => g.Result).IsRequired();
            builder.Entity<Game>().HasMany(g => g.Moves).WithOne(m => m.Game).HasForeignKey(m => m.GameId);

            builder.Entity<Move>().ToTable("Moves");
            builder.Entity<Move>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Move>().Property(m => m.Coordinate).IsRequired().HasMaxLength(2);
        }
    }
}