using System.Collections.Generic;
using System.Linq;
using Business.Models; // For ProjectListItem, Status

namespace Presentation.ViewModels;

// Helper class to hold status info and count for the filter bar
public class StatusFilterInfo
{
    public string StatusId { get; set; } = string.Empty; // Keep as string to match entity
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}

// ViewModel for the Projects Index page
public class ProjectIndexViewModel
{
    // The list of projects currently displayed (could be filtered later)
    public IEnumerable<ProjectListItem> Projects { get; set; } =
        Enumerable.Empty<ProjectListItem>();

    // Information for the status filter tabs
    public List<StatusFilterInfo> StatusFilters { get; set; } = new List<StatusFilterInfo>();

    // Total count for the "ALL" tab
    public int TotalProjectCount { get; set; }
}
