using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ECM_BE.Models.DTOs.FileConversion;
using ECM_BE.Models.DTOs.PlacementTest;
using ECM_BE.Models.DTOs.Quiz;
using ECM_BE.Services.Interfaces;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

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
                var questions = new List<QuizQuestionDTO>();

                int questionId = 1;
                QuizQuestionDTO? currentQuestion = null;

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    // Detect question start
                    if (Regex.IsMatch(trimmedLine, @"^(Question\s*\d+|Q\d+|\d+\.)", RegexOptions.IgnoreCase))
                    {
                        if (currentQuestion != null)
                        {
                            questions.Add(currentQuestion);
                        }

                        currentQuestion = new QuizQuestionDTO
                        {
                            QuestionId = questionId++,
                            Options = new List<string>()
                        };

                        var questionText = Regex.Replace(trimmedLine, @"^(Question\s*\d+|Q\d+|\d+\.)\s*", "", RegexOptions.IgnoreCase);
                        currentQuestion.Question = questionText;
                    }
                    // Detect options (A), B), C), D))
                    else if (currentQuestion != null && Regex.IsMatch(trimmedLine, @"^[A-Da-d][\).\]]"))
                    {
                        var optionText = Regex.Replace(trimmedLine, @"^[A-Da-d][\).\]]\s*", "");
                        currentQuestion.Options?.Add(optionText);
                    }
                    // Detect correct answer
                    else if (currentQuestion != null && Regex.IsMatch(trimmedLine, @"^(Answer|Correct):", RegexOptions.IgnoreCase))
                    {
                        var answerMatch = Regex.Match(trimmedLine, @"[A-Da-d]", RegexOptions.IgnoreCase);
                        if (answerMatch.Success)
                        {
                            var answerLetter = answerMatch.Value.ToUpper();
                            currentQuestion.CorrectAnswer = answerLetter[0] - 'A';
                        }
                    }
                    else if (currentQuestion != null && !string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        if (currentQuestion.Options?.Count == 0)
                        {
                            currentQuestion.Question += " " + trimmedLine;
                        }
                    }
                }

                if (currentQuestion != null)
                {
                    questions.Add(currentQuestion);
                }

                // Match Quiz.js format exactly
                var quizData = new
                {
                    questions = questions
                };

                var jsonString = JsonConvert.SerializeObject(quizData, Formatting.Indented);

                return new ConversionResultDTO
                {
                    Success = true,
                    Message = $"Quiz converted successfully ({questions.Count} questions)",
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
                var sections = new List<TestSectionDTO>();

                TestSectionDTO? currentSection = null;
                TestQuestionDTO? currentQuestion = null;
                int questionId = 1;
                int sectionId = 1;
                string? currentPassage = null;

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    // Detect section headers (UPPERCASE or "Section X:")
                    if (Regex.IsMatch(trimmedLine, @"^(Section\s*\d+|Part\s*\d+):", RegexOptions.IgnoreCase) ||
                        (trimmedLine.Length > 10 && trimmedLine == trimmedLine.ToUpper() && !trimmedLine.Contains("?") && !trimmedLine.Contains(".")))
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

                        currentSection = new TestSectionDTO
                        {
                            SectionId = sectionId++,
                            Title = trimmedLine,
                            Description = "",
                            Duration = 15,
                            Questions = new List<TestQuestionDTO>()
                        };
                        currentPassage = null;
                    }
                    // Detect passage/reading text (starts with regular sentence but not a question)
                    else if (currentSection != null && trimmedLine.Length > 50 && !Regex.IsMatch(trimmedLine, @"^(Question|\d+\.|[A-D][\).\]])") && !trimmedLine.EndsWith("?"))
                    {
                        currentPassage = trimmedLine;
                    }
                    // Detect questions
                    else if (Regex.IsMatch(trimmedLine, @"^(Question\s*\d+|Q\d+|\d+\.)", RegexOptions.IgnoreCase))
                    {
                        if (currentQuestion != null && currentSection != null)
                        {
                            currentSection.Questions.Add(currentQuestion);
                        }

                        // Detect question type based on keywords
                        string questionType = "multiple-choice";
                        if (trimmedLine.ToLower().Contains("write") || trimmedLine.ToLower().Contains("essay"))
                        {
                            questionType = "essay";
                        }
                        else if (trimmedLine.ToLower().Contains("complete the sentence"))
                        {
                            questionType = "sentence-completion";
                        }
                        else if (trimmedLine.ToLower().Contains("correct") || trimmedLine.ToLower().Contains("error"))
                        {
                            questionType = "error-correction";
                        }
                        else if (trimmedLine.ToLower().Contains("describe") || trimmedLine.ToLower().Contains("explain"))
                        {
                            questionType = "short-response";
                        }

                        currentQuestion = new TestQuestionDTO
                        {
                            QuestionId = questionId++,
                            Type = questionType,
                            Options = new List<string>(),
                            Points = 2,
                            Passage = currentPassage
                        };

                        var questionText = Regex.Replace(trimmedLine, @"^(Question\s*\d+|Q\d+|\d+\.)\s*", "", RegexOptions.IgnoreCase);
                        currentQuestion.Question = questionText;
                        currentPassage = null; // Reset passage after using it
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
                        else
                        {
                            // For non-multiple-choice, store answer text
                            var answerText = Regex.Replace(trimmedLine, @"^(Answer|Correct):\s*", "", RegexOptions.IgnoreCase);
                            currentQuestion.CorrectAnswerText = answerText;
                        }
                    }
                    // Detect word limits for writing questions
                    else if (currentQuestion != null && Regex.IsMatch(trimmedLine, @"\d+.*words?", RegexOptions.IgnoreCase))
                    {
                        var wordMatches = Regex.Matches(trimmedLine, @"\d+");
                        if (wordMatches.Count >= 2)
                        {
                            currentQuestion.MinWords = int.Parse(wordMatches[0].Value);
                            currentQuestion.MaxWords = int.Parse(wordMatches[1].Value);
                        }
                        else if (wordMatches.Count == 1)
                        {
                            currentQuestion.ExpectedWords = int.Parse(wordMatches[0].Value);
                        }
                    }
                    // Detect points
                    else if (currentQuestion != null && Regex.IsMatch(trimmedLine, @"Points?:\s*\d+", RegexOptions.IgnoreCase))
                    {
                        var pointsMatch = Regex.Match(trimmedLine, @"\d+");
                        if (pointsMatch.Success)
                        {
                            currentQuestion.Points = int.Parse(pointsMatch.Value);
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

                // Match Test.js format exactly
                var testData = new
                {
                    sections = sections
                };

                var jsonString = JsonConvert.SerializeObject(testData, Formatting.Indented);

                return new ConversionResultDTO
                {
                    Success = true,
                    Message = $"Test converted successfully ({sections.Count} sections, {sections.Sum(s => s.Questions.Count)} questions)",
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
