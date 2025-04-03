using System;

namespace Business.Models
{
    public class ProjectListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
        public int MemberCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string GradientCss { get; set; } = string.Empty;

        public string StatusId { get; set; } = string.Empty;
        public List<string> MemberAvatarUrls { get; set; } = new List<string>();

        public string StatusName { get; set; } = string.Empty;
    }
}
