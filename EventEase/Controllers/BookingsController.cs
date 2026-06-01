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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string searchString)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    b.BookingId.ToString().Contains(searchString) ||
                    b.Event.Title.Contains(searchString));
            }

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] =
                new SelectList(_context.Events, "EventId", "Title");

            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("BookingId,CustomerName,Email,Tickets,StartDate,EndDate,EventId")]
            Booking booking)
        {
            bool overlappingBooking = _context.Bookings
                .Any(b =>
                    b.EventId == booking.EventId &&
                    booking.StartDate < b.EndDate &&
                    booking.EndDate > b.StartDate);

            if (overlappingBooking)
            {
                ModelState.AddModelError(
                    "",
                    "This event already has a booking during the selected dates.");
            }

            if (booking.EndDate <= booking.StartDate)
            {
                ModelState.AddModelError(
                    "",
                    "End date must be after start date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] =
                new SelectList(_context.Events,
                               "EventId",
                               "Title",
                               booking.EventId);

            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            ViewData["EventId"] =
                new SelectList(_context.Events,
                               "EventId",
                               "Title",
                               booking.EventId);

            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("BookingId,CustomerName,Email,Tickets,StartDate,EndDate,EventId")]
            Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            bool overlappingBooking = _context.Bookings
                .Any(b =>
                    b.BookingId != booking.BookingId &&
                    b.EventId == booking.EventId &&
                    booking.StartDate < b.EndDate &&
                    booking.EndDate > b.StartDate);

            if (overlappingBooking)
            {
                ModelState.AddModelError(
                    "",
                    "This event already has a booking during the selected dates.");
            }

            if (booking.EndDate <= booking.StartDate)
            {
                ModelState.AddModelError(
                    "",
                    "End date must be after start date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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

            ViewData["EventId"] =
                new SelectList(_context.Events,
                               "EventId",
                               "Title",
                               booking.EventId);

            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}