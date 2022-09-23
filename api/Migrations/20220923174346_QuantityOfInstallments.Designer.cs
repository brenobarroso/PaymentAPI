﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PaymentAPI.Data;

#nullable disable

namespace PaymentAPI.Migrations
{
    [DbContext(typeof(PaymentDbContext))]
    [Migration("20220923174346_QuantityOfInstallments")]
    partial class QuantityOfInstallments
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("api.Models.Installment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("InstallmentGrossValue")
                        .HasColumnType("real");

                    b.Property<float>("InstallmentNetValue")
                        .HasColumnType("real");

                    b.Property<int>("InstallmentNumber")
                        .HasColumnType("integer");

                    b.Property<int>("PaymentId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ReceiptDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PaymentId");

                    b.ToTable("Installments");
                });

            modelBuilder.Entity("PaymentAPI.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CardNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Confirmation")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("DisapprovalDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float?>("FlatRate")
                        .IsRequired()
                        .HasColumnType("real");

                    b.Property<float>("GrossValue")
                        .HasColumnType("real");

                    b.Property<float?>("NetValue")
                        .HasColumnType("real");

                    b.Property<DateTime>("TransationDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("api.Models.Installment", b =>
                {
                    b.HasOne("PaymentAPI.Models.Payment", "Payment")
                        .WithMany("Installments")
                        .HasForeignKey("PaymentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("PaymentAPI.Models.Payment", b =>
                {
                    b.Navigation("Installments");
                });
#pragma warning restore 612, 618
        }
    }
}
