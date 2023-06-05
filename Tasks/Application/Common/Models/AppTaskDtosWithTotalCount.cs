namespace Tasks.Application.Common.Models
{
    public class AppTaskDtosWithTotalCount
    {
        public AppTaskDtosWithTotalCount(int count, List<AppTaskDto> AppTaskDtoList)
        {
            Count = count;
            Result = AppTaskDtoList ?? throw new ArgumentNullException(nameof(AppTaskDtoList));
        }

        public int Count { get; set; }
        public List<AppTaskDto> Result { get; set; }
    }
}
