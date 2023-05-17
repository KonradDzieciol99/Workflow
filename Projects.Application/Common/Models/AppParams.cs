using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Projects.Application.Common.Models;

public class AppParams
{
    public int? Skip { get; set; }
    public int Take { get; set; }
    public string? OrderBy { get; set; }
    public bool? IsDescending { get; set; }
    public string? Filter { get; set; }
    public string? GroupBy { get; set; }
    public string? Search { get; set; }
    public string[]? SelectedColumns { get; set; }
}
