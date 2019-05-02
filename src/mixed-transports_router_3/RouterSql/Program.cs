using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Router;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.Router.MixedTransports.Router";

        var routerConfig = new RouterConfiguration("Samples.Router.MixedTransports.Router");

        var msmqInterface = routerConfig.AddInterface<MsmqTransport>("MSMQ", t =>
        {
            
        });
        msmqInterface.EnableMessageDrivenPublishSubscribe(new InMemorySubscriptionStorage());


        var cs = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=NSBTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        var sqlServerInterface = routerConfig.AddInterface<SqlServerTransport>("SqlServer", t =>
        {
            t.ConnectionString(cs);
        });

        sqlServerInterface.EnableMessageDrivenPublishSubscribe(new InMemorySubscriptionStorage());

        var staticRouting = routerConfig.UseStaticRoutingProtocol();
        staticRouting.AddForwardRoute("MSMQ", "SqlServer");
        staticRouting.AddForwardRoute("SqlServer", "MSMQ" );
        routerConfig.AutoCreateQueues();


        var router = Router.Create(routerConfig);

        await router.Start().ConfigureAwait(false);

        Console.WriteLine("Press <enter> to exit");
        Console.ReadLine();

        await router.Stop().ConfigureAwait(false);
    }
}