using System.Net.Http.Headers;
using System.Xml.Linq;
using McgTgBotNet.DTOs;
using McgTgBotNet.Extensions;
using AutoMapper;
using McgTgBotNet.Models;
using System.Text.RegularExpressions;
using ReportBot.Services.Services.Interfaces;
using ReportBot.DataBase.Repositories.Interfaces;
using McgTgBotNet.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace McgTgBotNet.Services
{
    public class WorksnapsService : IWorksnapsService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;


        public WorksnapsService(
            IRepository<User> userRepository,
            IRepository<Project> projectRepository,
            IMapper mapper)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("MCCNm0JhBxAAbhsl4CvrV3ljBVtFVrYlcGATKhFX")));
            _mapper = mapper;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        public async Task<Dictionary<int, bool>> GetSummaryReportsAsync()
        {
            var today = DateTime.Today.Date.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"https://api.worksnaps.com:443/api/summary_reports?from_date={today}&to_date={today}&name=manager_report");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var doc = XDocument.Parse(content);

            var data = new List<SummaryReportDTO>();
            foreach (var element in doc.Root!.Elements())
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
                var user = _userRepository.FirstOrDefault(x => x.WorksnapsId == item.UserId);

                if (user == null)
                    continue;

                if (item.DurationInMinutes >= user.ShiftTime)
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

        public async Task<int> GetUserId(string email)
        {
            if (Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$") == false)
                throw new ArgumentException("Email is not valid");

            var response = await _httpClient.GetAsync($"https://api.worksnaps.com:443/api/users.xml");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var doc = XDocument.Parse(content);

            var data = new List<UserDTO>();

            foreach (var element in doc.Root!.Elements())
            {
                var item = element.ParseXML<UserDTO>();

                data.Add(item);
            }

            var user = data.FirstOrDefault(x => x.Email == email);

            if (user == null)
                throw new ArgumentException($"😔 No user with this email was found. Email: {email}");

            return user.Id;
        }

        public async Task<UserDTO> GetUserByWorksnapsId(int id)
        {
            var response = await _httpClient.GetAsync($"https://api.worksnaps.com:443/api/users/{id}.xml");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var doc = XDocument.Parse(content);

            var user = doc.Root!.ParseXML<UserDTO>();

            if (user == null)
                throw new ArgumentException($"No user with this id was found. Id: {id}");

            return user;
        }

        public async Task<bool> AddProjectToUser(int userId)
        {
            var userWorksnaps = await GetUserByWorksnapsId(userId);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(userWorksnaps.ApiToken)));

            var response = await _httpClient.GetAsync($"https://api.worksnaps.com:443/api/projects.xml");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var doc = XDocument.Parse(content);

            var data = new List<ProjectDTO>();

            foreach (var element in doc.Root!.Elements())
            {
                var item = element.ParseXML<ProjectDTO>();

                data.Add(item);
            }

            var projects = new List<Project>();

            foreach(var item in _mapper.Map<List<Project>>(data))
            {
                var project = await _projectRepository.FirstOrDefaultAsync(x => x.Name == item.Name);
                if (project == null)
                {
                    await _projectRepository.InsertAsync(item);
                    projects.Add(item);
                }
                else
                {
                    projects.Add(project);
                }
            }
            var user = await _userRepository.FirstOrDefaultAsync(x => x.WorksnapsId == userId)
                ?? throw new Exception("User not found");

            user.Projects.AddRange(projects);

            var result = await _userRepository.UpdateAsync(user);

            return result;
        }
    }
}