using GitCode.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace GitCode.DAL
{
    public class GitCodeContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<DetailTeam> DetailTeams { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<DetailClass> DetailClasses { get; set; }
        public DbSet<DetailClassAccess> DetailClassAccesses { get; set; }
        public DbSet<GitCodeBase> GitCodeBasses { get; set; }
        public DbSet<DetailRepository> DetailRepositories { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Wiki> Wikis { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<WikiDetail> WikiDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}