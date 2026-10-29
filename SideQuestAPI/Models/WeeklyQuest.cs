
using System;
using System.ComponentModel.DataAnnotations;

namespace SideQuestAPI.Models
{
    public class WeeklyQuest
    {
        [Key]
        public int DatabaseId { get; set; }

        [Required]
        public string QuestId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public int XpReward { get; set; }

        public string PhotoCriteria { get; set; } = string.Empty;

        [Required]
        public DateTime WeekStartUtc { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
    