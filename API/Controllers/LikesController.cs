using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ILogger<LikesController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(DataContext context, ILogger<LikesController> logger, IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        [Description("like a user")]
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUserNameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLike(sourceUserId);

            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You have already liked this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");
        }

        [Description("get user likes")]
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        [Description("get all user likes")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetAllUserLikes(string predicate)
        {
            var users = await _likesRepository.GetAllUserLikes(predicate, User.GetUserId());
            return Ok(users);
        }

        [Description("unlike a user")]
        [HttpDelete("{username}")]
        public async Task<ActionResult> DeleteLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUserNameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLike(sourceUserId);

            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You cannot unlike yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike == null) return NotFound("You did not like this user");

            if (await _likesRepository.DeleteLike(userLike)) return NoContent();

            return BadRequest("Failed to unlike user");
        }
    }
}