namespace LoginServer.Models
{
    public class person
    {
        public int person_id { get; set; }
        public int display_person_id { get; set; }
        public string login_provider { get; set; }
        public string person_hash { get; set; }
        public string email { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
