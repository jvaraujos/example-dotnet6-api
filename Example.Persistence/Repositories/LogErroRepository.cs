using Microsoft.EntityFrameworkCore;
using Example.Application.Contracts.Persistence.Interfaces.Logs;
using Example.Domain.Entities.Logs;
using Example.Persistence.DbContexts;
using System.Linq.Expressions;

namespace Example.Persistence.Repositories
{
    public class LogErroRepository : BaseRepository<LogErro>, ILogErroRepository
    {
        public LogErroRepository(DbContexts.ExampleDbContext context) : base(context)
        {}
    }
}
