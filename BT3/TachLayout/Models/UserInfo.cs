namespace TachLayout.Models
{
    public class UserInfo
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Constructor mặc định
        public UserInfo()
        {
            Username = "";
            Email = "";
            Phone = "";
            Address = "";
        }

        // Constructor với tham số
        public UserInfo(int userID, string username)
        {
            UserID = userID;
            Username = username;
            Email = "";
            Phone = "";
            Address = "";
        }
    }
}