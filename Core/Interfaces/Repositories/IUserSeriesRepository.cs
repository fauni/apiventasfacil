using Core.DTOs;
using Core.DTOs.Sap;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories
{
    public interface IUserSeriesRepository
    {
        Task<UserSerie> AssignSeriesAsync(UserSeriesDto dto);
        Task<IEnumerable<UserSeriesDto>> GetSeriesByUserWithDetailsAsync(int userId);
        Task<IEnumerable<SapSeriesDto>> GetAvailableSapSeriesAsync(int objectCode = 17); // 17 = Sales Orders
    }
}
