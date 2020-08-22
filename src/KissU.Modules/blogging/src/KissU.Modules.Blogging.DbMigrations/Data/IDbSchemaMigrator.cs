using System.Threading.Tasks;

namespace KissU.Modules.Blogging.DbMigrations.Data
{
    public interface IDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
