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
        public List<Ticket> CreatedTickets { get; set; }
    }
}