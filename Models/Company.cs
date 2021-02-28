﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZapTrapBugTrack.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<BTUser> Collaborators { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
