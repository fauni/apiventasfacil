using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> CreateAsync(User user);
        Task<User?> LoginAsync(string username, string password);
    }
}
