using Idear.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Idear.Areas.Staff.ViewModels
{
	public class TopicIdeasVM
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public List<Idea> Ideas { get; set; }

	}
}
