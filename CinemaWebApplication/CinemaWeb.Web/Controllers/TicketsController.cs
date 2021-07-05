using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CinemaWeb.Services.Interface;
using CinemaWeb.Domain.DomainModels;
using CinemaWeb.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using System.IO;

namespace CinemaWeb.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: Tickets
        public IActionResult Index()
        {
            var allTickets = this._ticketService.GetAllProducts();
            return View(allTickets);
        }

        public IActionResult FilterByDate()
        {
            return View("Index", _ticketService.FilterTicketsByDate());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult ExportTicketsByGenre(string genre)
        {
            List<Ticket> filteredTickets = this._ticketService.GetAllTicketsByGenre(genre);

            string fileName = "Tickets.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            if (filteredTickets.Count == 0)
            {
                return RedirectToAction("Index", "Tickets", new { error = "No tickets with this genre." });
            }

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Tickets");

                worksheet.Cell(1, 1).Value = "Ticket Id";
                worksheet.Cell(1, 2).Value = "Ticket Genre";

                for (int i = 1, t = 0; i <= filteredTickets.Count; i++, t++)
                {
                    var item = filteredTickets[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.FilmGenre.ToString();
                    //pecatenje na biletite vednas pod Ticket-brojkata
                    //worksheet.Cell(1, t + 3).Value = "Ticket-" + (t + 1);
                    //worksheet.Cell(2, t + 3).Value = item.MovieName;

                    for (int j = 0; j < 1; j++)
                    {
                        worksheet.Cell(1, j + 3).Value = "Ticket Name";
                        worksheet.Cell(i + 1, j + 3).Value = item.FilmName;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }
            }

            //var workbook = WriteToCSV(filteredTickets);

            //var stream = new MemoryStream();
            //workbook.SaveAs(stream);
            //var content = stream.ToArray();

            //return File(content, contentType, fileName);
        }

        public IActionResult AddTicketToCard(Guid? id)
        {
            var model = this._ticketService.GetShoppingCartInfo(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTicketToCard([Bind("TicketId", "Quantity")] AddToShoppingCardDto item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._ticketService.AddToShoppingCart(item, userId);

            if (result)
            {
                return RedirectToAction("Index", "Tickets");
            }

            return View(item);
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForProduct(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,FilmName,FilmImage,FilmDescription,FilmPrice,FilmGenre,FilmTime,Rating")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                this._ticketService.CreateNewProduct(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForProduct(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,FilmName,FilmImage,FilmDescription,FilmPrice,FilmGenre,FilmTime,Rating")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._ticketService.UpdeteExistingProduct(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForProduct(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            this._ticketService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(Guid id)
        {
            return this._ticketService.GetDetailsForProduct(id) != null;
        }
    }
}
