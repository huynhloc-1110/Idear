using Idear.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Idear.Areas.Staff.ViewModels
{
	public class TopicIdeasVM
	{
		public Topic Topic { get; set; }

		public List<SelectListItem> Ideas { get; set; }
	}
}
