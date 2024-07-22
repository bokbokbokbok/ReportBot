using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using McgTgBotNet.DTOs;
using McgTgBotNet.Extensions;
using AutoMapper;
using McgTgBotNet.Models;
using McgTgBot.DB;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace McgTgBotNet.Services
{
    public class WorksnapsService
    {
        private readonly HttpClient _httpClient;
        private readonly Mapper _mapper;


        public WorksnapsService(string token)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(token)));
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<ProjectDTO, Project>()));
        }

        public async Task<Dictionary<int, bool>> GetSummaryReportsAsync()
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
                var user = DBContext.GetUserByWorksnapsId(item.UserId);

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

            foreach (var element in doc.Root.Elements())
            {
                var item = element.ParseXML<UserDTO>();

                data.Add(item);
            }

            var user = data.FirstOrDefault(x => x.Email == email);

            if (user == null)
                throw new ArgumentException($"😔 No user with this email was found. Email: {email}");

            return user.Id;
        }

        public async Task<UserDTO> GetUserById(int id)
        {
            var response = await _httpClient.GetAsync($"https://api.worksnaps.com:443/api/users/{id}.xml");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var doc = XDocument.Parse(content);

            var user = doc.Root.ParseXML<UserDTO>();

            if (user == null)
                throw new ArgumentException($"No user with this id was found. Id: {id}");

            return user;
        }

        public async Task<bool> AddProjectToUser(int userId)
        {
            var user = await GetUserById(userId);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(user.ApiToken)));

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

            var entity = _mapper.Map<List<Project>>(data);

            
            var projects = DBContext.InsertProjectMany(entity);

            var result = DBContext.AddUserToProjects(userId, projects.Select(x => x.Name).ToList());

            return result;
        }
    }
}