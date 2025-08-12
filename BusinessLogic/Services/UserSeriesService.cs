using BusinessLogic.Data;
using Core.DTOs;
using Core.DTOs.Sap;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class UserSeriesService : IUserSeriesService
    {
        private readonly AppDbContext _context;
        private readonly IUserSeriesRepository _userSeriesRepository;


        public UserSeriesService(AppDbContext context, IUserSeriesRepository userSeriesRepository)
        {
            _context = context;
            _userSeriesRepository = userSeriesRepository;
        }

        public async Task<UserSerie> AssignSeriesAsync(UserSeriesDto dto)
        {
            // Validar que la serie existe en SAP antes de asignar
            var availableSeries = await _userSeriesRepository.GetAvailableSapSeriesAsync();
            var seriesExists = availableSeries.Any(s => s.Series.ToString() == dto.IdSerie);

            if (!seriesExists)
            {
                throw new InvalidOperationException($"La serie {dto.IdSerie} no existe o no está disponible en SAP");
            }

            // Validar que el usuario no tenga ya esa serie asignada
            var existingAssignment = await _context.UserSeries
                .AnyAsync(us => us.IdUsuario == dto.IdUsuario && us.IdSerie == dto.IdSerie);

            if (existingAssignment)
            {
                throw new InvalidOperationException($"El usuario ya tiene asignada la serie {dto.IdSerie}");
            }

            return await _userSeriesRepository.AssignSeriesAsync(dto);
        }

        public async Task<IEnumerable<UserSerie>> GetSeriesByUserAsync(int userId)
        {
            return await _context
                .UserSeries
                .Where(us => us.IdUsuario == userId)
                .Include(us => us.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserSeriesDto>> GetSeriesByUserWithDetailsAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("UserId debe ser mayor a 0", nameof(userId));
            }

            return await _userSeriesRepository.GetSeriesByUserWithDetailsAsync(userId);
        }

        public async Task<IEnumerable<SapSeriesDto>> GetAvailableSapSeriesAsync(int objectCode = 17)
        {
            return await _userSeriesRepository.GetAvailableSapSeriesAsync(objectCode);
        }
    }
}
