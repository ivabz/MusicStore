﻿using Microsoft.Framework.Cache.Memory;
using Microsoft.AspNet.Mvc;
using MusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly MusicStoreContext db;
        private readonly IMemoryCache cache;

        public HomeController(MusicStoreContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        //
        // GET: /Home/
        public IActionResult Index()
        {
            // Get most popular albums
            var albums = cache.GetOrAdd("topselling", context =>
            {
                //Refresh it every 10 minutes. Let this be the last item to be removed by cache if cache GC kicks in.
                context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                context.SetPriority(CachePreservationPriority.High);
                return GetTopSellingAlbums(6);
            });

            return View(albums);
        }

        private List<Album> GetTopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count

            // TODO [EF] We don't query related data as yet, so the OrderByDescending isn't doing anything
            return db.Albums
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count)
                .ToList();
        }
    }
}