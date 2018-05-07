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
    public class AuctionsController : Controller
    {
        private readonly AuctionRepository _repository;

        public AuctionsController(AuctionRepository repository)
        {
            _repository = repository;
        }

        // GET: Auctions
        public async Task<IActionResult> Index() => View(await _repository.GetAuctions().ToListAsync());

        // GET: Auctions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                var (found, auction) = await _repository.TryGetAsync<Auction, int>((int)id);
                if (found)
                {
                    return View(auction);
                }
            }

            return NotFound();
        }

        // GET: Auctions/Create
        [Authorize]
        public IActionResult Create()
        {
            var userName = this.HttpContext.User.Identity.Name;

            var newAuction = new Auction()
            {
                UserName = userName,
                ModifiedBy = userName,
                CreatedBy = userName,
                CreatedDate = DateTime.UtcNow
            };

            return View(newAuction);
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuctionId,UserName,ShortDescription,StartAmount,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(auction);
                await _repository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(auction);
        }

        // GET: Auctions/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var (found, auction) = await _repository.TryGetAsync<Auction, int>((int)id);
                if (found)
                {
                    return View(auction);
                }
            }

            return NotFound();
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("AuctionId,UserName,ShortDescription,StartAmount,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate")] Auction auction)
        {
            if (id != auction.AuctionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(auction);
                    await _repository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _repository.KeyExistsAsync<Auction, int>(auction.AuctionId))
                    {
                        throw;
                    }

                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(auction);
        }

        // GET: Auctions/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var (found, auction) = await _repository.TryGetAsync<Auction, int>((int)id);
                if (found)
                {
                    return View(auction);
                }
            }

            return NotFound();
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auction = await _repository.GetAsync<Auction, int>(id);
            _repository.Remove(auction);
            await _repository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
