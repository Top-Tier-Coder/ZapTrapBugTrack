using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZapTrapBugTrack.Models
{
    public class Ticket
    {
        public Ticket()
        {
            Comments = new HashSet<TicketComment>();
            Attachments = new HashSet<TicketAttachment>();
            Notifications = new HashSet<Notification>();
            Histories = new HashSet<TicketHistory>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Subject")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Issue Description")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Submitted")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTimeOffset? Updated { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Edited")]
        public DateTimeOffset? Edited { get; set; }

        [Display(Name = "Project Id")]
        public int ProjectId { get; set; }

        [Display(Name = "Ticket Type Id")]
        public int TicketTypeId { get; set; }

        [Display(Name = "Ticket Priority Id")]
        public int TicketPriorityId { get; set; }

        [Display(Name = "Ticket Status Id")]
        public int TicketStatusId { get; set; }

        [Display(Name = "Submitter Id")]
        public string OwnerUserId { get; set; }

        [Display(Name = "Developer Id")]
        public string DeveloperUserId { get; set; }

        [Display(Name = "Project Name")]
        public Project Project { get; set; }

        [Display(Name = "Ticket Type")]
        public TicketType TicketType { get; set; }

        [Display(Name = "Ticket Priority")]
        public TicketPriority TicketPriority { get; set; }

        [Display(Name = "Ticket Status")]
        public TicketStatus TicketStatus { get; set; }

        [Display(Name = "Submitter Name")]
        public BTUser OwnerUser { get; set; }

        [Display(Name = "Developer Name")]
        public BTUser DeveloperUser { get; set; }


        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<TicketAttachment> Attachments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<TicketHistory> Histories { get; set; }

    }

}
