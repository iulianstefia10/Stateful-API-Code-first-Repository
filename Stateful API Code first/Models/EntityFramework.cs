using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIfilms.Models.EntityFramework
{
    [Table("t_e_film_flm")]
    public class Film
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("flm_id")]
        public int FilmId { get; set; }

        [Required]
        [Column("flm_titre")]
        [StringLength(100, ErrorMessage = "Le titre ne doit pas dépasser 100 caractères.")]
        public string Title { get; set; } = null!;

        [Column("flm_resume", TypeName = "text")]
        public string? Summary { get; set; }

        [Column("flm_datesortie", TypeName = "date")]
        public DateTime? DateRelease { get; set; }

        [Column("flm_duree", TypeName = "numeric(3,0)")]
        public decimal? Duration { get; set; }

        [Column("flm_genre")]
        [StringLength(30, ErrorMessage = "Le genre ne doit pas dépasser 30 caractères.")]
        public string? Gender { get; set; }

       
        public virtual ICollection<Rating> NotesFilm { get; set; } = new List<Rating>();
    }
    [Table("t_j_notation_not")]
    public class Rating
    {
        [Key]
        [Column("utl_id", Order = 0)]
        public int UserId { get; set; }

        [Key]
        [Column("flm_id", Order = 1)]
        public int FilmId { get; set; }

        [Required]
        [Range(0, 5, ErrorMessage = "La note doit être comprise entre 0 et 5.")]
        [Column("not_note")]
        public int Note { get; set; }

        [ForeignKey("UserId")]
        public virtual User UserNotator { get; set; } = null!;

        [ForeignKey("FilmId")]
        public virtual Film MovieNote { get; set; } = null!;
    }

    [Table("t_e_utilisateur_utl")]
    public class User
    {
        

        [Column("utl_pwd")]
        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [StringLength(64)]
        public string Pwd { get; set; } = null!;

        [Column("utl_rue")]
        [StringLength(200)]
        public string? Street { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("utl_id")]
        public int UserId { get; set; }

        [Column("utl_nom")]
        [StringLength(50)]
        
        public string? Name { get; set; }

        [Column("utl_prenom")]
        [StringLength(50)]
        
        public string? FirstName { get; set; }

        [Column("utl_ville")]
        [StringLength(50)]
        
        public string? City { get; set; }

        [Column("utl_mobile",TypeName="char(10)")]
        public string? Mobile { get; set; }

        [Column("utl_cp", TypeName="char(5)")]
        public string? PostalCode { get; set; }

        [Column("utl_mail")]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        [StringLength(100, MinimumLength = 6)]
        [Required(ErrorMessage = "L'adresse e-mail est requise.")]

        public string Mail { get; set; } = null!;

        [Column("utl_pays")]
        [StringLength(50)]
        public string? Country { get; set; }

        [Column("utl_latitude")]
        public float? Latitude { get; set; }

        [Column("utl_longitude")]
        public float? Longitude { get; set; }

        [Column("utl_datecreation", TypeName = "date")]
        [Required(ErrorMessage = "La date de création est requise.")]
        public DateTime CreationDate { get; set; }

        public virtual ICollection<Rating> UserRatings { get; set; } = new List<Rating>();


    }

}
