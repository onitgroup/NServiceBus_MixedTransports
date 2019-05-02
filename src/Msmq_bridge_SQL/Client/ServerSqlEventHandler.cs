using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class MyEventHandler //: IHandleMessages<ServerSqlEvent>
{
    static ILog log = LogManager.GetLogger<MyEventHandler>();

    public Task Handle(ServerSqlEvent message, IMessageHandlerContext context)
    {
        log.Info($"Event {message.Id}");
        return Task.CompletedTask;
    }
}