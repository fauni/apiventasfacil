using Core.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IParameterService
    {
        Task<SapConnectionSettings> GetSapConnectionSettingsAsync();
        Task<SapConnectionSettings> GetParametersByGroupAsync(string group);
        Task<Parameter> CreateAsync(Parameter parameter);
        Task<IEnumerable<Parameter>> GetAllAsync();
        Task<Parameter> GetByIdAsync(int id);
        Task<Parameter> UpdateAsync(int id, Parameter parameter);
        Task<bool> DeleteAsync(int id);
    }
}
