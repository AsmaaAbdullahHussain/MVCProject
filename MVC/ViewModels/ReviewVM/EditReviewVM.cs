﻿using System.ComponentModel.DataAnnotations;

namespace mvc.ViewModels.ReviewVM
{
    public class EditReviewVM
    {
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Range(0,5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
