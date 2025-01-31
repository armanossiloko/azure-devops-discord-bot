﻿// <auto-generated />
using Azure.Discord.Bot.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Azure.Discord.Bot.DataAccess.Npgsql.Migrations
{
    [DbContext(typeof(DiscordDbContext))]
    partial class DiscordDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("display_name");

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

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint")
                        .HasColumnName("organization_id");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

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
                    b.HasOne("Azure.Discord.Bot.Models.LinkedServerOrganization", "Organization")
                        .WithMany("Subscriptions")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Azure.Discord.Bot.Models.SubscriptionMetadata", "Filter", b1 =>
                        {
                            b1.Property<string>("SubscriptionId")
                                .HasColumnType("text");

                            b1.Property<string>("ProjectId")
                                .HasColumnType("text")
                                .HasColumnName("project_id");

                            b1.Property<string>("RepositoryName")
                                .HasColumnType("text")
                                .HasColumnName("repository_name");

                            b1.Property<string[]>("ReviewerNames")
                                .HasColumnType("text[]")
                                .HasColumnName("reviewer_names");

                            b1.Property<string>("TargetBranch")
                                .HasColumnType("text")
                                .HasColumnName("target_branch");

                            b1.HasKey("SubscriptionId");

                            b1.ToTable("subscriptions");

                            b1.WithOwner()
                                .HasForeignKey("SubscriptionId");
                        });

                    b.Navigation("Filter");

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("Azure.Discord.Bot.Models.LinkedServerOrganization", b =>
                {
                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("Azure.Discord.Bot.Models.Server", b =>
                {
                    b.Navigation("LinkedOrganizations");
                });
#pragma warning restore 612, 618
        }
    }
}
