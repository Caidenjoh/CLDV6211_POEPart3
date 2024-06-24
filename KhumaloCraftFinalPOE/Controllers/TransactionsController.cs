using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KhumaloCraftFinalPOE.Data;
using KhumaloCraftFinalPOE.Models;

namespace KhumaloCraftFinalPOE.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.Identity?.Name; // Getting the current logged-in user's ID
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not logged in
            }

            // Retrieve all transactions for the current user
            var transactions = await _context.Transactions
                                        .Where(t => t.UserId == userId)
                                        .ToListAsync();

            if (!transactions.Any())
            {
                TempData["Error"] = "No items in the cart to checkout.";
                return RedirectToAction("Index");
            }

            // Process each transaction
            foreach (var transaction in transactions)
            {
                var processor = new Processor
                {
                    UserId = transaction.UserId,
                    TransactionId = transaction.TransactionId, // Correct field assignment
                    Quantity = transaction.Quantity,
                    ProcessingDate = DateTime.Now
                };
                _context.Processor.Add(processor);

                // Optionally remove the transaction if you don't need it anymore
                //_context.Transactions.Remove(transaction);
            }

            //_context.Transactions.Remove(transaction);

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
                TempData["Success"] = "Checkout successful!";
            }
            catch (Exception ex)
            {
                // Log the error
                TempData["Error"] = "Checkout failed: " + ex.Message;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "Processors"); // Redirect to a confirmation page or processor index
        }

        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddToCart(int ProductId)
        {
            // Assuming you have a method to handle adding items to the cart
            var product = _context.Products.FirstOrDefault(p => p.ProductId == ProductId);
            if (product != null && product.Availability == true)
            {
                var transaction = new Transactions
                {
                    ProductId = product.ProductId,
                    Quantity = 1, // Example static quantity
                    TransactionDate = DateTime.Now,
                    UserId = User.Identity.Name // Assuming user's username is used as an ID
                };

                _context.Transactions.Add(transaction);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Transactions"); // Assuming TransactionsController handles transactions listing
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.Include(t => t.Products);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await _context.Transactions
                .Include(t => t.Products)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transactions == null)
            {
                return NotFound();
            }

            return View(transactions);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,UserId,ProductId,Quantity,TransactionDate")] Transactions transactions)
        {
            if (ModelState.IsValid)
            {
                transactions.UserId = User.Identity?.Name;
                _context.Add(transactions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", transactions.ProductId);
            return View(transactions);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await _context.Transactions.FindAsync(id);
            if (transactions == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", transactions.ProductId);
            return View(transactions);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,UserId,ProductId,Quantity,TransactionDate")] Transactions transactions)
        {
            if (id != transactions.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transactions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionsExists(transactions.TransactionId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", transactions.ProductId);
            return View(transactions);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await _context.Transactions
                .Include(t => t.Products)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transactions == null)
            {
                return NotFound();
            }

            return View(transactions);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transactions = await _context.Transactions.FindAsync(id);
            if (transactions != null)
            {
                _context.Transactions.Remove(transactions);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionsExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }

    }       
}
