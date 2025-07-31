using Core.DTOs.UnitOfMeasure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories
{
    public interface IUnitOfMeasureRepository
    {
        Task<List<UnitOfMeasureDto>> GetUnitOfMeasuresByItemAsync(string itemCode);
    }
}
