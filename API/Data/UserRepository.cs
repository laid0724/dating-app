using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username, bool? isCurrentUser)
        {
            // AutoMapper queryable extension: ProjectTo<ClassToMapTo>(_mapper.ConfigurationProvider)
            // better than having to use select(e => new ClassName{ ... })
            // and manually typing each property and assignment; there is no spread operator like JS.
            // a benefit of this is that you no longer need to include additional tables, 
            // automapper will do this automatically

            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(r => r.Role)
                .AsNoTracking()
                .Where(u => u.UserRoles.Any(r => r.Role.Name == "Member"))
                .Where(e => e.UserName.ToLower().Trim() == username.ToLower().Trim())
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            if (isCurrentUser.HasValue && isCurrentUser.Value) query = query.IgnoreQueryFilters();

            return await query.FirstOrDefaultAsync();

            // var user = await GetUserByUserNameAsync(username);
            // var mappedMember = _mapper.Map<MemberDto>(user);
            // return mappedMember;
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            // return await _context.Users
            //     .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            //     .ToListAsync();

            // var users = await GetUsersAsync();
            // var mappedMembers = _mapper.Map<IEnumerable<MemberDto>>(users);
            // return mappedMembers;

            // PAGED && FILTERING LOGIC:
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(r => r.Role)
                .AsNoTracking()
                .Where(u => u.UserRoles.Any(r => r.Role.Name == "Member"))
                .AsQueryable();

            query = query.Where(e => e.UserName != userParams.CurrentUserName);
            query = query.Where(e => e.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(e => e.DateOfBirth >= minDob && e.DateOfBirth <= maxDob);

            // new switch syntax in C# 8, _ is default case
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(e => e.Created),
                _ => query.OrderByDescending(e => e.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(
                query
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking(), // read-only mode, wont trigger database changes
                userParams.PageNumber,
                userParams.PageSize
            );
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByPhotoId(int photoId)
        {
            return await _context.Users
                .Include(u => u.Photos)
                .IgnoreQueryFilters()
                .Where(u => u.Photos.Any(p => p.Id == photoId))
                .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users
                .Include(e => e.Photos)
                .SingleOrDefaultAsync(e => e.UserName.ToLower().Trim() == username.ToLower().Trim());
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users
                .Where(u => u.UserName == username)
                .Select(u => u.Gender)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(e => e.Photos)
                .ToListAsync();
        }

        public void Update(AppUser user)
        {
            // this lets entity add a flag to the user entity to denote that it has been modified
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}