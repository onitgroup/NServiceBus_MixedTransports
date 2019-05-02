using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class MyMessageHandler :
    //IHandleMessages<ServerSqlMessage>,
    IHandleMessages<ClientMsmqEvent>
{
    static ILog log = LogManager.GetLogger<MyMessageHandler>();

    public async Task Handle(ServerSqlMessage message, IMessageHandlerContext context)
    {
        log.Info($"Request {message.Id}");
        await context.Publish(new ServerSqlEvent
        {
            Id = message.Id
        }).ConfigureAwait(false);

    }


    public async Task Handle(ClientMsmqEvent message, IMessageHandlerContext context)
    {
        log.Info($"Event {message.Id}");
        await context.Publish(new ServerSqlEvent { Id = "ciao" });
    }
}