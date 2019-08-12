﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpirit.BeerXchange.Model;

namespace Xpirit.BeerXchange.Services
{
    public class BeerService : IBeerService
    {
        private readonly BeerXchangeContext context;

        public BeerService(BeerXchangeContext context)
        {
            this.context = context;
        }

        public async Task AddBeer(Beer beer)
        {
            context.Beer.Add(beer);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Beer>> GetAllBeers()
        {
            return await context.Beer.ToListAsync();
        }

        public async Task<Beer> GetBeerById(int id)
        {
            return await context.Beer.SingleAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Beer>> GetBeerHistory()
        {
            return await context.Beer.Where(b => b.RemovedDate != null || b.RemovedBy != null).OrderByDescending(b => b.RemovedDate).ToListAsync();
        }

        public async Task<IEnumerable<Beer>> GetCurrentBeers()
        {
            return await context.Beer.Where(b => b.RemovedBy == null).ToListAsync();
        }

        public async Task UpdateBeer(Beer beer)
        {
            if (beer is null)
            {
                throw new ArgumentNullException(nameof(beer));
            }

            var existingBeer = context.Beer.Single(b => b.Id == beer.Id);
            if (existingBeer is null)
            {
                throw new ArgumentException(nameof(beer),$"Beer with Id {beer.Id} does not exist");
            }

            context.Update(beer);
            await context.SaveChangesAsync();
        }

        public async Task<int> GetUserCredits(string user)
        {
            return await context.Beer.CountAsync(b => b.CreatedBy == user) - await context.Beer.CountAsync(b => b.RemovedBy == user);
        }
    }
}
