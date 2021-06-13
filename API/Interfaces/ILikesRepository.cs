using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<bool> SaveAllAsync();
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLike(int userId);
        Task<IEnumerable<LikeDto>> GetAllUserLikes(string predicate, int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
        Task<bool> DeleteLike(UserLike like);
    }
}