using Idear.Models;

namespace Idear.Areas.Staff.ViewModels
{
    public class HomeVM
    {
        public List<Topic> OpenedTopics { get; set; } = default!;
        public List<Comment> LatestComments { get; set; } = default!;
        public List<Idea> LatestIdeas { get; set; } = default!;
        public List<Idea> MostLikedIdeas { get; set; } = default!;
        public List<Idea> MostViewedIdeas { get; set; } = default!;
    }
}
