﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestTask_InfTech.DB;

#nullable disable

namespace TestTask_InfTech.Migrations.FileStorage
{
    [DbContext(typeof(FileStorageDB))]
    [Migration("20240929171359_InitFolder")]
    partial class InitFolder
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TestTask_InfTech.DB.Model.Extension", b =>
                {
                    b.Property<Guid>("ExtensionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ExtensionId");

                    b.ToTable("Ext");
                });

            modelBuilder.Entity("TestTask_InfTech.DB.Model.File", b =>
                {
                    b.Property<Guid>("FileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ExtensionId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FolderId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("FileId");

                    b.HasIndex("ExtensionId");

                    b.HasIndex("FolderId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("TestTask_InfTech.DB.Model.Folder", b =>
                {
                    b.Property<Guid>("FolderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentalFolderId")
                        .HasColumnType("uuid");

                    b.HasKey("FolderId");

                    b.HasIndex("ParentalFolderId");

                    b.ToTable("Folder");
                });

            modelBuilder.Entity("TestTask_InfTech.DB.Model.File", b =>
                {
                    b.HasOne("TestTask_InfTech.DB.Model.Extension", "Extension")
                        .WithMany()
                        .HasForeignKey("ExtensionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TestTask_InfTech.DB.Model.Folder", "Folder")
                        .WithMany()
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Extension");

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("TestTask_InfTech.DB.Model.Folder", b =>
                {
                    b.HasOne("TestTask_InfTech.DB.Model.Folder", "Parental")
                        .WithMany()
                        .HasForeignKey("ParentalFolderId");

                    b.Navigation("Parental");
                });
#pragma warning restore 612, 618
        }
    }
}
