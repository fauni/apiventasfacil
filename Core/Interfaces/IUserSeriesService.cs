using Core.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserSeriesService
    {
        Task<UserSerie> AssignSeriesAsync(UserSeriesDto dto);
        Task<IEnumerable<UserSerie>> GetSeriesByUserAsync(int userId);
    }
}
