using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuctionApp.Models;
using AuctionApp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace AuctionApp.Controllers
{
    [Authorize]
    public class BidsController : Controller
    {
        private readonly AuctionRepository _repository;

        public BidsController(AuctionRepository repository)
        {
            _repository = repository;
        }

        // GET: Bids
        public async Task<IActionResult> Index()
        {
            var auctions = _repository.GetBids();
            return View(await auctions.ToListAsync());
        }

        // GET: Bids/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bid = await _repository.GetAsync<Bid, int>((int)id);
            if (bid == null)
            {
                return NotFound();
            }

            return View(bid);
        }

        // GET: Bids/Create/1 <-- AuctionId
        public IActionResult Create(int? id)
        {
            var userName = this.HttpContext.User.Identity.Name;
            ViewData["UserName"] = userName;
            ViewData["AuctionId"] = new SelectList(_repository.GetAuctions(), "AuctionId", "ShortDescription", id);

            var newBid = new Bid()
            {
                UserName = userName,
                AuctionId = id ?? 0,
                CreatedBy = userName,
                CreatedDate = DateTime.UtcNow
            };

            return View(newBid);
        }

        // POST: Bids/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Amount,AuctionId,CreatedBy,CreatedDate")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(bid);
                await _repository.SaveChangesAsync();
                return RedirectToAction(nameof(Details), "Auctions", new { id = bid.AuctionId });
            }

            ViewData["AuctionId"] = new SelectList(_repository.GetAuctions(), "AuctionId", "ShortDescription", bid.AuctionId);
            return View(bid);
        }

        // GET: Bids/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var (found, bid) = await _repository.TryGetAsync<Bid, int>((int)id);
                if (found)
                {
                    ViewData["AuctionId"] = new SelectList(_repository.GetAuctions(), "AuctionId", "ShortDescription", bid.AuctionId);
                    return View(bid);
                }
            }

            return NotFound();
        }

        // POST: Bids/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BidId,UserName,Amount,AuctionId,CreatedBy,CreatedDate")] Bid bid)
        {
            if (id != bid.BidId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(bid);
                    await _repository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _repository.KeyExistsAsync<Bid, int>(bid.BidId))
                    {
                        throw;
                    }

                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuctionId"] = new SelectList(_repository.GetAuctions(), "AuctionId", "ShortDescription", bid.AuctionId);
            return View(bid);
        }

        // GET: Bids/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var (found, bid) = await _repository.TryGetAsync<Bid, int>((int)id);
                if (found)
                {
                    return View(bid);
                }
            }

            return NotFound();
        }

        // POST: Bids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bid = await _repository.GetAsync<Bid, int>(id);
            _repository.Remove(bid);
            await _repository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
