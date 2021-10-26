﻿using CodeModel;
using CodeModel.CaDETModel;
using CodeModel.CaDETModel.CodeItems;
using DataSetExplorer.DataSetBuilder.Model;
using DataSetExplorer.DataSetBuilder.Model.Repository;
using DataSetExplorer.DataSetSerializer;
using FluentResults;
using System.Collections.Generic;
using System.IO;

namespace DataSetExplorer
{
    public class DataSetAnalysisService : IDataSetAnalysisService
    {
        private readonly IDataSetProjectRepository _dataSetProjectRepository;
        public DataSetAnalysisService(IDataSetProjectRepository dataSetProjectRepository)
        {
            _dataSetProjectRepository = dataSetProjectRepository;
        }

        public Result<string> FindInstancesWithAllDisagreeingAnnotations(IDictionary<string, string> projects)
        {
            try
            {
                foreach (var projectFolderPath in projects.Keys)
                {
                    var project = LoadDataSetProject(projectFolderPath, projectFolderPath);
                    var exporter = new TextFileExporter(projects[projectFolderPath]);
                    exporter.ExportInstancesWithAnnotatorId(project.GetInstancesWithAllDisagreeingAnnotations());
                }
                return Result.Ok("Instances with disagreeing annotations exported.");
            } catch (IOException e)
            {
                return Result.Fail(e.ToString());
            }
        }

        public Result<string> FindInstancesRequiringAdditionalAnnotation(IDictionary<string, string> projects)
        {
            try
            {
                foreach (var projectFolderPath in projects.Keys)
                {
                    var project = LoadDataSetProject(projectFolderPath, projectFolderPath);
                    var exporter = new TextFileExporter(projects[projectFolderPath]);
                    exporter.ExportInstancesWithAnnotatorId(project.GetInsufficientlyAnnotatedInstances());
                }
                return Result.Ok("Instances requiring additional annotation exported.");
            } catch (IOException e)
            {
                return Result.Fail(e.ToString());
            }
        }

        public Result<List<SmellCandidateInstances>> FindInstancesWithAllDisagreeingAnnotations(IEnumerable<int> projectIds)
        {
            var instances = new List<SmellCandidateInstances>();
            var projects = _dataSetProjectRepository.GetDataSetProjects(projectIds);
            foreach (var project in projects) instances.AddRange(project.GetInstancesWithAllDisagreeingAnnotations());
            
            return Result.Ok(instances);
        }

        public Result<List<SmellCandidateInstances>> FindInstancesRequiringAdditionalAnnotation(IEnumerable<int> projectIds)
        {
            var instances = new List<SmellCandidateInstances>();
            var projects = _dataSetProjectRepository.GetDataSetProjects(projectIds);
            foreach (var project in projects) instances.AddRange(project.GetInsufficientlyAnnotatedInstances());
            
            return Result.Ok(instances);
        }

        private DataSetProject LoadDataSetProject(string folder, string projectName)
        {
            var importer = new ExcelImporter(folder);
            return importer.Import(projectName);
        }

        private List<Instance> LoadAnnotatedInstances(string datasetPath)
        {
            var importer = new ExcelImporter(datasetPath);
            return importer.ImportAnnotatedInstancesFromDataSet(datasetPath);
        }

        public Result<string> ExportMembersFromAnnotatedClasses(IDictionary<string, string> projects, string datasetPath, string outputFolder)
        {
            CodeModelFactory factory = new CodeModelFactory();

            try
            {
                var classesGroupedBySeverity = new Dictionary<int, List<CaDETClass>>();
                var annotatedInstances = LoadAnnotatedInstances(datasetPath);

                foreach (var projectUrl in projects.Keys)
                {
                    CaDETProject cadetProject = factory.CreateProjectWithCodeFileLinks(projects[projectUrl]);
                    GroupInstancesBySeverity(classesGroupedBySeverity, annotatedInstances, cadetProject);
                }

                var exporter = new TextFileExporter(outputFolder);
                exporter.ExportMembersFromAnnotatedClasses(classesGroupedBySeverity, annotatedInstances);
                return Result.Ok("Members from smelly classes exported.");
            }
            catch (IOException e)
            {
                return Result.Fail(e.ToString());
            }
        }

        private static void GroupInstancesBySeverity(Dictionary<int, List<CaDETClass>> classesGroupedBySeverity, List<Instance> annotatedInstances, CaDETProject cadetProject)
        {
            foreach (var instance in annotatedInstances)
            {
                var classForExport = cadetProject.Classes.Find(c => c.FullName.Equals(instance.CodeSnippetId));
                if (classForExport != null)
                {
                    var finalAnnotation = instance.GetFinalAnnotation();
                    if (!classesGroupedBySeverity.ContainsKey(finalAnnotation)) classesGroupedBySeverity.Add(finalAnnotation, new List<CaDETClass>());
                    classesGroupedBySeverity[finalAnnotation].Add(classForExport);
                }
            }
        }
    }
}
