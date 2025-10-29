using Infrastructure.Contexts;
using Infrastructure.Entities.Audit;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

namespace Infrastructure.Middlewares;

public class AuditSaveChangesInterceptor(
    ICurrentUserService currentUserService, 
    IServiceProvider serviceProvider) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
    {
        var audit = CreateAudit(eventData.Context, currentUserService);

        using var context = serviceProvider.GetRequiredService<AuditContext>();

        context.AddRange(audit);
        await context.SaveChangesAsync();

        return result;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var audit = CreateAudit(eventData.Context, currentUserService);

        using var auditContext = serviceProvider.GetRequiredService<AuditContext>();

        auditContext.AddRange(audit);
        auditContext.SaveChanges();

        return result;
    }

    private static List<AuditEntry> CreateAudit(DbContext? context, ICurrentUserService currentUser)
    {
        if (context is null) return [];
        if (context is AuditContext) return [];

        context.ChangeTracker.DetectChanges();

        var entries = new List<AuditEntry>();
        var correlationId = Guid.CreateVersion7();
        var occurredOn = DateTime.UtcNow;

        var databaseName = context.Database.GetDbConnection().Database;

        foreach (var e in context.ChangeTracker.Entries())
        {
            var entityName = e.Metadata.GetSchemaQualifiedTableName();
            var key = e.Properties.Where(p => p.Metadata.IsPrimaryKey()).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
            var action = Enum.GetName(e.State);

            var oldValues = new Dictionary<string, object?>();
            var newValues = new Dictionary<string, object?>();

            foreach (var p in e.Properties)
            {
                var name = p.Metadata.Name;

                if (e.State == EntityState.Added)
                {
                    newValues[name] = p.CurrentValue;
                }
                else if (e.State == EntityState.Deleted)
                {
                    oldValues[name] = p.OriginalValue;
                }
                else if (p.IsModified)
                {
                    oldValues[name] = p.OriginalValue;
                    newValues[name] = p.CurrentValue;
                }
            }

            entries.Add(AuditEntry.Create(
                correlationId,
                string.Join('.', [databaseName, entityName]),
                action!,
                currentUser.UserName,
                JsonSerializer.Serialize(key),
                oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues),
                newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues),
                occurredOn
            ));

        }

        return entries;
    }
}
