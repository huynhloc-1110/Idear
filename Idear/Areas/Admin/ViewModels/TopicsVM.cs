using Idear.Models;
using System.ComponentModel.DataAnnotations;

namespace Idear.Areas.Admin.ViewModels
{
    public class TopicsVM
    {
        public Topic? Topic { get; set; }
        public List<Topic>? Topics { get; set; }
    }
}
