using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Application.Common
{
    public class TaskQueryParameters : QueryParameters
    {
        public ProjectTaskStatus? Status { get; set; }
        public ProjectTaskPriority? Priority { get; set; }
        public Guid? AssignedToId { get; set; }
        public DateTime? DueBefore { get; set; }
        public DateTime? DueAfter { get; set; }
    }
}
