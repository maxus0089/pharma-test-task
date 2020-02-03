using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PharmStd.Data;
using PharmStd.Models;

namespace PharmStd.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostgresContext _sql;

        public HomeController(IPostgresContext sql)
        {
            _sql = sql;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Drugs");
        }

        [HttpGet]
        public async Task<IActionResult> Facilities()
        {
            var all = (await _sql.FacilitiesGet()).ToArray();
            return View("Facilities", new FacilitiesViewModel
            {
                Facilities = all,
                Patients = (await _sql.PatientsGet()).ToArray(),
                AllFacilities = all
                
            });
        }

        [HttpGet]
        public async Task<IActionResult> Patients()
        {
            var patients =( await _sql.PatientsGet()).ToArray();
            return View("Patients", new PatientsViewModel
            {
                Patients = patients,
                AllPatients = patients,
                Drugs = await _sql.DrugsGet()
            });
        }

        [HttpGet]
        public async Task<IActionResult> Drugs()
        {
            var drugs = await _sql.DrugsGet();
            return View("Drugs", drugs.OrderBy(x => x.Name));
        }

        [HttpPost]
        public async Task<IActionResult> DrugsFilter([FromForm] string name, [FromForm] string category,
            [FromForm] string activeComponent)
        {
            var drugs = (await _sql.DrugsGet())
                .Where(d =>
                    d.Name.ToLower().Contains(name?.ToLower() ?? "") &&
                    d.Category.ToLower().Contains(category?.ToLower() ?? "") &&
                    d.ActiveComponent.ToLower().Contains(activeComponent?.ToLower() ?? ""))
                .OrderBy(x => x.Name)
                .ToArray();
            return View("Drugs", drugs);
        }

        [HttpPost]
        public async Task<IActionResult> FacilitiesFilter([FromForm] string name)
        {
            var all = (await _sql.FacilitiesGet()).ToArray();
            var facilities = all.Where(f => f.Name.ToLower().Contains(name?.ToLower() ?? "")).ToArray();
            return View("Facilities", new FacilitiesViewModel
            {
                Facilities = facilities,
                Patients = (await _sql.PatientsGet()).ToArray(),
                AllFacilities = all
                
            });
        }

        [HttpPost]
        public async Task<IActionResult> PatientsFilter([FromForm] string firstName, [FromForm] string lastName)
        {
            var all = (await _sql.PatientsGet()).ToArray();
            var filter = all
                .Where(p =>
                    p.FirstName.ToLower().Contains(firstName?.ToLower() ?? "") &&
                    p.LastName.ToLower().Contains(lastName?.ToLower() ?? ""))
                .ToArray();
            return View("Patients", new PatientsViewModel
            {
                Patients = filter,
                AllPatients = all,
                Drugs = await _sql.DrugsGet()
            });
        }

        public async Task<IActionResult> AddPrescription([FromForm] int patientId,[FromForm] int drugId)
        {
            await _sql.AddPrescription(patientId, drugId);
            return RedirectToAction("Patients");
        }

        public async Task<IActionResult> AddPatient([FromForm] string firstName,[FromForm] string lastName)
        {
            await _sql.AddPatient(firstName, lastName);
            return RedirectToAction("Patients");
        }

        public async Task<IActionResult> AddFacility([FromForm] string name,[FromForm] string address,[FromForm] string phone)
        {
            await _sql.AddFacility(name,address,phone);
            return RedirectToAction("Facilities");
        }

        public async Task<IActionResult> AddPatientInFacility([FromForm] int patientId,[FromForm] int facilityId)
        {
            await _sql.AddPatientInFacility(patientId, facilityId);
            return RedirectToAction("Facilities");
        }
    }
}