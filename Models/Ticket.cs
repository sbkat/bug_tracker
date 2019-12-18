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
        [Display (Name="Project: ")]
        [Required (ErrorMessage = "Project is required.")]
        public string Title { get; set; }
        [Display (Name="Task: ")]
        [Required (ErrorMessage = "Task is required.")]
        public string Task { get; set; }
        [Display (Name="Priority: ")]
        [Required (ErrorMessage = "Priority is required.")]
        public string Priority { get; set; }
        [Display (Name="Deadline: ")]
        [DataType(DataType.Date)]
        [Required (ErrorMessage = "Deadline is required.")]
        public DateTime Deadline { get; set; }
        [Display (Name="Status: ")]
        [Required (ErrorMessage = "Status is required.")]
        public string Status { get; set; }
        [Display (Name="Assigned to: ")]
        public int UserId { get; set; }
        public User Assignment { get; set; }
    }
}