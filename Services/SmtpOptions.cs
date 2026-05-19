namespace Kyrsova_OOP.Services
{
    public class SmtpOptions
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool UseStartTls { get; set; } = true;
        public bool UseSsl { get; set; } = false;
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromAddress { get; set; } = "";
        public string FromName { get; set; } = "";
        public string ToAddress { get; set; } = "";
        public string DailyTime { get; set; } = "20:00";
        public string TimeZoneId { get; set; } = "GMT+2";
    }
}
