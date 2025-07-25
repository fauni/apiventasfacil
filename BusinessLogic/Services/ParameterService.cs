using BusinessLogic.Data;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ParameterService : IParameterService
    {
        private readonly AppDbContext _context;
        public ParameterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Parameter> CreateAsync(Parameter parameter)
        {
            parameter.CreatedAt = DateTime.UtcNow;
            _context.Parameters.Add(parameter);
            await _context.SaveChangesAsync();
            return parameter;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var parameter = await _context.Parameters.FindAsync(id);
            if (parameter == null) return false;

            _context.Parameters.Remove(parameter);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Parameter>> GetAllAsync()
        {
            return await _context.Parameters.ToListAsync();
        }

        public async Task<Parameter> GetByIdAsync(int id)
        {
            return await _context.Parameters.FindAsync(id);
        }

        public async Task<SapConnectionSettings> GetParametersByGroupAsync(string group)
        {
            var parameters = await _context.Parameters
                .Where(p => p.Group == group && p.Enable)
                .ToDictionaryAsync(p => p.Name, p => p.Value);

            return new SapConnectionSettings
            {
                User = parameters["user"],
                Password = parameters["password"],
                ServiceLayerUrl = parameters["service_layer"],
                Database = parameters["bd_sap"],
                UserDatabase = parameters["bd_user"],
                PasswordDatabase = parameters["bd_password"],
                ServerDatabase = parameters["bd_server"]
            };
        }

        public async Task<SapConnectionSettings> GetSapConnectionSettingsAsync()
        {
            var parameters = await _context.Parameters
                .Where(p => p.Group == "ireilab" && p.Enable)
                .ToDictionaryAsync(p => p.Name, p => p.Value);

            return new SapConnectionSettings
            {
                User = parameters["user"],
                Password = parameters["password"],
                ServiceLayerUrl = parameters["service_layer"],
                Database = parameters["bd_sap"],
                UserDatabase = parameters["bd_user"],
                PasswordDatabase = parameters["bd_password"]
            };
        }

        public async Task<Parameter> UpdateAsync(int id, Parameter parameter)
        {
            var existing = await _context.Parameters.FindAsync(id);
            if (existing == null) return null;

            existing.Group = parameter.Group;
            existing.Name = parameter.Name;
            existing.Label = parameter.Label;
            existing.Value = parameter.Value;
            existing.Type = parameter.Type;
            existing.Enable = parameter.Enable;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
