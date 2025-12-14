using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LernApp.Data.Migrations
{
    [DbContext(typeof(LernAppDbContext))]
    partial class LernAppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("LernApp.Models.GenerierteCSV", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("UserId");
                b.Property<int?>("PromptId");
                b.Property<string>("Dateiname").IsRequired();
                b.Property<string>("Inhalt").IsRequired();
                b.Property<DateTime>("ErstelltAm");
                b.Property<string>("Beschreibung");
                b.HasKey("Id");
                b.HasIndex("PromptId");
                b.HasIndex("UserId");
                b.ToTable("GenerierteCSVs");
            });

            modelBuilder.Entity("LernApp.Models.LernEinheit", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("UserId");
                b.Property<string>("Fach").IsRequired();
                b.Property<string>("Thema").IsRequired();
                b.Property<string>("Beschreibung");
                b.Property<DateTime>("Datum");
                b.Property<DateTime>("ErstelltAm");
                b.Property<DateTime>("AktualisiertAm");
                b.HasKey("Id");
                b.HasIndex("UserId");
                b.ToTable("LernEinheiten");
            });

            modelBuilder.Entity("LernApp.Models.DateiAnalyse", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("LernEinheitId");
                b.Property<string>("Dateiname").IsRequired();
                b.Property<string>("InhaltZusammenfassung").IsRequired();
                b.Property<DateTime>("AnalysiertAm");
                b.Property<string>("DateityP");
                b.HasKey("Id");
                b.HasIndex("LernEinheitId");
                b.ToTable("DateiAnalysen");
            });

            modelBuilder.Entity("LernApp.Models.Prompt", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("UserId");
                b.Property<string>("Text").IsRequired();
                b.Property<string>("Response").IsRequired();
                b.Property<DateTime>("ErstelltAm");
                b.Property<string>("Kategorie");
                b.HasKey("Id");
                b.HasIndex("UserId");
                b.ToTable("Prompts");
            });

            modelBuilder.Entity("LernApp.Models.UserEinstellung", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("UserId");
                b.Property<string>("Sprache").IsRequired();
                b.Property<string>("Thema").IsRequired();
                b.Property<bool>("BenachrichtigungenAktiv");
                b.Property<string>("AIModell");
                b.Property<DateTime>("AktualisiertAm");
                b.HasKey("Id");
                b.HasIndex("UserId");
                b.ToTable("UserEinstellungen");
            });

            modelBuilder.Entity("LernApp.Models.User", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("Name").IsRequired();
                b.Property<string>("Email").IsRequired();
                b.Property<string>("PasswordHash").IsRequired();
                b.Property<int>("LernzeitProTag");
                b.Property<DateTime>("ErstelltAm");
                b.Property<DateTime>("AktualisiertAm");
                b.HasKey("Id");
                b.ToTable("Users");
            });
        }
    }
}
