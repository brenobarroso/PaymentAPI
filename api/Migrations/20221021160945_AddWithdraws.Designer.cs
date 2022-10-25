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
    [Migration("20221021160945_AddWithdraws")]
    partial class AddWithdraws
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("api.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Agency")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric");

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HolderName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

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

            modelBuilder.Entity("api.Models.Withdraws.Withdraw", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DisapprovalDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Withdraws");
                });

            modelBuilder.Entity("PaymentAPI.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

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

                    b.HasIndex("AccountId");

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

            modelBuilder.Entity("api.Models.Withdraws.Withdraw", b =>
                {
                    b.HasOne("api.Models.Account", "Account")
                        .WithMany("Withdraws")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("PaymentAPI.Models.Payment", b =>
                {
                    b.HasOne("api.Models.Account", "Account")
                        .WithMany("Payments")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("api.Models.Account", b =>
                {
                    b.Navigation("Payments");

                    b.Navigation("Withdraws");
                });

            modelBuilder.Entity("PaymentAPI.Models.Payment", b =>
                {
                    b.Navigation("Installments");
                });
#pragma warning restore 612, 618
        }
    }
}
