using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace bug_tracker.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [StringLength(250, MinimumLength = 5, ErrorMessage ="Message must be at least 5 characters (250 max)")]
        public string Content { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public User UserCommented { get; set; }
        public Ticket TicketCommentedOn { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set;} = DateTime.Now;
    }
}