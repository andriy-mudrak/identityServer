namespace AuthServer.Models
{
    public class Localized
    {
        public string ru_RU { get; set; }
    }

    public class PreferredLocale
    {
        public string country { get; set; }
        public string language { get; set; }
    }

    public class LastName
    {
        public Localized localized { get; set; }
        public PreferredLocale preferredLocale { get; set; }
    }

    public class Localized2
    {
        public string ru_RU { get; set; }
    }

    public class PreferredLocale2
    {
        public string country { get; set; }
        public string language { get; set; }
    }

    public class FirstName
    {
        public Localized2 localized { get; set; }
        public PreferredLocale2 preferredLocale { get; set; }
    }

    public class ProfilePicture
    {
        public string displayImage { get; set; }
    }

    public class PersonLinkedinModel
    {
        public string localizedLastName { get; set; }
        public LastName lastName { get; set; }
        public FirstName firstName { get; set; }
        public ProfilePicture profilePicture { get; set; }
        public string id { get; set; }
        public string localizedFirstName { get; set; }
    }
}