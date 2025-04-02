using System.ComponentModel.DataAnnotations;

namespace CareerProject.Models.AccountView
{
    public class Meeting
    {
        [Key]
        public int MeetingId { get; set; }
        [Required]
        public string MentorEmail { get; set; }
        [Required]

        public string StudentEmail { get; set; }
        [Required]
        public DateTime MeetingDate { get; set; }
        



    }
}
