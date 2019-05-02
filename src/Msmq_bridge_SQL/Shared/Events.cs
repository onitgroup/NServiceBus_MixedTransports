using NServiceBus;

public class ServerSqlEvent : IEvent
{
    public string Id { get; set; }
}

public class ClientMsmqEvent : IEvent
{
    public string Id { get; set; }
}