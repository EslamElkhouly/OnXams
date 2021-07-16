namespace OnlineExamPlatform.Authentication
{
    public class HashingPassword
    {
        private static string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());
        }

        public static bool ValidatePassword(string password, string correctHash)
        {
            //Need To check For Null First Before Return
            return BCrypt.Net.BCrypt.Verify(password, correctHash);
        }
    }
}