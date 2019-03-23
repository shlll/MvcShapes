using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace project.Models.ViewShapes
{
    public class CreateEditShapesViewModel
    {
        [Required(ErrorMessage = "Type is required")]
        public string ShapeType { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public HttpPostedFileBase Media { get; set; }
    }
}