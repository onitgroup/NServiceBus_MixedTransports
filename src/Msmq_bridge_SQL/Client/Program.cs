using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    const string letters = "ABCDEFGHIJKLMNOPQRSTUVXYZ";

    static async Task Main()
    {
        Console.Title = "MSMQ_Service";

        var random = new Random();
        var endpointConfiguration = new EndpointConfiguration("MSMQ_Service");

        var transport = endpointConfiguration.UseTransport<MsmqTransport>();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        var recoverability = endpointConfiguration.Recoverability();
        recoverability.Immediate(immediate => immediate.NumberOfRetries(0));
        recoverability.Delayed(delayed => delayed.NumberOfRetries(0));

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.EnableInstallers();



        var route = transport.Routing();
        route.RouteToEndpoint(typeof(ServerSqlMessage), "MSMQ_SQL_Router");
        route.RegisterPublisher(typeof(ServerSqlEvent), "MSMQ_SQL_Router");
        
        //var bridge = transport.Routing().ConnectToRouter("Samples.Router.MixedTransports.Router");
        //bridge.RouteToEndpoint(typeof(MyMessage), "Samples.Router.MixedTransports.Server");
        //bridge.RegisterPublisher(typeof(MyEvent), "Samples.Router.MixedTransports.Server");



        var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

        Console.WriteLine("Press <enter> to send a message");

        while (true)
        {
            Console.ReadLine();
            var id = new string(Enumerable.Range(0, 4).Select(x => letters[random.Next(letters.Length)]).ToArray());

            //await endpointInstance.Send(new ServerSqlMessage
            //{
            //    Id = id
            //}).ConfigureAwait(false);

            await endpointInstance.Publish(new ClientMsmqEvent
            {
                Id = id
            }).ConfigureAwait(false);
        }
    }
}