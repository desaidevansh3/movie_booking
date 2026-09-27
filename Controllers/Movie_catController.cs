using movie_booking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace movie_booking.Controllers
{
    public class Movie_catController : Controller
    {
        // GET: Movie_cat
        public ActionResult Index()
        {
            Movies_Handler mc = new Movies_Handler();
            ModelState.Clear();
            List<Movie_Category> movie_list = mc.displaymoviecategory();
            return View(movie_list);
        }

        // GET: Movie_cat/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Movie_cat/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movie_cat/Create
        [HttpPost]
        public ActionResult Create(Movie_Category mc)
        {
            try
            {
                Movies_Handler mhandle = new Movies_Handler();
                mhandle.Add_Movie_Category(mc);
                if(ModelState.IsValid) 
                {
                    ViewBag.Message = "Value Inserted";
                }
                else 
                {
                    ViewBag.Message = "Value Not Inserted";
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Movie_cat/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Movie_cat/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Movie_cat/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Movie_cat/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
