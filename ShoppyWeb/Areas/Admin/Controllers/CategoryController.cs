﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.DataAccess;
using Shoppy.Models;
using Shoppy.Utility;

namespace ShoppyWeb;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CategoryController(IUnitOfWork unitOfWork) : Controller
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public IActionResult Index()
  {
    List<Category> objCategoryList = [.. _unitOfWork.Category.GetAll()];
    return View(objCategoryList);
  }

  public IActionResult Create()
  {
    return View();
  }

  [HttpPost]
  public IActionResult Create(Category obj)
  {
    if (ModelState.IsValid)
    {
      _unitOfWork.Category.Add(obj);
      _unitOfWork.Save();
      TempData["success"] = "Category created successfully";
      return RedirectToAction("Index");
    }
    return View();
  }

  public IActionResult Edit(int? id)
  {
    if (id == null || id == 0)
    {
      return NotFound();
    }
    Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
    if (categoryFromDb == null)
    {
      return NotFound();
    }
    return View(categoryFromDb);
  }

  [HttpPost]
  public IActionResult Edit(Category obj)
  {
    if (ModelState.IsValid)
    {
      _unitOfWork.Category.Update(obj);
      _unitOfWork.Save();
      TempData["success"] = "Category updated successfully";
      return RedirectToAction("Index");
    }
    return View();
  }

  public IActionResult Delete(int? id)
  {
    if (id == null || id == 0)
    {
      return NotFound();
    }
    Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
    if (categoryFromDb == null)
    {
      return NotFound();
    }
    return View(categoryFromDb);
  }

  [HttpPost, ActionName("Delete")]
  public IActionResult DeletePost(int? id)
  {
    Category obj = _unitOfWork.Category.Get(u => u.Id == id);
    if (obj == null)
    {
      return NotFound();
    }
    _unitOfWork.Category.Remove(obj);
    _unitOfWork.Save();
    TempData["success"] = "Category deleted successfully";
    return RedirectToAction("Index");
  }
}
