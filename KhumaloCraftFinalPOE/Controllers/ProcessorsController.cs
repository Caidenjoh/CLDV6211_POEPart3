using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KhumaloCraftFinalPOE.Data;
using KhumaloCraftFinalPOE.Models;
using Microsoft.Extensions.Logging;

namespace KhumaloCraftFinalPOE.Controllers
{
    public class ProcessorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProcessorsController> _logger;

        public ProcessorsController(ApplicationDbContext context, ILogger<ProcessorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Processors
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Processor.Include(p => p.Transaction);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Processors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var processor = await _context.Processor
                .Include(p => p.Transaction)
                .FirstOrDefaultAsync(m => m.ProcessorId == id);
            if (processor == null)
            {
                return NotFound();
            }

            return View(processor);
        }

        // GET: Processors/Create
        public IActionResult Create()
        {
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId");
            return View();
        }

        // POST: Processors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProcessorId,UserId,TransactionId,Quantity,ProcessingDate")] Processor processor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(processor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId", processor.TransactionId);
            return View(processor);
        }

        // GET: Processors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var processor = await _context.Processor.FindAsync(id);
            if (processor == null)
            {
                return NotFound();
            }
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId", processor.TransactionId);
            return View(processor);
        }

        // POST: Processors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProcessorId,UserId,TransactionId,Quantity,ProcessingDate")] Processor processor)
        {
            if (id != processor.ProcessorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(processor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcessorExists(processor.ProcessorId))
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
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "TransactionId", "TransactionId", processor.TransactionId);
            return View(processor);
        }

        // GET: Processors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var processor = await _context.Processor
                .Include(p => p.Transaction)
                .FirstOrDefaultAsync(m => m.ProcessorId == id);
            if (processor == null)
            {
                return NotFound();
            }

            return View(processor);
        }

        // POST: Processors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var processor = await _context.Processor.FindAsync(id);
            if (processor != null)
            {
                _context.Processor.Remove(processor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProcessorExists(int id)
        {
            return _context.Processor.Any(e => e.ProcessorId == id);
        }

        // GET: Processors/ProcessOrder/5
        public async Task<IActionResult> ProcessOrder(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                TempData["Error"] = "Transaction not found.";
                return RedirectToAction("Index");
            }

            _logger.LogInformation($"Processing transaction: {transaction.TransactionId} for user: {transaction.UserId}");

            // Check if the product exists in the Products table
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == transaction.ProductId);
            if (!productExists)
            {
                TempData["Error"] = $"Product with ID {transaction.ProductId} does not exist.";
                _logger.LogError($"Product with ID {transaction.ProductId} does not exist.");
                return RedirectToAction("Index");
            }

            // Product exists, proceed with creating the order history
            var orderHistory = new OrderHistory
            {
                UserId = transaction.UserId,
                TransactionId = transaction.TransactionId,
                Quantity = transaction.Quantity,
                OrderDate = DateTime.Now,
                ProductId = transaction.ProductId
            };
            _context.OrderHistory.Add(orderHistory);

            // Optionally remove the transaction if you don't need it anymore
            _context.Transactions.Remove(transaction);

            // Save changes outside the loop after processing all transactions
            await _context.SaveChangesAsync();

            TempData["Success"] = "Order processed successfully.";
            return RedirectToAction("Index", "OrderHistories"); // Redirect to a confirmation page or order history index
        }
    }
}

