﻿using MeetMe.DTOs;
using MeetMe.Entities;
using MeetMe.Extentions;
using MeetMe.Helpers;
using MeetMe.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetMe.Data.Repository
{
    public class LikesRepository : ILikeRepository
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

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.Predicate == "liked") 
            {
                likes = likes.Where(l => l.SourceUserId == likesParams.UserId);
                users = likes.Select(l => l.LikedUser);
            }
            if (likesParams.Predicate == "likedBy") 
            {
                likes = likes.Where(l => l.LikedUserId == likesParams.UserId);
                users = likes.Select(l => l.SourceUser);
            }

            var likedUsers =  users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Country = user.Country,
                Id = user.Id

            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
