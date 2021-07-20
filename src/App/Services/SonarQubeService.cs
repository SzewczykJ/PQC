using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using App.Data.Entities;
using App.Data.IDataRepository;
using App.Models;
using App.Services.IServices;
using File = App.Data.Entities.File;

namespace App.Services
{
    public class SonarQubeService : ISonarQubeService
    {
        private readonly IFileDetailRepo fileDetailRepo;
        private readonly IFileRepo fileRepo;
        private readonly ILanguageRepo languageRepo;
        private readonly ISonarQubeClient sonarQubeClient;

        public SonarQubeService(ISonarQubeClient sonarQubeClient,
            IFileRepo fileRepo,
            IFileDetailRepo fileDetailRepo,
            ILanguageRepo languageRepo)
        {
            this.sonarQubeClient = sonarQubeClient;
            this.fileRepo = fileRepo;
            this.languageRepo = languageRepo;
            this.fileDetailRepo = fileDetailRepo;
        }

        public async Task SaveDataFromRepositoryAsync(string projectName,
            Commit sonarCommit,
            Dictionary<string, CommitChanges> commitChanges)
        {
            Thread.Sleep(5000);
            var projectTree = await this.sonarQubeClient.GetProjectTree(projectName);

            await this.SaveFileMetricAsync(projectTree, sonarCommit, commitChanges);
        }

        private async Task SaveFileMetricAsync(ProjectTree projectTree,
            Commit sonarCommit,
            Dictionary<string, CommitChanges> commitChanges)
        {
            foreach (var component in projectTree.Components)
            {
                if (component.Path != null && commitChanges.ContainsKey(component.Path))
                {
                    if (component.Qualifier.Equals("FIL") || component.Qualifier.Equals("UTS"))
                    {
                        var root = await this.sonarQubeClient.GetMetricsForFile(component.Key);
                        var metric = this.MapMetric(root.Component.Measures);
                        var fileDetail = this.fileDetailRepo.FindByPath(component.Path);

                        if (fileDetail == null)
                        {
                            fileDetail = new FileDetail
                            {
                                Name = component.Name,
                                FullPath = component.Path,
                                Language = this.GetLanguage(component.Language)
                            };
                        }

                        var file = new File
                        {
                            SHA = commitChanges.GetValueOrDefault(component.Path).SHA,
                            FileDetail = fileDetail,
                            Metric = metric,
                            Commit = sonarCommit,
                            Status = commitChanges.GetValueOrDefault(component.Path).Status.ToString()
                        };

                        this.fileRepo.Add(file);
                    }
                }
            }
        }

        private Metric MapMetric(List<Measure> measures)
        {
            var metric = new Metric();
            metric.Date = DateTime.UtcNow;

            foreach (var measure in measures)
            {
                switch (measure.Metric)
                {
                    case "complexity":
                        metric.Complexity = int.Parse(measure.Value);
                        break;
                    case "cognitive_complexity":
                        metric.CognitiveComplexity = int.Parse(measure.Value);
                        break;
                    case "duplicated_lines":
                        metric.DuplicatedLines = int.Parse(measure.Value);
                        break;
                    case "code_smells":
                        metric.CodeSmells = int.Parse(measure.Value);
                        break;
                    case "new_code_smells":
                        metric.NewCodeSmells = measure.Value == null ? null : int.Parse(measure.Value);
                        break;
                    case "comment_lines":
                        metric.CommentLines = int.Parse(measure.Value);
                        break;
                    case "comment_lines_density":
                        metric.CommentLinesDensity = double.Parse(measure.Value, CultureInfo.InvariantCulture);
                        break;
                    case "ncloc":
                        metric.Ncloc = int.Parse(measure.Value);
                        break;
                    case "statements":
                        metric.Statements = int.Parse(measure.Value);
                        break;
                    case "branch_coverage":
                        metric.BranchCoverage = double.Parse(measure.Value, CultureInfo.InvariantCulture);
                        break;
                    case "line_coverage":
                        metric.LineCoverage = double.Parse(measure.Value, CultureInfo.InvariantCulture);
                        break;
                }
            }

            return metric;
        }

        private Language GetLanguage(string sonarLanguage)
        {
            switch (sonarLanguage)
            {
                case "java":
                    return this.languageRepo.FindByName("Java");
                case "cs":
                    return this.languageRepo.FindByName("C#");
                case "php":
                    return this.languageRepo.FindByName("PHP");
                case "js":
                    return this.languageRepo.FindByName("JavaScript");
                case "ts":
                    return this.languageRepo.FindByName("TypeScript");
                case "kotlin":
                    return this.languageRepo.FindByName("Kotlin");
                case "ruby":
                    return this.languageRepo.FindByName("Ruby");
                case "go":
                    return this.languageRepo.FindByName("Go");
                case "scala":
                    return this.languageRepo.FindByName("Scala");
                case "flex":
                    return this.languageRepo.FindByName("Flex");
                case "py":
                    return this.languageRepo.FindByName("Python");
                case "web":
                    return this.languageRepo.FindByName("HTML");
                case "css":
                    return this.languageRepo.FindByName("CSS");
                case "xml":
                    return this.languageRepo.FindByName("XML");
                case "vbnet":
                    return this.languageRepo.FindByName("VB.NET");
                default:
                    return this.languageRepo.FindByName("Other");
            }
        }
    }
}
