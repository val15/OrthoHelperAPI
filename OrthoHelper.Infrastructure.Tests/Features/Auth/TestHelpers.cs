using Microsoft.EntityFrameworkCore;
using OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext;

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
