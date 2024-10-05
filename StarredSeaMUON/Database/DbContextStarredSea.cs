using Microsoft.EntityFrameworkCore;
using StarredSeaMUON.Database.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database
{
    internal class DbContextStarredSea : DbContext
    {
        public DbSet<DbRoom> Rooms { get; set; }
        public DbSet<DbItemType> ItemTypes { get; set; }
        public DbSet<DbMobType> MobTypes { get; set; }
        public DbSet<DbAccount> Accounts { get; set; }
        public DbSet<DbMobInstance> MobInstances { get; set; }
        public DbSet<DbItemInstance> ItemInstances { get; set; }

        public string DbPath { get; }



        public DbRoom? GetRoomByID(long id)
        {
            return Rooms.Where(b => b.RoomID == id).First();
        }

        public List<DbMobInstance> GetMobsInRoom(DbRoom room)
        {
            return MobInstances.Where(b => b.CRoomID == room.RoomID).ToList();
        }
        public List<DbItemInstance> GetItemsInRoom(DbRoom room)
        {
            return ItemInstances.Where(b => b.CRoomID == room.RoomID).ToList();
        }

        /// <summary>
        /// Gets a user account by username
        /// </summary>
        /// <param name="username">username to find</param>
        /// <returns>DbAccount instance, or null if none found</returns>
        public DbAccount? GetAccount(string username)
        {
            IQueryable<DbAccount> q = Accounts.Where(b => b.Username == username);
            return (q.Count() > 0) ? q.First() : null;
        }








        public DbContextStarredSea()
        {
            DbPath = Path.Join(Directory.GetCurrentDirectory(), "data.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextStarredSea).Assembly);

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<Metadata>().HaveConversion<ValueConverterMetadata>();
            configurationBuilder.Properties<List<string>>().HaveConversion<ValueConverterListString>();
            configurationBuilder.Properties<Dictionary<string, string>>().HaveConversion<ValueConverterDictStringString>();
            configurationBuilder.Properties<List<Exit>>().HaveConversion<ValueConverterListExit>();
        }
    }
}
