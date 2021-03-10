﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SmartEcoA.Models;

namespace SmartEcoA.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210310161205_CarPostDataAutoTest_20210310_00")]
    partial class CarPostDataAutoTest_20210310_00
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SmartEcoA.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("SmartEcoA.Models.CarModelAutoTest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CarPostId")
                        .HasColumnType("integer");

                    b.Property<decimal?>("DEL_MAX")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("DEL_MIN")
                        .HasColumnType("numeric");

                    b.Property<decimal>("EngineType")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("K_MAX")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("K_SVOB")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("L_MAX")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("L_MIN")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_CH")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_CO")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_TAH")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_CH")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_CO")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_TAH")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CarPostId");

                    b.ToTable("CarModelAutoTest");
                });

            modelBuilder.Entity("SmartEcoA.Models.CarModelSmokeMeter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Boost")
                        .HasColumnType("boolean");

                    b.Property<int>("CarPostId")
                        .HasColumnType("integer");

                    b.Property<decimal?>("DFreeMark")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("DMaxMark")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CarPostId");

                    b.ToTable("CarModelSmokeMeter");
                });

            modelBuilder.Entity("SmartEcoA.Models.CarPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Latitude")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CarPost");
                });

            modelBuilder.Entity("SmartEcoA.Models.CarPostDataAutoTest", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CarModelAutoTestId")
                        .HasColumnType("integer");

                    b.Property<string>("DOPOL1")
                        .HasColumnType("text");

                    b.Property<string>("DOPOL2")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal?>("K_1")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("K_2")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("K_3")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("K_4")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("K_MAX")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("K_SVOB")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_CH")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_CO")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_CO2")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_L")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_NO")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_O2")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MAX_TAH")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_CH")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_CO")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_CO2")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_L")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_NO")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_O2")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MIN_TAH")
                        .HasColumnType("numeric");

                    b.Property<string>("Number")
                        .HasColumnType("text");

                    b.Property<decimal?>("ZAV_NOMER")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("CarModelAutoTestId");

                    b.ToTable("CarPostDataAutoTest");
                });

            modelBuilder.Entity("SmartEcoA.Models.CarPostDataSmokeMeter", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CarModelSmokeMeterId")
                        .HasColumnType("integer");

                    b.Property<decimal>("DFree")
                        .HasColumnType("numeric");

                    b.Property<decimal>("DMax")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("NDFree")
                        .HasColumnType("numeric");

                    b.Property<decimal>("NDMax")
                        .HasColumnType("numeric");

                    b.Property<string>("Number")
                        .HasColumnType("text");

                    b.Property<bool>("RunIn")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CarModelSmokeMeterId");

                    b.ToTable("CarPostDataSmokeMeter");
                });

            modelBuilder.Entity("SmartEcoA.Models.DataProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataProvider");
                });

            modelBuilder.Entity("SmartEcoA.Models.MeasuredParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("KazhydrometCode")
                        .HasColumnType("text");

                    b.Property<decimal?>("MPCDailyAverage")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MPCMaxOneTime")
                        .HasColumnType("numeric");

                    b.Property<string>("NameEN")
                        .HasColumnType("text");

                    b.Property<string>("NameKK")
                        .HasColumnType("text");

                    b.Property<string>("NameRU")
                        .HasColumnType("text");

                    b.Property<string>("OceanusCode")
                        .HasColumnType("text");

                    b.Property<decimal>("OceanusCoefficient")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("MeasuredParameter");
                });

            modelBuilder.Entity("SmartEcoA.Models.PollutionEnvironment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("NameEN")
                        .HasColumnType("text");

                    b.Property<string>("NameKK")
                        .HasColumnType("text");

                    b.Property<string>("NameRU")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PollutionEnvironment");
                });

            modelBuilder.Entity("SmartEcoA.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Automatic")
                        .HasColumnType("boolean");

                    b.Property<int>("DataProviderId")
                        .HasColumnType("integer");

                    b.Property<string>("Information")
                        .HasColumnType("text");

                    b.Property<int?>("KazhydrometID")
                        .HasColumnType("integer");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("numeric");

                    b.Property<string>("MN")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<int>("PollutionEnvironmentId")
                        .HasColumnType("integer");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DataProviderId");

                    b.HasIndex("PollutionEnvironmentId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("SmartEcoA.Models.PostData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Data")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("IP")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PostData");
                });

            modelBuilder.Entity("SmartEcoA.Models.PostDataAvg", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MeasuredParameterId")
                        .HasColumnType("integer");

                    b.Property<int>("PostId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("MeasuredParameterId");

                    b.HasIndex("PostId");

                    b.ToTable("PostDataAvg");
                });

            modelBuilder.Entity("SmartEcoA.Models.PostDataDivided", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("MN")
                        .HasColumnType("text");

                    b.Property<string>("OceanusCode")
                        .HasColumnType("text");

                    b.Property<long>("PostDataId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("PostDataId");

                    b.ToTable("PostDataDivided");
                });

            modelBuilder.Entity("SmartEcoA.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("SmartEcoA.Models.Stat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Stat");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SmartEcoA.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SmartEcoA.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartEcoA.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SmartEcoA.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartEcoA.Models.CarModelAutoTest", b =>
                {
                    b.HasOne("SmartEcoA.Models.CarPost", "CarPost")
                        .WithMany()
                        .HasForeignKey("CarPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartEcoA.Models.CarModelSmokeMeter", b =>
                {
                    b.HasOne("SmartEcoA.Models.CarPost", "CarPost")
                        .WithMany()
                        .HasForeignKey("CarPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartEcoA.Models.CarPostDataAutoTest", b =>
                {
                    b.HasOne("SmartEcoA.Models.CarModelAutoTest", "CarModelAutoTest")
                        .WithMany()
                        .HasForeignKey("CarModelAutoTestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartEcoA.Models.CarPostDataSmokeMeter", b =>
                {
                    b.HasOne("SmartEcoA.Models.CarModelSmokeMeter", "CarModelSmokeMeter")
                        .WithMany()
                        .HasForeignKey("CarModelSmokeMeterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartEcoA.Models.Post", b =>
                {
                    b.HasOne("SmartEcoA.Models.DataProvider", "DataProvider")
                        .WithMany()
                        .HasForeignKey("DataProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartEcoA.Models.PollutionEnvironment", "PollutionEnvironment")
                        .WithMany()
                        .HasForeignKey("PollutionEnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartEcoA.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("SmartEcoA.Models.PostDataAvg", b =>
                {
                    b.HasOne("SmartEcoA.Models.MeasuredParameter", "MeasuredParameter")
                        .WithMany()
                        .HasForeignKey("MeasuredParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartEcoA.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartEcoA.Models.PostDataDivided", b =>
                {
                    b.HasOne("SmartEcoA.Models.PostData", "PostData")
                        .WithMany()
                        .HasForeignKey("PostDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
