using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZapTrapBugTrack.Models.ViewModel
{
    public class InviteViewModel
    {
        //public BTUser User { get; set; }
        ////Need Email, FirstName, LastName



        //public Company Company { get; set; }
        ////Need Name and Description



        //public Project Project { get; set; }
        ////Need Name and Description


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }


        [Display(Name = "Project Description")]
        public string ProjectDescription { get; set; }


        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }


        [Display(Name = "Company Description")]
        public string CompanyDescription { get; set; }

    }
}
 


