﻿// <auto-generated />
using System;
using EfCoreRelationshipsDemo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EfCoreRelationshipsDemo.Migrations
{
    [DbContext(typeof(SampleDbContext))]
    [Migration("20221228010308_AddInvoiceItem")]
    partial class AddInvoiceItem
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EfCoreRelationshipsDemo.Models.Invoice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<decimal>("Amount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("Amount");

                    b.Property<string>("ContactName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)")
                        .HasColumnName("ContactName");

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("Description");

                    b.Property<DateTimeOffset>("DueDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("DueDate");

                    b.Property<DateTimeOffset>("InvoiceDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("InvoiceDate");

                    b.Property<string>("InvoiceNumber")
                        .IsRequired()
                        .HasColumnType("varchar(32)")
                        .HasColumnName("InvoiceNumber");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)")
                        .HasColumnName("Status");

                    b.HasKey("Id");

                    b.HasIndex("InvoiceNumber")
                        .IsUnique();

                    b.ToTable("Invoices", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("31bf42f3-c8c8-43d6-8b8e-6445f471e5a6"),
                            Amount = 100m,
                            ContactName = "Iron Man",
                            Description = "Invoice for the first month",
                            DueDate = new DateTimeOffset(new DateTime(2021, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            InvoiceDate = new DateTimeOffset(new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            InvoiceNumber = "INV-001",
                            Status = "AwaitPayment"
                        },
                        new
                        {
                            Id = new Guid("2622c982-eaad-410d-bc5e-1d4811323c01"),
                            Amount = 200m,
                            ContactName = "Captain America",
                            Description = "Invoice for the first month",
                            DueDate = new DateTimeOffset(new DateTime(2021, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            InvoiceDate = new DateTimeOffset(new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            InvoiceNumber = "INV-002",
                            Status = "AwaitPayment"
                        },
                        new
                        {
                            Id = new Guid("4e017bd5-14c8-4b7e-bc94-d1fee33cbca1"),
                            Amount = 300m,
                            ContactName = "Thor",
                            Description = "Invoice for the first month",
                            DueDate = new DateTimeOffset(new DateTime(2021, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            InvoiceDate = new DateTimeOffset(new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            InvoiceNumber = "INV-003",
                            Status = "Draft"
                        });
                });

            modelBuilder.Entity("EfCoreRelationshipsDemo.Models.InvoiceItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<decimal>("Amount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("Amount");

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("Description");

                    b.Property<Guid>("InvoiceId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("InvoiceId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("Name");

                    b.Property<decimal>("Quantity")
                        .HasPrecision(8, 2)
                        .HasColumnType("decimal(8,2)")
                        .HasColumnName("Quantity");

                    b.Property<decimal>("UnitPrice")
                        .HasPrecision(8, 2)
                        .HasColumnType("decimal(8,2)")
                        .HasColumnName("UnitPrice");

                    b.HasKey("Id");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceItems", (string)null);
                });

            modelBuilder.Entity("EfCoreRelationshipsDemo.Models.InvoiceItem", b =>
                {
                    b.HasOne("EfCoreRelationshipsDemo.Models.Invoice", "Invoice")
                        .WithMany("InvoiceItems")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("EfCoreRelationshipsDemo.Models.Invoice", b =>
                {
                    b.Navigation("InvoiceItems");
                });
#pragma warning restore 612, 618
        }
    }
}
