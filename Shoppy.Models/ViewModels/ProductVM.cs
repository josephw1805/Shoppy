using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shoppy.Models;

public class ProductVM
{
  public Product Product { get; set; }
  public IEnumerable<SelectListItem> CategoryList { get; set; }
}
