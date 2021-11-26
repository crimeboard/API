using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CrimeBoardContext : DbContext
    {
        public CrimeBoardContext()
        {
        }

        public CrimeBoardContext(DbContextOptions<CrimeBoardContext> options)
            : base(options)
        {
            //this.Set<VwGroupTree>().AsNoTracking();         //View data must not be cached by Entity Framework, must come from DB
            //this.Set<VwLinkTree>().AsNoTracking();
        }

        public virtual DbSet<CbAliasName> CbAliasNames { get; set; }
        public virtual DbSet<CbAudit> CbAudits { get; set; }
        public virtual DbSet<CbGroup> CbGroups { get; set; }
        public virtual DbSet<CbGroupXImage> CbGroupXImages { get; set; }
        public virtual DbSet<CbGroupXRelationship> CbGroupXRelationships { get; set; }
        public virtual DbSet<CbGroupXTransaction> CbGroupXTransactions { get; set; }
        public virtual DbSet<CbIndividual> CbIndividuals { get; set; }
        public virtual DbSet<CbIndividualXRelationship> CbIndividualXRelationships { get; set; }
        public virtual DbSet<CbIndividualXTransaction> CbIndividualXTransactions { get; set; }
        public virtual DbSet<CbRelationship> CbRelationships { get; set; }
        public virtual DbSet<CbResource> CbResources { get; set; }
        public virtual DbSet<CbTransaction> CbTransactions { get; set; }
        public virtual DbSet<CbTransactionXImage> CbTransactionXImages { get; set; }
        public virtual DbSet<CbTrumpAppointment> CbTrumpAppointments { get; set; }
        public virtual DbSet<CbViewAuditUser> CbViewAuditUsers { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<ImageBlob> ImageBlobs { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<CbUser> CbUsers { get; set; }
        public virtual DbSet<VwGroupTree> VwGroupTrees { get; set; }
        public virtual DbSet<VwLinkTree> VwLinkTrees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder
                //.UseLazyLoadingProxies(true)
                .UseSqlServer(configuration.GetConnectionString("DatabaseConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<CbViewAuditUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("CB_View_Audit_User");

                entity.Property(e => e.DateChanged).HasColumnType("datetime");

                entity.Property(e => e.FieldName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.GoogleHandle).HasMaxLength(25);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.NewValue).HasMaxLength(500);

                entity.Property(e => e.OldValue).HasMaxLength(500);

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TwitterHandle).HasMaxLength(25);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<CbUser>(entity =>
            {
                entity.HasKey(e => e.TwitterId)
                    .HasName("PK_CB_User_1");

                entity.ToTable("CB_User");

                entity.Property(e => e.TwitterId)
                    .HasMaxLength(25)
                    .HasColumnName("TwitterID");

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.GoogleHandle).HasMaxLength(25);

                entity.Property(e => e.GuidVerificationCode)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.TwitterHandle).HasMaxLength(25);
            });

            modelBuilder.Entity<CbAliasName>(entity =>
            {
                entity.ToTable("CB_Alias_Name");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.IndividualId).HasColumnName("IndividualID");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(140);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.CbAliasName)
                    .HasForeignKey<CbAliasName>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CB_Alias_Name_CB_Individual");
            });

            modelBuilder.Entity<CbAudit>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.TableName, e.Id, e.FieldName, e.DateChanged })
                    .HasName("PK_CB_Audit_1");

                entity.ToTable("CB_Audit");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.TableName).HasMaxLength(100);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FieldName).HasMaxLength(100);

                entity.Property(e => e.DateChanged).HasColumnType("datetime");

                entity.Property(e => e.OldValue).HasMaxLength(500);

                entity.Property(e => e.NewValue).HasMaxLength(500);
            });


            modelBuilder.Entity<CbGroup>(entity =>
            {
                entity.ToTable("CB_Group");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.WikipediaUrl)
                    .HasMaxLength(150)
                    .HasColumnName("WikipediaURL");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK__Group__Group");
            });

            modelBuilder.Entity<CbGroupXImage>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.ImageId })
                    .HasName("PK__Group_X_Image");

                entity.ToTable("CB_Group_X_Image");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.CbGroupXImages)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Group_X_Image__Group");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.CbGroupXImages)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Group_X_Image_ImageBlob");
            });

            modelBuilder.Entity<CbGroupXRelationship>(entity =>
            {
                entity.HasKey(e => new { e.RelationshipId, e.GroupId })
                    .HasName("PK_Group_X_Relationship");

                entity.ToTable("CB_Group_X_Relationship");

                entity.Property(e => e.RelationshipId).HasColumnName("RelationshipID");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.CbGroupXRelationships)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CB_Group_X_Relationship_CB_Group");

                entity.HasOne(d => d.Relationship)
                    .WithMany(p => p.CbGroupXRelationships)
                    .HasForeignKey(d => d.RelationshipId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CB_Group_X_Relationship_CB_Relationship");
            });

            modelBuilder.Entity<CbGroupXTransaction>(entity =>
            {
                entity.HasKey(e => new { e.TransactionId, e.GroupId })
                    .HasName("PK_Group_X_Transaction");

                entity.ToTable("CB_Group_X_Transaction");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.CbGroupXTransactions)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CB_Group_X_Transaction_CB_Group");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.CbGroupXTransactions)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CB_Group_X_Transaction_CB_Transaction");
            });

            modelBuilder.Entity<CbIndividual>(entity =>
            {
                entity.ToTable("CB_Individual");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CommonName).HasMaxLength(120);

                entity.Property(e => e.CountryOfBirth).HasMaxLength(30);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Firstname).HasMaxLength(70);

                entity.Property(e => e.JobTitle).HasMaxLength(120);

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.Surname).HasMaxLength(70);

                entity.Property(e => e.TwitterHandle).HasMaxLength(70);

                entity.Property(e => e.WikipediaUrl)
                    .HasMaxLength(150)
                    .HasColumnName("WikipediaURL");
            });

            modelBuilder.Entity<CbIndividualXRelationship>(entity =>
            {
                entity.HasKey(e => new { e.RelationshipId, e.IndividualId });

                entity.ToTable("CB_Individual_X_Relationship");

                entity.Property(e => e.RelationshipId).HasColumnName("RelationshipID");

                entity.Property(e => e.IndividualId).HasColumnName("IndividualID");

                entity.HasOne(d => d.Individual)
                    .WithMany(p => p.CbIndividualXRelationships)
                    .HasForeignKey(d => d.IndividualId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CB_Individual_X_Relationship_CB_Individual");

                entity.HasOne(d => d.Relationship)
                    .WithMany(p => p.CbIndividualXRelationships)
                    .HasForeignKey(d => d.RelationshipId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CB_Individual_X_Relationship_CB_Relationship");
            });

            modelBuilder.Entity<CbIndividualXTransaction>(entity =>
            {
                entity.HasKey(e => new { e.TransactionId, e.IndividualId });

                entity.ToTable("CB_Individual_X_Transaction");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.IndividualId).HasColumnName("IndividualID");

                entity.HasOne(d => d.Individual)
                    .WithMany(p => p.CbIndividualXTransactions)
                    .HasForeignKey(d => d.IndividualId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CB_Individual_X_Transaction_CB_Individual");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.CbIndividualXTransactions)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CB_Individual_X_Transaction_CB_Transaction");
            });

            modelBuilder.Entity<CbRelationship>(entity =>
            {
                entity.ToTable("CB_Relationship");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Ended).HasColumnType("date");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.RelationshipTypeId).HasColumnName("RelationshipTypeID");

                entity.Property(e => e.Started).HasColumnType("date");
            });

            modelBuilder.Entity<CbResource>(entity =>
            {
                entity.ToTable("CB_Resource");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(140);

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.Url)
                    .HasMaxLength(250)
                    .HasColumnName("URL");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.CbResources)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK__Resource__Transaction");
            });

            modelBuilder.Entity<CbTransaction>(entity =>
            {
                entity.ToTable("CB_Transaction");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.JuristictionId).HasColumnName("JuristictionID");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.MaxFine)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MinFine)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MonetaryAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NewspaperArticleUrl)
                    .HasMaxLength(200)
                    .HasColumnName("NewspaperArticleURL");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.TwitterPostUrl)
                    .HasMaxLength(200)
                    .HasColumnName("TwitterPostURL");

                entity.Property(e => e.YouTubeUrl)
                    .HasMaxLength(200)
                    .HasColumnName("YouTubeURL");
            });

            modelBuilder.Entity<CbTransactionXImage>(entity =>
            {
                entity.HasKey(e => new { e.TransactionId, e.ImageId })
                    .HasName("PK__Transaction_X_Image");

                entity.ToTable("CB_Transaction_X_Image");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.CbTransactionXImages)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transaction_X_Image_ImageBlob");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.CbTransactionXImages)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transaction_X_Image__Transaction");
            });

            modelBuilder.Entity<CbTrumpAppointment>(entity =>
            {
                entity.HasKey(e => new { e.Position, e.Name })
                    .HasName("PK__TrumpAppointments");

                entity.ToTable("CB_TrumpAppointments");

                entity.Property(e => e.Position)
                    .HasMaxLength(120)
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsFixedLength(true);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DepartureDate).HasColumnType("date");

                entity.Property(e => e.IndividualId).HasColumnName("IndividualID");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.PriorJob)
                    .HasMaxLength(120)
                    .IsFixedLength(true);

                entity.Property(e => e.Reason)
                    .HasMaxLength(100)
                    .IsFixedLength(true);

                entity.Property(e => e.Successor)
                    .HasMaxLength(100)
                    .IsFixedLength(true);

                entity.Property(e => e.WhereTo)
                    .HasMaxLength(120)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Individual)
                    .WithMany(p => p.CbTrumpAppointments)
                    .HasForeignKey(d => d.IndividualId)
                    .HasConstraintName("FK__TrumpAppointments__Individual");
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.ToTable("Folder");

                entity.HasIndex(e => e.ParentFolderId, "IX_Folder_ParentFolderId");

                entity.HasIndex(e => new { e.SiteId, e.FolderName }, "IX_Folder_SiteId_FolderName");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.FolderName).HasMaxLength(70);

                entity.Property(e => e.GroupId).HasDefaultValueSql("((-2))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsRestricted).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.SortOrder).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.ParentFolder)
                    .WithMany(p => p.InverseParentFolder)
                    .HasForeignKey(d => d.ParentFolderId)
                    .HasConstraintName("FK_Folder_Folder");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.Folders)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("FK_Folder_Template");
            });

            modelBuilder.Entity<ImageBlob>(entity =>
            {
                entity.HasKey(e => e.ImageId);

                entity.ToTable("ImageBlob");

                entity.HasIndex(e => e.FolderId, "IX_ImageBlob_FolderId");

                entity.Property(e => e.AltText).HasMaxLength(200);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.FileName).HasMaxLength(150);

                entity.Property(e => e.FolderId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ImageBlob1)
                    .HasColumnType("image")
                    .HasColumnName("ImageBlob");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.ThumbnailBlob).HasColumnType("image");

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.ImageBlobs)
                    .HasForeignKey(d => d.FolderId)
                    .HasConstraintName("FK_ImageBlob_Folder");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.ToTable("Template");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.Layout).HasColumnType("text");

                entity.Property(e => e.TemplateName).HasMaxLength(70);

                entity.Property(e => e.Type).HasMaxLength(20);
            });

            modelBuilder.Entity<VwGroupTree>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VW_Group_Tree");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.Name).HasMaxLength(70);

                entity.Property(e => e.WikipediaUrl).HasMaxLength(150);
            });

            modelBuilder.Entity<VwLinkTree>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VW_Link_Tree");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}