using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace BankAccounts.Models 
{
    public class Transaction
    {
        // No other fields!
        [Key]
        public int TransactionId {get;set;}

        [Required]
        [Display(Name = "Deposit/Withdraw:")]
        public decimal Amount {get; set;}
        public int UserId {get; set;}
        public User User {get; set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}