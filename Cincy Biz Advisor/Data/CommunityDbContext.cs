using Cincy_Biz_Advisor.Models;
using Microsoft.EntityFrameworkCore;

namespace Cincy_Biz_Advisor.Data;

public class CommunityDbContext : DbContext
{
    public CommunityDbContext(DbContextOptions<CommunityDbContext> options)
        : base(options)
    {
    }

    public DbSet<CommunityPost> CommunityPosts => Set<CommunityPost>();
}