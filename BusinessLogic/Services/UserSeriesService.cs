using BusinessLogic.Data;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UserSeriesService : IUserSeriesService
    {
        private readonly AppDbContext _context;
        
        public UserSeriesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserSerie> AssignSeriesAsync(UserSeriesDto dto)
        {
            var userSeries = new UserSerie
            {
                IdUsuario = dto.IdUsuario,
                IdSerie = dto.IdSerie,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserSeries.Add(userSeries);
            await _context.SaveChangesAsync();
            return userSeries;
        }

        public async Task<IEnumerable<UserSerie>> GetSeriesByUserAsync(int userId)
        {
            return await _context
                .UserSeries
                .Where(us => us.IdUsuario == userId)
                .Include(us => us.User)
                .ToListAsync();
        }
    }
}
