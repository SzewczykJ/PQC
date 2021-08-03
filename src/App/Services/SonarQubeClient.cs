using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;
using App.Services.IServices;
using Microsoft.Extensions.Configuration;

namespace App.Services
{
    public class SonarQubeClient : ISonarQubeClient
    {
        private static readonly string[] metricKeys =
        {
            "complexity", "cognitive_complexity", "duplicated_lines", "code_smells", "new_code_smells",
            "comment_lines", "comment_lines_density", "ncloc", "statements", "branch_coverage", "line_coverage"
        };

        private readonly byte[] credentials;

        private readonly HttpClient httpClient;

        private readonly string metricKeysParam = string.Join(",", metricKeys);


        public SonarQubeClient(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            var login = configuration["SonarQube:Login"];
            var password = configuration["SonarQube:Password"];
            this.credentials = Encoding.ASCII.GetBytes(login + ":" + password);
            this.httpClient.BaseAddress = new Uri(configuration["SonarQube:Api"]);
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(this.credentials));
        }

        public async Task<Root> GetMetricsForFile(string fileKey)
        {
            return await this.httpClient.GetFromJsonAsync<Root>(
                $"measures/component?component={fileKey}&metricKeys={this.metricKeysParam}");
        }

        public async Task<ProjectTree> GetProjectTree(string projectName)
        {
            return await this.httpClient.GetFromJsonAsync<ProjectTree>(
                $"components/tree?component={projectName}");
        }

        public async Task<Project> CreateProject(string projectName)
        {
            var parameters = new Dictionary<string, string> {["name"] = projectName, ["project"] = projectName};

            var httpResponseMessage =
                await this.httpClient.PostAsync("projects/create",
                    new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                return this.ExistingProject(projectName);
            }

            var tmp = await httpResponseMessage.Content.ReadAsStringAsync();

            return JsonSerializer
                .Deserialize<SonarQubeProject>(tmp, new JsonSerializerOptions {PropertyNameCaseInsensitive = true})
                .Project;
        }

        public async Task<HttpStatusCode> DeleteProject(string projectName)
        {
            var parameters = new Dictionary<string, string> {{"project", projectName}};

            var httpResponseMessage =
                await this.httpClient.PostAsync("projects/delete",
                    new FormUrlEncodedContent(parameters));
            return httpResponseMessage.StatusCode;
        }

        public async Task<ProjectToken> GenerateToken(string tokenName)
        {
            var parameters = new Dictionary<string, string> {{"name", tokenName}};


            var httpResponseMessage =
                await this.httpClient.PostAsync("user_tokens/generate",
                    new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                await this.RevokeToken(tokenName);
                httpResponseMessage =
                    await this.httpClient.PostAsync("user_tokens/generate",
                        new FormUrlEncodedContent(parameters));
            }

            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProjectToken>(response,
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        }

        public async Task<HttpStatusCode> RevokeToken(string tokenName)
        {
            var parameters = new Dictionary<string, string> {{"name", tokenName}};

            var httpResponseMessage =
                await this.httpClient.PostAsync("user_tokens/revoke",
                    new FormUrlEncodedContent(parameters));
            return httpResponseMessage.StatusCode;
        }

        private Project ExistingProject(string projectKey)
        {
            return new() {Key = projectKey, Name = projectKey};
        }
    }
}
