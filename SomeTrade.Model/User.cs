namespace SomeTrade.Model
{
    public class User : Core.ModelBase
    {
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Address { get; set; }
        public string IdNo { get; set; }
        public string IpAddress { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
        public bool IsBanned { get; set; }
        public bool IsDeleted { get; set; }
    }
}
