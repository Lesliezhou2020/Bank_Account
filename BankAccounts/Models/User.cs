using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccounts.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Display(Name="FirstName: ", Prompt="Enter your First name here")]
        [Required]
        [MinLength(2, ErrorMessage="First name must be 2 characters or longer!")]
        public string FirstName {get;set;}

        [Display(Name="LastName: ", Prompt="Enter your Last name here")]
        [Required]
        [MinLength(2, ErrorMessage="First name must be 2 characters or longer!")]
        public string LastName {get;set;}

        [Display(Name="Email: ")]
        [Required(ErrorMessage="Email address is required.")]
            // [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage="Invalid email address.")]
        public string Email {get;set;}

        [DataType(DataType.Password)]
        [Required]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        // Will not be mapped to your users table!
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
        public List<Transaction> Transactions {get; set;}
    }    
}
