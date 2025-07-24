using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyCompany.Data;

public abstract class RepositoryBase
{
    protected readonly DbContext Context;
    protected readonly ILogger Logger;
    protected RepositoryBase(ILogger logger, DbContext context)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(context);
        
        Logger = logger;
        Context = context;
    }
}