using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class SapSessionService : ISapSessionService
    {
        private readonly IParameterService _parameterService;
        private SapConnectionSettings _settings;
        private string _sessionCookie = "";
        private string _routeId = "";

        public SapSessionService(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        public async Task<string> GetSessionCookieAsync()
        {
            if (!string.IsNullOrEmpty(_sessionCookie))
                return $"{_sessionCookie};{_routeId}";

            _settings = await _parameterService.GetParametersByGroupAsync("ireilab");

            // Crear cliente con validación SSL deshabilitada para desarrollo
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };

            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(_settings.ServiceLayerUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonConvert.SerializeObject(new
            {
                CompanyDB = _settings.Database,
                UserName = _settings.User,
                Password = _settings.Password
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Login", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error en el login al Service Layer: " + body);
            }

            // Extraer las cookies necesarias
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    if (cookie.StartsWith("B1SESSION"))
                        _sessionCookie = cookie.Split(';')[0];
                    if (cookie.StartsWith("ROUTEID"))
                        _routeId = cookie.Split(';')[0];
                }
            }

            return $"{_sessionCookie};{_routeId}";
        }

        public async Task LogoutAsync()
        {
            if (string.IsNullOrEmpty(_sessionCookie))
                return; // Ya está desconectado

            if (_settings == null)
            {
                _settings = await _parameterService.GetParametersByGroupAsync("ireilab");
            }

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };

            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(_settings.ServiceLayerUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Cookie", $"{_sessionCookie};{_routeId}");

            var response = await client.PostAsync("Logout", null);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception("Error al cerrar sesión en el Service Layer: " + body);
            }

            // Limpiar cookies locales
            _sessionCookie = "";
            _routeId = "";
        }
    }
}
