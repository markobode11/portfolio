﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210103145007_SecondM")]
    partial class SecondM
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("Domain.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Domain.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("CurrentQuantity")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("OptimalQuantity")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.HasKey("ItemId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Domain.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("JobId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("Domain.JobItem", b =>
                {
                    b.Property<int>("JobItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<int>("QuantityNeeded")
                        .HasColumnType("int");

                    b.HasKey("JobItemId");

                    b.HasIndex("ItemId");

                    b.HasIndex("JobId");

                    b.ToTable("JobItems");
                });

            modelBuilder.Entity("Domain.PerformedJob", b =>
                {
                    b.Property<int>("PerformedJobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PerformDate")
                        .HasColumnType("datetime2");

                    b.HasKey("PerformedJobId");

                    b.HasIndex("JobId");

                    b.ToTable("PerformedJobs");
                });

            modelBuilder.Entity("Domain.Item", b =>
                {
                    b.HasOne("Domain.Category", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Domain.JobItem", b =>
                {
                    b.HasOne("Domain.Item", "Item")
                        .WithMany("JobItems")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Job", "Job")
                        .WithMany("JobItems")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Job");
                });

            modelBuilder.Entity("Domain.PerformedJob", b =>
                {
                    b.HasOne("Domain.Job", "Job")
                        .WithMany("PerformedJobs")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("Domain.Category", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Domain.Item", b =>
                {
                    b.Navigation("JobItems");
                });

            modelBuilder.Entity("Domain.Job", b =>
                {
                    b.Navigation("JobItems");

                    b.Navigation("PerformedJobs");
                });
#pragma warning restore 612, 618
        }
    }
}
