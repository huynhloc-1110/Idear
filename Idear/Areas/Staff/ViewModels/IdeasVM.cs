using Idear.Models;

namespace Idear.Areas.Staff.ViewModels
{
    public class IdeasVM
    {
        public Idea Idea { get; set; } = default!;
        public List<Idea> RelatedIdeas { get; set;} = default!;

        public Comment Comment { get; set; } = default!;

        public int ReactFlag { get; set; } = 0;
    }
}
