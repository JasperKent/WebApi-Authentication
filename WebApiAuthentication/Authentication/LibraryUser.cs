using Microsoft.AspNetCore.Identity;
using WebApiAuthentication.DataAccess.Entities;

namespace WebApiAuthentication.Authentication
{
    public class LibraryUser : IdentityUser
    {
        public bool RatingsAllowed { get; set; }
        public required ICollection<BookReview> Reviews { get; set; }
    }
}
