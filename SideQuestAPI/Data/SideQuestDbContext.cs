
using Microsoft.EntityFrameworkCore;
using SideQuestAPI.Models;

namespace SideQuestAPI.Data
{
    public class SideQuestDbContext : DbContext
    {
        public SideQuestDbContext(
            DbContextOptions<SideQuestDbContext> options)
            : base(options)
        {
        }

        public DbSet<WeeklyQuest> WeeklyQuests { get; set; }
    }
}
