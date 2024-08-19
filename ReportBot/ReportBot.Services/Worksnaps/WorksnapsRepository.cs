using AutoMapper;
using McgTgBotNet.DTOs;
using McgTgBotNet.Extensions;
using McgTgBotNet.Models;
using ReportBot.Common.DTOs.Project;
using ReportBot.Common.Exceptions;
using ReportBot.Common.Extensions;
using ReportBot.DataBase.Repositories.Interfaces;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace ReportBot.Services.Worksnaps
{
    public class WorksnapsRepository : IWorksnapsRepository
    {
        private readonly HttpClient _httpClient;

        public WorksnapsRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void ConfigureHttpClient(string? token)
        {
            var apiKey = token != null ? token : ConfigExtension.GetConfiguration("Worksnaps:ApiKey");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", ApiExtension.DecodeToken(apiKey));
        }

        private async Task<XDocument> GetXmlResponseAsync(string requestUri, string? token)
        {
            ConfigureHttpClient(token);
            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return XDocument.Parse(content);
        }

        public async Task<List<SummaryReportDTO>> GetSummaryReportsAsync(string? token, DateTime from, DateTime to, string? projectIds)
        {
            var requestUri = $"https://api.worksnaps.com:443/api/summary_reports?from_date={from.Date.ToString("yyyy-MM-dd")}&to_date={to.Date.ToString("yyyy-MM-dd")}&name=manager_report&project_ids={projectIds}";
            var doc = await GetXmlResponseAsync(requestUri, token);
            var data = new List<SummaryReportDTO>();
            foreach (var element in doc.Root!.Elements())
            {
                var item = element.ParseXML<SummaryReportDTO>();

                data.Add(item);
            }

            return data;
        }

        public async Task<List<WorksnapsUserDTO>> GetUsersAsync(string? token)
        {
            var requestUri = "https://api.worksnaps.com:443/api/users.xml";
            var doc = await GetXmlResponseAsync(requestUri, token);
            var data = new List<WorksnapsUserDTO>();
            foreach (var element in doc.Root!.Elements())
            {
                var item = element.ParseXML<WorksnapsUserDTO>();

                data.Add(item);
            }

            return data;
        }

        public async Task<WorksnapsUserDTO> GetUserByIdAsync(string? token, int id)
        {
            var requestUri = $"https://api.worksnaps.com:443/api/users/{id}.xml";
            var doc = await GetXmlResponseAsync(requestUri, token);
            var user = doc.Root!.ParseXML<WorksnapsUserDTO>();

            if (user == null)
                throw new NotFoundException($"No user with this id was found. Id: {id}");

            return user;
        }

        public async Task<List<ProjectDTO>> GetProjectsAsync(string? token)
        {
            var requestUri = "https://api.worksnaps.com:443/api/projects.xml";
            var doc = await GetXmlResponseAsync(requestUri, token);
            var data = new List<ProjectDTO>();
            foreach (var element in doc.Root!.Elements())
            {
                var item = element.ParseXML<ProjectDTO>();

                data.Add(item);
            }

            return data;
        }

        public async Task<List<AssignmentDTO>> GetUserAssignmentAsync(string? token, string projectIds)
        {
            var requestUri = $"https://api.worksnaps.com:443/api/projects/{projectIds}/user_assignments.xml";
            var doc = await GetXmlResponseAsync(requestUri, token);
            var data = new List<AssignmentDTO>();
            foreach (var element in doc.Root!.Elements())
            {
                var item = element.ParseXML<AssignmentDTO>();

                data.Add(item);
            }

            return data;
        }

        public async Task<List<TimeEntryDTO>> GetTimeEntriesAsync(string? token, string projectId, string userId, long fromTimestamp, long toTimestamp)
        {
            var requestUri = $"https://api.worksnaps.com:443/api/projects/{projectId}/time_entries.xml?user_ids={userId}&from_timestamp={fromTimestamp}&to_timestamp={toTimestamp}";
            var doc = await GetXmlResponseAsync(requestUri, token);
            var data = new List<TimeEntryDTO>();
            foreach (var element in doc.Root!.Elements())
            {
                var item = element.ParseXML<TimeEntryDTO>();

                data.Add(item);
            }

            return data;
        }
    }
}
