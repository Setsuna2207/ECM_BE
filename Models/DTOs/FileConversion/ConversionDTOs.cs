using System.Collections.Generic;

namespace ECM_BE.Models.DTOs.FileConversion
{
    public class ConvertFileRequestDTO
    {
        public IFormFile File { get; set; }
        public string FileType { get; set; } // "quiz" or "test"
    }

    public class ConvertedQuestionDTO
    {
        public int QuestionId { get; set; }
        public string Type { get; set; } // multiple-choice, essay, short-response, etc.
        public string Question { get; set; }
        public string? Passage { get; set; }
        public List<string>? Options { get; set; }
        public int? CorrectAnswer { get; set; } // index for multiple choice
        public string? CorrectAnswerText { get; set; } // text for other types
        public int Points { get; set; }
        public int? MinWords { get; set; }
        public int? MaxWords { get; set; }
    }

    public class ConvertedSectionDTO
    {
        public int SectionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? MediaUrl { get; set; }
        public int? Duration { get; set; }
        public List<ConvertedQuestionDTO> Questions { get; set; }
    }

    public class ConvertedTestDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int TotalQuestions { get; set; }
        public int? PassingScore { get; set; }
        public List<ConvertedSectionDTO> Sections { get; set; }
    }

    public class ConversionResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ConvertedTestDTO? Data { get; set; }
        public string? JsonString { get; set; }
    }
}
