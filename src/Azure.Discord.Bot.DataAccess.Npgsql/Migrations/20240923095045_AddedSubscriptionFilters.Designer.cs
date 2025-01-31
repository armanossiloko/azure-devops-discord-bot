﻿// <auto-generated />
using Azure.Discord.Bot.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Azure.Discord.Bot.DataAccess.Npgsql.Migrations
{
    [DbContext(typeof(DiscordDbContext))]
    [Migration("20240923095045_AddedSubscriptionFilters")]
    partial class AddedSubscriptionFilters
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Azure.Discord.Bot.Models.LinkedServerOrganization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("OrganizationUrl")
                        .HasColumnType("text")
                        .HasColumnName("organization_url");

                    b.Property<decimal>("ServerId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("server_id");

                    b.Property<string>("Token")
                        .HasColumnType("text")
                        .HasColumnName("token");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("linked_server_organizations");
                });

            modelBuilder.Entity("Azure.Discord.Bot.Models.Server", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("alias");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("key");

                    b.HasKey("Id");

                    b.ToTable("servers");
                });

            modelBuilder.Entity("Azure.Discord.Bot.Models.Subscription", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("event_type");

                    b.HasKey("Id");

                    b.ToTable("subscriptions");
                });

            modelBuilder.Entity("Azure.Discord.Bot.Models.LinkedServerOrganization", b =>
                {
                    b.HasOne("Azure.Discord.Bot.Models.Server", "Server")
                        .WithMany("LinkedOrganizations")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("Azure.Discord.Bot.Models.Subscription", b =>
                {
                    b.OwnsMany("Azure.Discord.Bot.Models.SubscriptionMetadata", "Filters", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<string>("OwnerId")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("RepositoryName")
                                .HasColumnType("text")
                                .HasColumnName("repository_name");

                            b1.Property<string>("ReviewerName")
                                .HasColumnType("text")
                                .HasColumnName("reviewer_name");

                            b1.Property<string>("TargetBranch")
                                .HasColumnType("text")
                                .HasColumnName("target_branch");

                            b1.Property<string>("TeamName")
                                .HasColumnType("text")
                                .HasColumnName("team_name");

                            b1.HasKey("Id");

                            b1.HasIndex("OwnerId");

                            b1.ToTable("SubscriptionMetadata");

                            b1.WithOwner()
                                .HasForeignKey("OwnerId");
                        });

                    b.Navigation("Filters");
                });

            modelBuilder.Entity("Azure.Discord.Bot.Models.Server", b =>
                {
                    b.Navigation("LinkedOrganizations");
                });
#pragma warning restore 612, 618
        }
    }
}
