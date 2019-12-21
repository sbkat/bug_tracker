using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace bug_tracker.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }
        [Display (Name="Problem: ")]
        [Required (ErrorMessage = "Problem is required.")]
        public string Title { get; set; }
        [Display (Name="Task: ")]
        [Required (ErrorMessage = "Task is required.")]
        public string Task { get; set; }
        [Display (Name="Priority: ")]
        [Required (ErrorMessage = "Priority is required.")]
        public string Priority { get; set; }
        [Display (Name="Deadline: ")]
        [Required (ErrorMessage = "Deadline is required.")]
        public DateTime Deadline { get; set; }
        [Display (Name="Status: ")]
        [Required (ErrorMessage = "Status is required.")]
        public string Status { get; set; }
        [Display (Name="Assigned to: ")]
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set;} = DateTime.Now;
        public User Assignment { get; set; }
        public Admin Creator { get; set; }
        public List<Comment> Comments { get; set; }
    }
}