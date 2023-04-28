using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Projects.Models
{
    public class AppParams
    {
        public int? Skip { get; set; }
        public int Take { get; set; } = 10;
        public string? OrderBy { get; set; } = null;
        public bool? IsDescending { get; set; } = null;
        public string? Filter { get; set; } = null;
        public string? GroupBy { get; set; } = null;
        public string? Search { get; set; } = null;
        public string[]? SelectedColumns { get; set; } = null;
    }
}
