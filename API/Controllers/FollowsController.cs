using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FollowsController : BaseAPIController
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowsRepository _followsRepository;
        public FollowsController(IUserRepository userRepository, IFollowsRepository followsRepository)
        {
            _followsRepository = followsRepository;
            _userRepository = userRepository;       
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddFollow (string username)
        {
            var sourceUserId = User.GetUserId();
            var followedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _followsRepository.GetUserWithFollows(sourceUserId);

            if(followedUser == null) return NotFound();

            if(sourceUser.UserName == username) return BadRequest("You cannot follow yourself");

            var userFollow = await _followsRepository.GetUserFollows(sourceUserId, followedUser.Id);

            if (userFollow!= null) return BadRequest("You already follow this user");

            userFollow = new UserFollows
            {
                SourceUserId = sourceUserId,
                TargetUserId = followedUser.Id
            };

            sourceUser.FollowedUsers.Add(userFollow);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to follow user"); 
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<FollowDto>>> GetUserFollows([FromQuery]FollowsParams followsParams)
        {
            followsParams.UserId = User.GetUserId();
            var users = await _followsRepository.GetUserFollows(followsParams);
            Response.AddpaginationHeader(new PaginationHeader(
                users.CurrentPage, 
                users.PageSize, 
                users.TotalCount, 
                users.TotalPages));
            return Ok(users);
        }
    }
}