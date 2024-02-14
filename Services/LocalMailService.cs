namespace CItyInfo.API.Services
{
    public class LocalMailService : IMailService
    {

        private readonly string _mailTo = String.Empty;
        private readonly string _mailFrom = String.Empty;

        public LocalMailService(IConfiguration configuration) 
        {
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }

        public void Send(string message)
        {
            Console.WriteLine($"LocalMailService from {_mailFrom} to {_mailTo}");
            Console.WriteLine(message);
        }
    }
}
