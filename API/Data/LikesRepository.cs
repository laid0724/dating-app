using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.LikedUser);
            }

            if (predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == userId);
                users = likes.Select(like => like.SourceUser);
            }

            return await users.Select(user => new LikeDto
            {
                Id = user.Id,
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City
            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithLike(int userId)
        {
            return await _context.Users
                .Include(e => e.LikedUsers)
                .FirstOrDefaultAsync(e => e.Id == userId);
        }

        public async Task<bool> DeleteLike(UserLike like)
        {
            // see: https://stackoverflow.com/questions/23315542/finding-an-element-in-a-dbset-with-a-composite-primary-key
            var likeEntity = await _context.Likes.FindAsync(like.SourceUserId, like.LikedUserId);

            _context.Likes.Remove(like);

            return await SaveAllAsync();
        }
        public async Task<bool> SaveAllAsync()
        {
            // if more than a single change has been made when saving the db changes
            return await _context.SaveChangesAsync() > 0;
        }
    }
}