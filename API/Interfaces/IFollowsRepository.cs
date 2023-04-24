using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IFollowsRepository
    {
        Task<UserFollows> GetUserFollows(int sourceUserId, int targetUserId);
        Task<AppUser> GetUserWithFollows(int userId);

        Task<PagedList<FollowDto>> GetUserFollows(FollowsParams followsParams);
    }
}