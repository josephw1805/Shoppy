using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shoppy.Models;

public class RoleManagementVM
{
  public ApplicationUser ApplicationUser { get; set; }
  public IEnumerable<SelectListItem> RoleList { get; set; }
  public IEnumerable<SelectListItem> CompanyList { get; set; }
}
