using Core.DTOs.TfeUnitOfMeasure;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class TfeUnitOfMeasureService : ITfeUnitOfMeasureService
    {
        private readonly ITfeUnitOfMeasureRepository _tfeUnitOfMeasureRepository;

        public TfeUnitOfMeasureService(ITfeUnitOfMeasureRepository tfeUnitOfMeasureRepository)
        {
            _tfeUnitOfMeasureRepository = tfeUnitOfMeasureRepository;
        }

        public async Task<List<TfeUnitOfMeasureDto>> GetTfeUnitsOfMeasureAsync()
        {
            return await _tfeUnitOfMeasureRepository.GetTfeUnitsOfMeasureAsync();
        }
    }
}
