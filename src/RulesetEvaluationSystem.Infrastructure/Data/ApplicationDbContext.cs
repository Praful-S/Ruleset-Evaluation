using Microsoft.EntityFrameworkCore;
using RulesetEvaluationSystem.Domain.Entities;
using RulesetEvaluationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesetEvaluationSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ruleset> Rulesets { get; set; }
        public DbSet<RulesetCondition> RulesetConditions { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<RuleCondition> RuleConditions { get; set; }
        public DbSet<EvaluationLog> EvaluationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ruleset Configuration
            modelBuilder.Entity<Ruleset>(entity =>
            {
                entity.ToTable("tbl_Ruleset");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn(1, 1);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

                entity.HasIndex(e => e.Name).HasDatabaseName("IX_tbl_Ruleset_Name");

                entity.HasMany(e => e.Conditions)
                    .WithOne(e => e.Ruleset)
                    .HasForeignKey(e => e.RulesetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Rules)
                    .WithOne(e => e.Ruleset)
                    .HasForeignKey(e => e.RulesetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RulesetCondition Configuration
            modelBuilder.Entity<RulesetCondition>(entity =>
            {
                entity.ToTable("tbl_RulesetCondition");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn(1, 1);

                entity.Property(e => e.Field).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Value).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Operator)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

                entity.HasIndex(e => e.RulesetId).HasDatabaseName("IX_tbl_RulesetCondition_RulesetId");
            });

            // Rule Configuration
            modelBuilder.Entity<Rule>(entity =>
            {
                entity.ToTable("tbl_Rule");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn(1, 1);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Priority).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.ProductionPlant).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

                entity.HasIndex(e => e.Priority).HasDatabaseName("IX_tbl_Rule_Priority");
                entity.HasIndex(e => e.RulesetId).HasDatabaseName("IX_tbl_Rule_RulesetId");

                entity.HasMany(e => e.Conditions)
                    .WithOne(e => e.Rule)
                    .HasForeignKey(e => e.RuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RuleCondition Configuration
            modelBuilder.Entity<RuleCondition>(entity =>
            {
                entity.ToTable("tbl_RuleCondition");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn(1, 1);

                entity.Property(e => e.Field).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Value).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Operator)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

                entity.HasIndex(e => e.RuleId).HasDatabaseName("IX_tbl_RuleCondition_RuleId");
            });

            // EvaluationLog Configuration
            modelBuilder.Entity<EvaluationLog>(entity =>
            {
                entity.ToTable("tbl_EvaluationLog");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn(1, 1);

                entity.Property(e => e.OrderId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.InputJson).IsRequired();
                entity.Property(e => e.Matched).IsRequired();
                entity.Property(e => e.MatchedRulesetName).HasMaxLength(200);
                entity.Property(e => e.MatchedRuleName).HasMaxLength(200);
                entity.Property(e => e.ProductionPlant).HasMaxLength(100);
                entity.Property(e => e.Reason).HasMaxLength(1000);
                entity.Property(e => e.EvaluatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

                entity.HasIndex(e => e.OrderId).HasDatabaseName("IX_tbl_EvaluationLog_OrderId");
                entity.HasIndex(e => e.EvaluatedAt).HasDatabaseName("IX_tbl_EvaluationLog_EvaluatedAt");
                entity.HasIndex(e => new { e.Matched, e.EvaluatedAt })
                    .HasDatabaseName("IX_tbl_EvaluationLog_Matched_EvaluatedAt");
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // ============================================
            // RULESET ONE - Publisher 99990
            // ============================================
            modelBuilder.Entity<Ruleset>().HasData(
                new Ruleset
                {
                    Id = 1,
                    Name = "Ruleset One",
                    Description = "Rules for Publisher 99990",
                    CreatedAt = now,
                    IsActive = true
                }
            );

            modelBuilder.Entity<RulesetCondition>().HasData(
                new RulesetCondition
                {
                    Id = 1,
                    RulesetId = 1,
                    Field = "PublisherNumber",
                    Operator = OperatorType.Equals,
                    Value = "99990",
                    CreatedAt = now,
                    IsActive = true
                },
                new RulesetCondition
                {
                    Id = 2,
                    RulesetId = 1,
                    Field = "OrderMethod",
                    Operator = OperatorType.Equals,
                    Value = "POD",
                    CreatedAt = now,
                    IsActive = true
                }
            );

            modelBuilder.Entity<Rule>().HasData(
                new Rule
                {
                    Id = 1,
                    RulesetId = 1,
                    Name = "Rule 1",
                    Description = "PB books to US for Publisher 99990",
                    Priority = 1,
                    ProductionPlant = "US",
                    CreatedAt = now,
                    IsActive = true
                }
            );

            modelBuilder.Entity<RuleCondition>().HasData(
                new RuleCondition
                {
                    Id = 1,
                    RuleId = 1,
                    Field = "BindTypeCode",
                    Operator = OperatorType.Equals,
                    Value = "PB",
                    CreatedAt = now,
                    IsActive = true
                },
                new RuleCondition
                {
                    Id = 2,
                    RuleId = 1,
                    Field = "IsCountry",
                    Operator = OperatorType.Equals,
                    Value = "US",
                    CreatedAt = now,
                    IsActive = true
                },
                new RuleCondition
                {
                    Id = 3,
                    RuleId = 1,
                    Field = "PrintQuantity",
                    Operator = OperatorType.LessThanOrEqual,
                    Value = "20",
                    CreatedAt = now,
                    IsActive = true
                }
            );

            // ============================================
            // RULESET TWO - Publisher 99999
            // ============================================
            modelBuilder.Entity<Ruleset>().HasData(
                new Ruleset
                {
                    Id = 2,
                    Name = "Ruleset Two",
                    Description = "Rules for Publisher 99999",
                    CreatedAt = now,
                    IsActive = true
                }
            );

            modelBuilder.Entity<RulesetCondition>().HasData(
                new RulesetCondition
                {
                    Id = 3,
                    RulesetId = 2,
                    Field = "PublisherNumber",
                    Operator = OperatorType.Equals,
                    Value = "99999",
                    CreatedAt = now,
                    IsActive = true
                },
                new RulesetCondition
                {
                    Id = 4,
                    RulesetId = 2,
                    Field = "OrderMethod",
                    Operator = OperatorType.Equals,
                    Value = "POD",
                    CreatedAt = now,
                    IsActive = true
                }
            );

            // Ruleset Two - Rules (Updated with Rule 4)
            modelBuilder.Entity<Rule>().HasData(
                new Rule
                {
                    Id = 2,
                    RulesetId = 2,
                    Name = "Rule 2",
                    Description = "CV books to UK for Publisher 99999",
                    Priority = 1,
                    ProductionPlant = "UK",
                    CreatedAt = now,
                    IsActive = true
                },
                new Rule
                {
                    Id = 3,
                    RulesetId = 2,
                    Name = "Rule 3",
                    Description = "PB books with high quantity to KGL for Publisher 99999",
                    Priority = 2,
                    ProductionPlant = "KGL",
                    CreatedAt = now,
                    IsActive = true
                },
                // *** NEW RULE 4 ADDED HERE ***
                new Rule
                {
                    Id = 4,
                    RulesetId = 2,
                    Name = "Rule 4",
                    Description = "PB books with low quantity to US for Publisher 99999",
                    Priority = 3,
                    ProductionPlant = "US",
                    CreatedAt = now,
                    IsActive = true
                }
            );

            modelBuilder.Entity<RuleCondition>().HasData(
                // Rule 2 Conditions (CV, UK, <=20)
                new RuleCondition
                {
                    Id = 4,
                    RuleId = 2,
                    Field = "BindTypeCode",
                    Operator = OperatorType.Equals,
                    Value = "CV",
                    CreatedAt = now,
                    IsActive = true
                },
                new RuleCondition
                {
                    Id = 5,
                    RuleId = 2,
                    Field = "IsCountry",
                    Operator = OperatorType.Equals,
                    Value = "UK",
                    CreatedAt = now,
                    IsActive = true
                },
                new RuleCondition
                {
                    Id = 6,
                    RuleId = 2,
                    Field = "PrintQuantity",
                    Operator = OperatorType.LessThanOrEqual,
                    Value = "20",
                    CreatedAt = now,
                    IsActive = true
                },

                // Rule 3 Conditions (PB, US, >=20)
                new RuleCondition
                {
                    Id = 7,
                    RuleId = 3,
                    Field = "BindTypeCode",
                    Operator = OperatorType.Equals,
                    Value = "PB",
                    CreatedAt = now,
                    IsActive = true
                },
                new RuleCondition
                {
                    Id = 8,
                    RuleId = 3,
                    Field = "IsCountry",
                    Operator = OperatorType.Equals,
                    Value = "US",
                    CreatedAt = now,
                    IsActive = true
                },
                new RuleCondition
                {
                    Id = 9,
                    RuleId = 3,
                    Field = "PrintQuantity",
                    Operator = OperatorType.GreaterThanOrEqual,
                    Value = "20",
                    CreatedAt = now,
                    IsActive = true
                },

                // *** NEW RULE 4 CONDITIONS ADDED HERE ***
                // Rule 4 Conditions (PB, US, <=20)
                new RuleCondition
                {
                    Id = 10,
                    RuleId = 4,
                    Field = "BindTypeCode",
                    Operator = OperatorType.Equals,
                    Value = "PB",
                    CreatedAt = now,
                    IsActive = true
                },
                new RuleCondition
                {
                    Id = 11,
                    RuleId = 4,
                    Field = "IsCountry",
                    Operator = OperatorType.Equals,
                    Value = "US",
                    CreatedAt = now,
                    IsActive = true
                },
                new RuleCondition
                {
                    Id = 12,
                    RuleId = 4,
                    Field = "PrintQuantity",
                    Operator = OperatorType.LessThanOrEqual,
                    Value = "20",
                    CreatedAt = now,
                    IsActive = true
                }
            );
        }
    }
}