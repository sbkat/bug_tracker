using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bug_tracker.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set;} = DateTime.Now;
        public List<Ticket> CreatedTickets { get; set; }
    }
}