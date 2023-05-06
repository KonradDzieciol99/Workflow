using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Tasks.Models
{
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

        //public static async ValueTask<AppParams?> BindAsync(HttpContext context,
        //                                               ParameterInfo parameter)
        //{

        //    //var result = new AppParams
        //    //{
        //    //    Skip = context.Request.Query.ContainsKey("Skip") ? int.Parse(context.Request.Query["Skip"]) : (int?)null,
        //    //    Take = context.Request.Query.ContainsKey("Take") ? int.Parse(context.Request.Query["Take"]) : 0,
        //    //    OrderBy = context.Request.Query.ContainsKey("OrderBy") ? context.Request.Query["OrderBy"].ToString() : null,
        //    //    IsDescending = context.Request.Query.ContainsKey("IsDescending") ? bool.Parse(context.Request.Query["IsDescending"]) : (bool?)null,
        //    //    Filter = context.Request.Query.ContainsKey("Filter") ? context.Request.Query["Filter"].ToString() : null,
        //    //    GroupBy = context.Request.Query.ContainsKey("GroupBy") ? context.Request.Query["GroupBy"].ToString() : null,
        //    //    Search = context.Request.Query.ContainsKey("Search") ? context.Request.Query["Search"].ToString() : null,
        //    //    SelectedColumns = context.Request.Query.ContainsKey("SelectedColumns") ? context.Request.Query["SelectedColumns"].ToString().Split(',') : null
        //    //};
        //    var result = new AppParams
        //    {
        //        Skip = context.Request.Query.ContainsKey("Skip") && int.TryParse(context.Request.Query["Skip"], out var skip) ? skip : (int?)null,
        //        Take = context.Request.Query.ContainsKey("Take") && int.TryParse(context.Request.Query["Take"], out var take) ? take : 0,
        //        OrderBy = context.Request.Query.ContainsKey("OrderBy") ? context.Request.Query["OrderBy"].ToString() : null,
        //        IsDescending = context.Request.Query.ContainsKey("IsDescending") && bool.TryParse(context.Request.Query["IsDescending"], out var isDescending) ? isDescending : (bool?)null,
        //        Filter = context.Request.Query.ContainsKey("Filter") ? context.Request.Query["Filter"].ToString() : null,
        //        GroupBy = context.Request.Query.ContainsKey("GroupBy") ? context.Request.Query["GroupBy"].ToString() : null,
        //        Search = context.Request.Query.ContainsKey("Search") ? context.Request.Query["Search"].ToString() : null,
        //        SelectedColumns = context.Request.Query.ContainsKey("SelectedColumns") ? context.Request.Query["SelectedColumns"].ToString().Split(',') : null
        //    };

        //    //new ValidationException()

        //    return await ValueTask.FromResult<AppParams?>(result);
        //}
    }
}
