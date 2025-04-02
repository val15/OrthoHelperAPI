using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.Tests.Features.Auth
{
    public static class DbContextFactory
    {
        public static ApiDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de données en mémoire
                .Options;

            return new ApiDbContext(options);
        }
    }
}
