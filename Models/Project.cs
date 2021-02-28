using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ZapTrapBugTrack.Extensions;

namespace ZapTrapBugTrack.Models
{
    public class Project
    {
        public Project()
        {
            Members = new HashSet<BTUser>();
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Select Image")]
        [NotMapped]
        [DataType(DataType.Upload)]
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", "doc", "docx", "xls", "xlsx", "pdf"})]
        public IFormFile ImageFormFile { get; set; }
        public string ImageFileName { get; set; }
        public byte[] ImageFileData { get; set; }
        public int? CompanyId { get; set; }


        public virtual ICollection<BTUser> Members { get; set; }
        //public IList<ProjectUser> ProjectUsers { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual Company Company { get; set; }


    }
}
