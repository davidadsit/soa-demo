namespace RabbitWrapper
{
    public interface IMessageSerializer
    {
        string Serialize<T>(T objectToSerialize);
        T Deserialize<T>(string serializedObject);      
    }
}