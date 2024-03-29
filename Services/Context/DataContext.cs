using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogbackend.Models;
using Microsoft.EntityFrameworkCore;

namespace blogbackend.Services.Context
{
    public class DataContext : DbContext
    {
        public DbSet<UserModel> UserInfo { get; set; }
        public DbSet<BlogItemModel> BlogInfo { get; set; }
        public DataContext(DbContextOptions options): base(options) {}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}