
using System.Security.AccessControl;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace API.Data
{
    public class FollowsRepository : IFollowsRepository
    {
        private readonly DataContext _context ;
        
        public FollowsRepository(DataContext context)
        {
            _context = context;                 
        }
        public async Task<UserFollows> GetUserFollows(int sourceUserId, int targetUserId)
        {
            return await _context.Follows.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<FollowDto>> GetUserFollows(FollowsParams followsParams)
        {
             var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
             var follows = _context.Follows.AsQueryable();
             if(followsParams.Predicate == "followed")
             {
               follows = follows.Where(follow => follow.SourceUserId == followsParams.UserId);
               users = follows.Select(follow => follow.TargetUser);
             } 
            if(followsParams.Predicate == "followedBy")
             {
               follows = follows.Where(follow => follow.TargetUserId == followsParams.UserId);
               users = follows.Select(follow => follow.SourceUser);
             } 

             var followedUsers=   users.Select(user => new FollowDto 
             {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
             });

             return await PagedList<FollowDto>.CreateAsync(followedUsers, followsParams.PageNumber, followsParams.PageSize);
        }

        public async Task<AppUser> GetUserWithFollows(int userId)
        {
            return await _context.Users
            .Include(x => x.FollowedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}