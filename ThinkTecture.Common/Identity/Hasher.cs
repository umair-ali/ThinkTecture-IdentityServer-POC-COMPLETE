using Microsoft.AspNet.Identity;

namespace ThinkTecture.Common.Identity
{
    public class ApplicationPasswordHasher : PasswordHasher
    {
        #region custom pasword hashing - testing only
        //public override string HashPassword(string password)
        //{
        //    return Reverse(password);
        //}

        //public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        //{
        //    return hashedPassword.Equals(Reverse(providedPassword)) ?
        //        PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        //}

        //public static string Reverse(string s)
        //{
        //    var charArray = s.ToCharArray();
        //    Array.Reverse(charArray);
        //    return new string(charArray);
        //}
        #endregion
    }
}
