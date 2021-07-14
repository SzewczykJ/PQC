﻿// <auto-generated />
using System;
using App.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace App.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("App.Data.Entities.Branch", b =>
                {
                    b.Property<int>("BranchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("BranchId");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("App.Data.Entities.Commit", b =>
                {
                    b.Property<int>("CommitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("BranchId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("DeveloperId")
                        .HasColumnType("integer");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<long>("RepositoryId")
                        .HasColumnType("bigint");

                    b.Property<string>("Sha")
                        .HasColumnType("text");

                    b.HasKey("CommitId");

                    b.HasIndex("BranchId");

                    b.HasIndex("DeveloperId");

                    b.HasIndex("RepositoryId");

                    b.ToTable("Commits");
                });

            modelBuilder.Entity("App.Data.Entities.Developer", b =>
                {
                    b.Property<int>("DeveloperId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("DeveloperId");

                    b.ToTable("Developers");
                });

            modelBuilder.Entity("App.Data.Entities.File", b =>
                {
                    b.Property<int>("FileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CommitId")
                        .HasColumnType("integer");

                    b.Property<int?>("FileDetailId")
                        .HasColumnType("integer");

                    b.Property<string>("SHA")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.HasKey("FileId");

                    b.HasIndex("CommitId");

                    b.HasIndex("FileDetailId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("App.Data.Entities.FileDetail", b =>
                {
                    b.Property<int>("FileDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Extension")
                        .HasColumnType("text");

                    b.Property<string>("FullPath")
                        .HasColumnType("text");

                    b.Property<int?>("LanguageId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("FileDetailId");

                    b.HasIndex("LanguageId");

                    b.ToTable("FileDetails");
                });

            modelBuilder.Entity("App.Data.Entities.Language", b =>
                {
                    b.Property<int>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("LanguageId");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("App.Data.Entities.Metric", b =>
                {
                    b.Property<int>("MetricId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double?>("BranchCoverage")
                        .HasColumnType("double precision");

                    b.Property<int?>("CodeSmells")
                        .HasColumnType("integer");

                    b.Property<int?>("CognitiveComplexity")
                        .HasColumnType("integer");

                    b.Property<int?>("CommentLines")
                        .HasColumnType("integer");

                    b.Property<double?>("CommentLinesDensity")
                        .HasColumnType("double precision");

                    b.Property<int?>("Complexity")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("DuplicatedLines")
                        .HasColumnType("integer");

                    b.Property<int>("FileId")
                        .HasColumnType("integer");

                    b.Property<double?>("LineCoverage")
                        .HasColumnType("double precision");

                    b.Property<int?>("Ncloc")
                        .HasColumnType("integer");

                    b.Property<int?>("NewCodeSmells")
                        .HasColumnType("integer");

                    b.Property<int?>("Statements")
                        .HasColumnType("integer");

                    b.HasKey("MetricId");

                    b.HasIndex("FileId")
                        .IsUnique();

                    b.ToTable("Metrics");
                });

            modelBuilder.Entity("App.Data.Entities.Repository", b =>
                {
                    b.Property<long>("RepositoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Private")
                        .HasColumnType("boolean");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("RepositoryId");

                    b.ToTable("Repositories");
                });

            modelBuilder.Entity("App.Data.Entities.Commit", b =>
                {
                    b.HasOne("App.Data.Entities.Branch", "Branch")
                        .WithMany("Commits")
                        .HasForeignKey("BranchId");

                    b.HasOne("App.Data.Entities.Developer", "Developer")
                        .WithMany()
                        .HasForeignKey("DeveloperId");

                    b.HasOne("App.Data.Entities.Repository", "Repository")
                        .WithMany("Commits")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Developer");

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("App.Data.Entities.File", b =>
                {
                    b.HasOne("App.Data.Entities.Commit", "Commit")
                        .WithMany("Files")
                        .HasForeignKey("CommitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Data.Entities.FileDetail", "FileDetail")
                        .WithMany("Files")
                        .HasForeignKey("FileDetailId");

                    b.Navigation("Commit");

                    b.Navigation("FileDetail");
                });

            modelBuilder.Entity("App.Data.Entities.FileDetail", b =>
                {
                    b.HasOne("App.Data.Entities.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId");

                    b.Navigation("Language");
                });

            modelBuilder.Entity("App.Data.Entities.Metric", b =>
                {
                    b.HasOne("App.Data.Entities.File", "File")
                        .WithOne("Metric")
                        .HasForeignKey("App.Data.Entities.Metric", "FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("App.Data.Entities.Branch", b =>
                {
                    b.Navigation("Commits");
                });

            modelBuilder.Entity("App.Data.Entities.Commit", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("App.Data.Entities.File", b =>
                {
                    b.Navigation("Metric");
                });

            modelBuilder.Entity("App.Data.Entities.FileDetail", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("App.Data.Entities.Repository", b =>
                {
                    b.Navigation("Commits");
                });
#pragma warning restore 612, 618
        }
    }
}
