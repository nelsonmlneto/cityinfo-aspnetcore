namespace CItyInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        public void Send(string message)
        {
            Console.WriteLine("CloudMailService");
            Console.WriteLine(message);
        }
    }
}
