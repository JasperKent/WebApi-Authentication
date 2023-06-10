using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAuthentication.Authentication;
using WebApiAuthentication.DataAccess.Entities;

namespace WebApiAuthentication.DataAccess.Context
{
    public class ReviewContext : IdentityDbContext
    {
        public ReviewContext(DbContextOptions<ReviewContext> options)
            :base(options)
        {            
        }

        public DbSet<BookReview> BookReviews { get; set; }
        public DbSet<LibraryUser> LibraryUsers { get; set; }
    }
}
