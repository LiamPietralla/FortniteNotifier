// <auto-generated />
using System;
using FortniteNotifier.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FortniteNotifier.Shared.Migrations
{
    [DbContext(typeof(FortniteContext))]
    partial class FortniteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FortniteNotifier.Shared.Data.Models.Recipient", b =>
                {
                    b.Property<int>("RecipientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RecipientId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("InsertTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdateTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("RecipientId");

                    b.ToTable("Recipients");
                });

            modelBuilder.Entity("FortniteNotifier.Shared.Data.Models.UnsubscribeRequest", b =>
                {
                    b.Property<int>("UnsubscribeRequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UnsubscribeRequestId"));

                    b.Property<DateTime?>("CompleteTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Completed")
                        .HasColumnType("boolean");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("InsertTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("RecipientId")
                        .HasColumnType("integer");

                    b.Property<Guid>("UnsubscribeRequestUrlId")
                        .HasColumnType("uuid");

                    b.HasKey("UnsubscribeRequestId");

                    b.HasIndex("RecipientId");

                    b.ToTable("UnsubscribeRequests");
                });

            modelBuilder.Entity("FortniteNotifier.Shared.Data.Models.VersionRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("InsertTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("UpdateTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("VersionStatus")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("VersionRecords");
                });

            modelBuilder.Entity("FortniteNotifier.Shared.Data.Models.UnsubscribeRequest", b =>
                {
                    b.HasOne("FortniteNotifier.Shared.Data.Models.Recipient", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipient");
                });
#pragma warning restore 612, 618
        }
    }
}
