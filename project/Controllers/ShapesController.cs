using Microsoft.AspNet.Identity;
using project.Models;
using project.Models.Shape;
using project.Models.ViewShapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace project.Controllers
{
    public class ShapesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Shapes
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Shape
                .Where(p => p.UserId == userId)
                .Select(p => new IndexShapesViewModel
                {
                    Id = p.Id,
                    Category = p.Category,
                    Type = p.Type,
                    Description = p.Description
                }).ToList();

            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            PopulateViewBag();
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(CreateEditShapesViewModel model)
        {
            return SaveShapes(null,model);
        }
        public ActionResult SaveShapes(int? id, CreateEditShapesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewBag();
                return View();
            }
            var userId = User.Identity.GetUserId();

            if (db.Shape.Any(p => p.UserId == userId &&
            p.Type == model.ShapeType &&
            (!id.HasValue || p.Id != id.Value)))
            {
                ModelState.AddModelError(nameof(CreateEditShapesViewModel.ShapeType),
                    "Shape should be very special");

                PopulateViewBag();
                return View();
            }
            string fileExtension;
            if (model.Media != null)
            {
                fileExtension = Path.GetExtension(model.Media.FileName);

                if (!Constants.AllowedFileExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "File extension is not allowed.");
                    PopulateViewBag();
                    return View();
                }
            }
            Shapes shape;
            if (!id.HasValue)
            {
                shape = new Shapes();
                shape.UserId = userId;
                db.Shape.Add(shape);
            }
            else
            {
                shape = db.Shape.FirstOrDefault(p => p.Id == id && p.UserId == userId);

                if (shape == null)
                {
                    return RedirectToAction(nameof(ShapesController.Index));
                }
            }
            shape.Type = model.ShapeType;
            shape.Category = model.Category;
            shape.Description = model.Description;
            if (model.Media != null)
            {
                if (!Directory.Exists(Constants.MappedUploadFolder))
                {
                    Directory.CreateDirectory(Constants.MappedUploadFolder);
                }

                var fileName = model.Media.FileName;
                var fullPathWithName = Constants.MappedUploadFolder + fileName;

                model.Media.SaveAs(fullPathWithName);

                shape.MediaUrl = Constants.UploadFolder + fileName;
            }
            db.SaveChanges();
            return RedirectToAction(nameof(ShapesController.Index));
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ShapesController.Index));
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

          
            var userId = User.Identity.GetUserId();
            var shape = db.Shape.FirstOrDefault(
               p => p.Id == id && p.UserId == userId);
            if (shape == null)
            {
                return RedirectToAction(nameof(ShapesController.Index));
            }
            PopulateViewBag();
            var model = new CreateEditShapesViewModel();
            model.Category = shape.Category;
            model.ShapeType = shape.Type;
            model.Description = shape.Description;
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id,CreateEditShapesViewModel model)
        {
            return SaveShapes(id, model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ShapesController.Index));
            }
            var userId = User.Identity.GetUserId();
            var shape = db.Shape.FirstOrDefault(p => p.Id == id && p.UserId == userId);

            if (shape != null)
            {
                db.Shape.Remove(shape);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(ShapesController.Index));
        }
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(ShapesController.Index));

            var userId = User.Identity.GetUserId();

            var shape = db.Shape.FirstOrDefault(p =>
            p.Id == id.Value &&
            p.UserId == userId);

            if (shape == null)
                return RedirectToAction(nameof(ShapesController.Index));

            var model = new DetailsShapesViewModel();
            model.Category = shape.Category;
            model.Description = shape.Description;
            model.Type = shape.Type;
            model.MediaUrl = shape.MediaUrl;

            return View(model);
        }
        [HttpPost]
        public ActionResult Search(string search)
        {
            var shape = db.Shape.Where(p => p.Type.Contains(search))
          .Select(p => new IndexShapesViewModel
          {
              Id = p.Id,
              Type = p.Type,
              Category = p.Category,
              Description = p.Description
          }).ToList();
            return View("Index", shape);
        }
        private void PopulateViewBag()
        {
            var categories = new SelectList(
                                  new List<string>
                                  {
                                      "Trangle",
                                      "Rectangle",
                                      "Square",
                                      "Circle",
                                      "SemiCircle"
                                  });

            ViewBag.Categories = categories;
        }

    }
}