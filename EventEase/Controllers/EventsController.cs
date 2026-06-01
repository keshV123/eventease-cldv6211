/*
 * EventEase - CLOUD Assignment Part 2
 * 
 * This project was developed using ASP.NET Core MVC scaffolding,
 * Microsoft documentation, Azure Blob Storage documentation,
 * and AI-assisted guidance for implementation refinement,
 * validation logic, and UI improvements.
 * 
 * Technologies used:
 * - ASP.NET Core MVC
 * - Entity Framework Core
 * - Azure Blob Storage / Azurite
 * - SQL Server LocalDB
 * 
 * Author: ST10474344
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.Services;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BlobService _blobService;

        public EventsController(
            AppDbContext context,
            BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Events
        public async Task<IActionResult> Index(
            int? eventTypeId,
            DateTime? fromDate,
            DateTime? toDate,
            bool venueAvailable = false)
        {
            var query = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if (eventTypeId.HasValue)
                query = query.Where(e => e.EventTypeId == eventTypeId);

            if (fromDate.HasValue)
                query = query.Where(e => e.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(e => e.Date <= toDate.Value);

            if (venueAvailable && fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(e => !_context.Bookings.Any(b =>
                    b.Event.VenueId == e.VenueId &&
                    b.StartDate < toDate.Value &&
                    b.EndDate > fromDate.Value));
            }

            ViewData["EventTypeId"] = new SelectList(
                _context.EventTypes, "EventTypeId", "TypeName", eventTypeId);

            return View(await query.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] =
                new SelectList(_context.Venues, "VenueId", "VenueName");
            ViewData["EventTypeId"] =
                new SelectList(_context.EventTypes, "EventTypeId", "TypeName");

            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("EventId,Title,Date,Description,VenueId,EventTypeId")]
            Event @event,
            IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        @event.ImageUrl =
                            await _blobService.UploadFileAsync(imageFile);
                    }

                    _context.Add(@event);

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(
                        "",
                        "An error occurred while uploading the image.");
                }
            }

            ViewData["VenueId"] =
                new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
            ViewData["EventTypeId"] =
                new SelectList(_context.EventTypes, "EventTypeId", "TypeName", @event.EventTypeId);

            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            ViewData["VenueId"] =
                new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
            ViewData["EventTypeId"] =
                new SelectList(_context.EventTypes, "EventTypeId", "TypeName", @event.EventTypeId);

            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("EventId,Title,Date,ImageUrl,Description,VenueId,EventTypeId")]
            Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] =
                new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
            ViewData["EventTypeId"] =
                new SelectList(_context.EventTypes, "EventTypeId", "TypeName", @event.EventTypeId);

            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasBookings = _context.Bookings.Any(b => b.EventId == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] =
                    "Cannot delete event with active bookings.";

                return RedirectToAction(nameof(Index));
            }

            var @event = await _context.Events.FindAsync(id);

            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}