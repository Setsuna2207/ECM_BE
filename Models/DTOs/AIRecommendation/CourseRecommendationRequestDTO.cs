using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.AIRecommendation
{
    public class CourseRecommendationRequestDTO
    {
        [Required(ErrorMessage = "LearningPathID is required")]
        public int LearningPathID { get; set; }
        
        [Required(ErrorMessage = "ResultID is required")]
        public int ResultID { get; set; }
    }
}
