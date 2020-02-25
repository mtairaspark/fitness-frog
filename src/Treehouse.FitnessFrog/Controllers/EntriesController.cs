using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today,
                ActivityId = 2
            };
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            ValidateEntry(entry);
            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);
                return RedirectToAction("Index");
            }
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var entry = _entriesRepository.GetEntry((int)id);
            if(entry == null)
            {
                return HttpNotFound();
            }
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            ValidateEntry(entry);
            if (ModelState.IsValid) 
            {
                _entriesRepository.UpdateEntry(entry);
                return RedirectToAction("Index");
            }
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Retrieve entry for the provided value
            Entry entry = _entriesRepository.GetEntry((int)id);

            //Return not found if not found
            if(entry == null)
            {
                return HttpNotFound();
            }

            //Pass entry to the view
            return View("Delete",entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Delete the entry
            _entriesRepository.DeleteEntry(id);

            //Redirect to entries view
            return RedirectToAction("Index");
        }

        private void ValidateEntry(Entry entry)
        {
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "Duration must be greater than 0");
            }
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}