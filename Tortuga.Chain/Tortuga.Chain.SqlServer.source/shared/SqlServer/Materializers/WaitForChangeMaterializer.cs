#if !SqlDependency_Missing
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer.Materializers
{


    internal static class WaitForChangeMaterializer
    {
        internal static Task GenerateTask<TCommandBuilder>(TCommandBuilder commandBuilder, CancellationToken cancellationToken, object state)
            where TCommandBuilder : DbCommandBuilder<SqlCommand, SqlParameter>, ISupportsChangeListener
        {
            var materializer = new WaitForChangeMaterializer<TCommandBuilder>(commandBuilder);
            return materializer.GenerateTask(cancellationToken, state);
        }
    }
}
#endif