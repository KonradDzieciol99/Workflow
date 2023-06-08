namespace Tasks.Application.Common.Models
{
    public class AppTaskDtosWithTotalCount
    {
        public AppTaskDtosWithTotalCount(int count, List<AppTaskDto> result)
        {
            Count = count;
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public int Count { get; set; }
        public List<AppTaskDto> Result { get; set; }
    }
}
