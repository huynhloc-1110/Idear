using Idear.Models;

namespace Idear.Areas.QAManager.ViewModels
{
    public class ExceptionReportsVM
    {
        public List<Idea> NoCommentIdeas { get; set; } = default!;
        public List<Idea> AnonymousIdeas { get; set; } = default!;
        public List<Comment> AnonymousComments { get; set; } = default!;
    }
}
