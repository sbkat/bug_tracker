using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bug_tracker.Models
{
    public class TicketViewModel
    {
        [NotMapped]
        public Ticket Ticket { get; set; }
        public List<User> Users { get; set; }
        public int UserId { get; set; }
        public int AdminId { get; set; }
    }
}