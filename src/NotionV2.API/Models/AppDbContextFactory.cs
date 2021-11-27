using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NotionV2.API.Models
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            string connection = $"Server={Environment.MachineName};Database=NotionV2Db;Trusted_Connection=True;";
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlServer(connection);
            return new AppDbContext(builder.Options);
        }
    }
}