using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "SQL_Service";
        var endpointConfiguration = new EndpointConfiguration("SQL_Service");

        var cs = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=NSB_MSMQ_SQL_Router;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
        transport.ConnectionString(cs);
     

        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        var recoverability = endpointConfiguration.Recoverability();
        recoverability.Immediate(immediate => immediate.NumberOfRetries(0));
        recoverability.Delayed(delayed => delayed.NumberOfRetries(0));

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.EnableInstallers();


        var routing = transport.Routing().ConnectToRouter("MSMQ_SQL_Router");
        routing.RegisterPublisher(typeof(ClientMsmqEvent), "MSMQ_Service");

        //routing.RouteToEndpoint(typeof(ServerSqlEvent), "MSMQ_SQL_Router");


        var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

        Console.WriteLine("Press <enter> to exit.");
        Console.ReadLine();

        await endpointInstance.Stop().ConfigureAwait(false);
    }
}