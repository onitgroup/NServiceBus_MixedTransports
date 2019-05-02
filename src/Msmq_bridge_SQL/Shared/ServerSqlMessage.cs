using NServiceBus;

public class ServerSqlMessage : IMessage
{
    public string Id { get; set; }
}