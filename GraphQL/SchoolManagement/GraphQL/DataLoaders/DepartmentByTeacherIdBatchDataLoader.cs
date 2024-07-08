﻿using Microsoft.EntityFrameworkCore;

using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.GraphQL.DataLoaders;

public class DepartmentByTeacherIdBatchDataLoader(
    IDbContextFactory<AppDbContext> dbContextFactory,
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options = null)
    : BatchDataLoader<Guid, Department>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<Guid, Department>> LoadBatchAsync(IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var departments = await dbContext.Departments.Where(x => keys.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
        return departments;
    }
}
