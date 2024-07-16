using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using McgTgBotNet.DTOs;
using McgTgBotNet.Extensions;

namespace McgTgBotNet.Services
{
    public class WorksnapsService
    {
        private readonly HttpClient _httpClient;

        public WorksnapsService(string token)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(token)));
        }

        public async Task<Dictionary<int, bool>> GetTimeEntryAsync()
        {
            var today = DateTime.Today.Date.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"https://api.worksnaps.com:443/api/summary_reports?from_date={today}&to_date={today}&name=manager_report");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var doc = XDocument.Parse(content);

            var data = new List<SummaryReportDTO>();
            foreach (var element in doc.Root.Elements())
            {
                var item = element.ParseXML<SummaryReportDTO>();

                data.Add(item);
            }

            return IsSessionFinished(data);
        }

        private Dictionary<int, bool> IsSessionFinished(List<SummaryReportDTO> data)
        {
            var usersIsFinished = new Dictionary<int, bool>();

            foreach (var item in data)
            {
                if (item.DurationInMinutes > 30)
                {
                    usersIsFinished.Add(item.UserId, true);
                }
                else
                {
                    usersIsFinished.Add(item.UserId, false);
                }
            }

            return usersIsFinished;
        }

        public async Task<bool> AddProject()
        {
            var response = await _httpClient.GetAsync($"https://api.worksnaps.com:443/api/projects.xml");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var doc = XDocument.Parse(content);

            var data = new List<ProjectDTO>();

            foreach (var element in doc.Root.Elements())
            {
                var item = element.ParseXML<ProjectDTO>();

                data.Add(item);
            }

            return true;
        }
    }
}
