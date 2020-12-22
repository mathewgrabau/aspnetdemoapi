using System;
using System.ComponentModel.DataAnnotations;

namespace DemoApi.Models
{
    public class BookingForm
    {
        // Nullable so they come in as null, using the validation to mark as required.
        [Required]
        [Display(Name ="startAt", Description = "Booking start time")]
        public DateTimeOffset? StartAt { get; set; }
        [Required]
        [Display(Name = "endAt", Description ="Booking end time")]
        public DateTimeOffset? EndAt { get; set; }
    }
}
