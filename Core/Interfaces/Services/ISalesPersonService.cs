using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ISalesPersonService
    {
        /// <summary>
        /// Obtener un vendedor por su código
        /// </summary>
        /// <param name="slpCode">Código del vendedor</param>
        /// <returns>Datos del vendedor si existe y está activo</returns>
        Task<SalesPersonDto?> GetSalesPersonByCodeAsync(int slpCode);

        /// <summary>
        /// Obtener todos los vendedores activos
        /// </summary>
        /// <returns>Lista de vendedores activos</returns>
        Task<List<SalesPersonDto>> GetActiveSalesPersonsAsync();

        /// <summary>
        /// Verificar si un vendedor existe y está activo
        /// </summary>
        /// <param name="slpCode">Código del vendedor</param>
        /// <returns>True si existe y está activo</returns>
        Task<bool> ValidateSalesPersonAsync(int slpCode);
    }
}
