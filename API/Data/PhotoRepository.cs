using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public PhotoRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await _context.Photos
                .IgnoreQueryFilters()
                .Where(p => !p.IsApproved)
                .ProjectTo<PhotoForApprovalDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<Photo> GetPhotoById(int photoId)
        {
            return await _context.Photos
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(p => p.Id == photoId);
        }


        public void RemovePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }
    }
}