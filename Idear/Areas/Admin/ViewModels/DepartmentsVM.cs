using Idear.Models;

namespace Idear.Areas.Admin.ViewModels
{
    public class DepartmentsVM
    {
        public Department? Department { get; set; }
        public List<Department>? Departments { get; set; }
    }
}