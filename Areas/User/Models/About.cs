using Swipe.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swipe.Areas.User.Models
{
    public class About
    {
        [ForeignKey("ApplicationUser")]
        public string ID { get; set; }

        [MaxLength(1000)]
        [Display(Name = "About Me")]
        public string AboutMe { get; set; }

        [MaxLength(500)]
        [Display(Name = "What I'm Looking For")]
        public string WhatImLookingFor { get; set; }

        [MaxLength(500)]
        [Display(Name = "I Value")]
        public string IValue { get; set; }

        [MaxLength(100)]
        [Display(Name = "Last Show I Binge Watched")]
        public string LastShowBingeWatched { get; set; }

        [MaxLength(100)]
        [Display(Name = "I Could Probably Beat You At")]
        public string ICouldProbablyBeatYouAt { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
