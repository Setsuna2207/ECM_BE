using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ECM_BE.Models.DTOs.FileConversion;
using ECM_BE.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace ECM_BE.Services
{
    public class FileConversionService : IFileConversionService
    {
        public async Task<ConversionResultDTO> ConvertDocxToJsonAsync(IFormFile file, string fileType)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new ConversionResultDTO
                    {
                        Success = false,
                        Message = "No file uploaded"
                    };
                }

                // Read DOCX content
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var content = ExtractTextFromDocx(stream);

                // Parse based on file type
                if (fileType.ToLower() == "quiz")
                {
                    return await ParseQuizContentAsync(content);
                }
                else if (fileType.ToLower() == "test")
                {
                    return await ParseTestContentAsync(content);
                }
                else
                {
                    return new ConversionResultDTO
                    {
                        Success = false,
                        Message = "Invalid file type. Use 'quiz' or 'test'"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ConversionResultDTO
                {
                    Success = false,
                    Message = $"Error converting DOCX: {ex.Message}"
                };
            }
        }

        public async Task<ConversionResultDTO> ConvertPdfToJsonAsync(IFormFile file, string fileType)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new ConversionResultDTO
                    {
                        Success = false,
                        Message = "No file uploaded"
                    };
                }

                // Read PDF content
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var content = ExtractTextFromPdf(stream);

                // Parse based on file type
                if (fileType.ToLower() == "quiz")
                {
                    return await ParseQuizContentAsync(content);
                }
                else if (fileType.ToLower() == "test")
                {
                    return await ParseTestContentAsync(content);
                }
                else
                {
                    return new ConversionResultDTO
                    {
                        Success = false,
                        Message = "Invalid file type. Use 'quiz' or 'test'"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ConversionResultDTO
                {
                    Success = false,
                    Message = $"Error converting PDF: {ex.Message}"
                };
            }
        }

        private string ExtractTextFromDocx(Stream stream)
        {
            var text = new StringBuilder();

            using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, false))
            {
                var body = doc.MainDocumentPart?.Document?.Body;
                if (body != null)
                {
                    foreach (var paragraph in body.Descendants<Paragraph>())
                    {
                        text.AppendLine(paragraph.InnerText);
                    }
                }
            }

            return text.ToString();
        }

        private string ExtractTextFromPdf(Stream stream)
        {
            var text = new StringBuilder();

            using (var reader = new iTextSharp.text.pdf.PdfReader(stream))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    text.Append(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page));
                }
            }

            return text.ToString();
        }

        public async Task<ConversionResultDTO> ParseQuizContentAsync(string content)
        {
            try
            {
                var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var questions = new List<ConvertedQuestionDTO>();

                int questionId = 1;
                ConvertedQuestionDTO? currentQuestion = null;

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    // Detect question start (e.g., "Question 1:", "1.", "Q1:")
                    if (Regex.IsMatch(trimmedLine, @"^(Question\s*\d+|Q\d+|\d+\.)", RegexOptions.IgnoreCase))
                    {
                        if (currentQuestion != null)
                        {
                            questions.Add(currentQuestion);
                        }

                        currentQuestion = new ConvertedQuestionDTO
                        {
                            QuestionId = questionId++,
                            Type = "multiple-choice",
                            Options = new List<string>(),
                            Points = 1
                        };

                        // Extract question text
                        var questionText = Regex.Replace(trimmedLine, @"^(Question\s*\d+|Q\d+|\d+\.)\s*", "", RegexOptions.IgnoreCase);
                        currentQuestion.Question = questionText;
                    }
                    // Detect options (e.g., "A)", "a.", "(A)", "A.")
                    else if (currentQuestion != null && Regex.IsMatch(trimmedLine, @"^[A-Da-d][\).\]]"))
                    {
                        var optionText = Regex.Replace(trimmedLine, @"^[A-Da-d][\).\]]\s*", "");
                        currentQuestion.Options?.Add(optionText);
                    }
                    // Detect correct answer (e.g., "Answer: B", "Correct: B")
                    else if (currentQuestion != null && Regex.IsMatch(trimmedLine, @"^(Answer|Correct):", RegexOptions.IgnoreCase))
                    {
                        var answerMatch = Regex.Match(trimmedLine, @"[A-Da-d]", RegexOptions.IgnoreCase);
                        if (answerMatch.Success)
                        {
                            var answerLetter = answerMatch.Value.ToUpper();
                            currentQuestion.CorrectAnswer = answerLetter[0] - 'A';
                        }
                    }
                    // Append to current question if not empty
                    else if (currentQuestion != null && !string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        if (currentQuestion.Options?.Count == 0)
                        {
                            currentQuestion.Question += " " + trimmedLine;
                        }
                    }
                }

                // Add last question
                if (currentQuestion != null)
                {
                    questions.Add(currentQuestion);
                }

                var section = new ConvertedSectionDTO
                {
                    SectionId = 1,
                    Title = "Quiz Questions",
                    Description = "Converted quiz questions",
                    Questions = questions
                };

                var testData = new ConvertedTestDTO
                {
                    Title = "Converted Quiz",
                    Description = "Quiz converted from file",
                    Duration = 30,
                    TotalQuestions = questions.Count,
                    Sections = new List<ConvertedSectionDTO> { section }
                };

                var jsonString = JsonConvert.SerializeObject(testData, Formatting.Indented);

                return new ConversionResultDTO
                {
                    Success = true,
                    Message = "Quiz converted successfully",
                    Data = testData,
                    JsonString = jsonString
                };
            }
            catch (Exception ex)
            {
                return new ConversionResultDTO
                {
                    Success = false,
                    Message = $"Error parsing quiz content: {ex.Message}"
                };
            }
        }

        public async Task<ConversionResultDTO> ParseTestContentAsync(string content)
        {
            try
            {
                var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var sections = new List<ConvertedSectionDTO>();

                ConvertedSectionDTO? currentSection = null;
                ConvertedQuestionDTO? currentQuestion = null;
                int questionId = 1;
                int sectionId = 1;

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    // Detect section headers (e.g., "Section 1:", "LISTENING COMPREHENSION")
                    if (Regex.IsMatch(trimmedLine, @"^(Section\s*\d+|Part\s*\d+):", RegexOptions.IgnoreCase) ||
                        trimmedLine.Length > 10 && trimmedLine == trimmedLine.ToUpper() && !trimmedLine.Contains("?"))
                    {
                        if (currentSection != null)
                        {
                            if (currentQuestion != null)
                            {
                                currentSection.Questions.Add(currentQuestion);
                                currentQuestion = null;
                            }
                            sections.Add(currentSection);
                        }

                        currentSection = new ConvertedSectionDTO
                        {
                            SectionId = sectionId++,
                            Title = trimmedLine,
                            Description = "",
                            Questions = new List<ConvertedQuestionDTO>()
                        };
                    }
                    // Detect questions
                    else if (Regex.IsMatch(trimmedLine, @"^(Question\s*\d+|Q\d+|\d+\.)", RegexOptions.IgnoreCase))
                    {
                        if (currentQuestion != null && currentSection != null)
                        {
                            currentSection.Questions.Add(currentQuestion);
                        }

                        currentQuestion = new ConvertedQuestionDTO
                        {
                            QuestionId = questionId++,
                            Type = "multiple-choice",
                            Options = new List<string>(),
                            Points = 2
                        };

                        var questionText = Regex.Replace(trimmedLine, @"^(Question\s*\d+|Q\d+|\d+\.)\s*", "", RegexOptions.IgnoreCase);
                        currentQuestion.Question = questionText;
                    }
                    // Detect options
                    else if (currentQuestion != null && Regex.IsMatch(trimmedLine, @"^[A-Da-d][\).\]]"))
                    {
                        var optionText = Regex.Replace(trimmedLine, @"^[A-Da-d][\).\]]\s*", "");
                        currentQuestion.Options?.Add(optionText);
                    }
                    // Detect answer
                    else if (currentQuestion != null && Regex.IsMatch(trimmedLine, @"^(Answer|Correct):", RegexOptions.IgnoreCase))
                    {
                        var answerMatch = Regex.Match(trimmedLine, @"[A-Da-d]", RegexOptions.IgnoreCase);
                        if (answerMatch.Success)
                        {
                            var answerLetter = answerMatch.Value.ToUpper();
                            currentQuestion.CorrectAnswer = answerLetter[0] - 'A';
                        }
                    }
                }

                // Add last question and section
                if (currentQuestion != null && currentSection != null)
                {
                    currentSection.Questions.Add(currentQuestion);
                }
                if (currentSection != null)
                {
                    sections.Add(currentSection);
                }

                var testData = new ConvertedTestDTO
                {
                    Title = "Converted Test",
                    Description = "Test converted from file",
                    Duration = 90,
                    TotalQuestions = sections.Sum(s => s.Questions.Count),
                    PassingScore = 60,
                    Sections = sections
                };

                var jsonString = JsonConvert.SerializeObject(testData, Formatting.Indented);

                return new ConversionResultDTO
                {
                    Success = true,
                    Message = "Test converted successfully",
                    Data = testData,
                    JsonString = jsonString
                };
            }
            catch (Exception ex)
            {
                return new ConversionResultDTO
                {
                    Success = false,
                    Message = $"Error parsing test content: {ex.Message}"
                };
            }
        }
    }
}
