using ECM_BE.Models.DTOs.History;
using ECM_BE.Models.Entities;

namespace ECM_BE.Models.Mapper
{
    public static class HistoryMapper
    {
        public static HistoryDTO ToHistoryDTO(this History history)
        {
            return new HistoryDTO
            {
                HistoryID = history.HistoryID,
                UserID = history.userID,
                CourseID = history.CourseID,
                LastAccessed = history.LastAccessed
            };
        }
    }
}
