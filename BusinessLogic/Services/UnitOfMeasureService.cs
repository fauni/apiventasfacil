using Core.DTOs.UnitOfMeasure;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UnitOfMeasureService: IUnitOfMeasureService
    {
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;

        public UnitOfMeasureService(IUnitOfMeasureRepository unitOfMeasureRepository)
        {
            _unitOfMeasureRepository = unitOfMeasureRepository;
        }

        public async Task<List<UnitOfMeasureDto>> GetUnitOfMeasuresByItemAsync(string itemCode)
        {
            return await _unitOfMeasureRepository.GetUnitOfMeasuresByItemAsync(itemCode);
        }
    }
}
