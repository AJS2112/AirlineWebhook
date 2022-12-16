﻿using Microsoft.EntityFrameworkCore;
using TravelAgentWeb.Models;

namespace TravelAgentWeb.Data
{
    public class TravelAgentDbContext : DbContext
    {
        public TravelAgentDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<WebhookSecret> SubscriptionSecrets { get; set; }
    }
}
