namespace HW6.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.ProductReview")]
    public partial class ProductReview
    {
        public int ProductReviewID { get; set; }

        public int ProductID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name must be entered, and less than 50 characters")]
        public string ReviewerName { get; set; }

        public DateTime ReviewDate { get; set; }

        [Required]
        [StringLength(50, ErrorMessage ="Email must be entered, and less than 50 characters")]
        public string EmailAddress { get; set; }

        [Required]
        [Display (Name = "Rating (out of 5)")]
        [Range(0,5, ErrorMessage ="Rating must be entered, and between 0-5")]
        public int Rating { get; set; }

        [StringLength(3850, ErrorMessage ="Comment must be less than 3850 characters")]
        public string Comments { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Product Product { get; set; }
    }
}
