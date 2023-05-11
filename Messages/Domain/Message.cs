namespace Messages.Domain;

public class Message
{
    // has two variables UUID and message, constructor that takes message and generates UUID with getters and setters
    private int UUID { get; set; }
    private string message { get; set; }
    
    public Message(string message)
    {
        UUID = Guid.NewGuid().GetHashCode();
        this.message = message;
    }
    public void AddMessage(string message)
    {
        UUID = Guid.NewGuid().GetHashCode();
        this.message = message;
    }
    
    public void AddMessage(int uuid, string message)
    {
        UUID = uuid;
        this.message = message;
    }
    //get only messages from Message
    public string GetMessages()
    {
        return message;
    }
    public int GetUUID()
    {
        return UUID;
    }
    public override string ToString()
    {
        // return UUID and message in string format
        return $"UUID: {UUID}, message: {message}";
    }
    public override bool Equals(object? obj)
    {
        // if obj is null or obj is not Message type return false
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        Message message = (Message) obj;
        return UUID == message.UUID && this.message == message.message;
    }
}