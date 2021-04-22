﻿using System;
using System.Collections.Generic;
using System.Linq;
using RepositoryCompiler.CodeModel.CaDETModel.CodeItems;
using SmellDetector.Controllers;
using SmellDetector.SmellModel;
using SmellDetector.SmellModel.Reports;

namespace SmellDetector.Detectors.RuleEngines
{
    internal class ClassMetricRuleEngine : IDetector
    {
        private readonly List<Rule> _rules;
        private readonly List<Rule> _dynamicRules;
        public ClassMetricRuleEngine()
        {
            Rule rule1 = new Rule("10.1109/WCRE.2005.15",
                                  new AndCriteria(
                                            new AndCriteria(
                                                new MetricCriteria(CaDETMetric.ATFD , OperationEnum.GREATER_THAN, 2),
                                                new MetricCriteria(CaDETMetric.WMC, OperationEnum.GREATER_THAN, 47)),
                                            new MetricCriteria(CaDETMetric.TCC, OperationEnum.LESS_THAN, 0.33)),
                                  SmellType.GOD_CLASS);
            Rule rule2 = new Rule("10.1109/SCAM.2013.6648192",
                                  new OrCriteria(
                                      new MetricCriteria(CaDETMetric.CLOC, OperationEnum.GREATER_THAN, 750),
                                      new MetricCriteria(CaDETMetric.NMD_NAD, OperationEnum.GREATER_THAN, 20)
                                      ),
                                  SmellType.GOD_CLASS);
            Rule rule3 = new Rule("10.1109/MSR.2007.21",
                                  new OrCriteria(
                                            new MetricCriteria(CaDETMetric.NMD, OperationEnum.GREATER_THAN, 15),
                                            new MetricCriteria(CaDETMetric.NAD, OperationEnum.GREATER_THAN, 15)),
                                  SmellType.GOD_CLASS);
            _rules = new List<Rule>();
            _rules.Add(rule1);
            _rules.Add(rule2);
            _rules.Add(rule3);
            _dynamicRules = new List<Rule>();
        }

        private void createDynamicRules(List<CaDETClassDTO> caDetClassDtoList)
        {
            int indexOfSignificativeMetricValue = calculateIndexBasedOnPercentage(caDetClassDtoList, 20);
            double atfdThreshold = calculateDynamicThreshold(caDetClassDtoList, CaDETMetric.ATFD, indexOfSignificativeMetricValue);

            Rule rule1 = new Rule("10.1016/j.jss.2006.10.018",
                                new AndCriteria(new AndCriteria(new MetricCriteria(CaDETMetric.ATFD, OperationEnum.GREATER_OR_EQUALS, atfdThreshold),
                                                                new MetricCriteria(CaDETMetric.ATFD, OperationEnum.GREATER_THAN, 4)),
                                                new AndCriteria(new MetricCriteria(CaDETMetric.WMC, OperationEnum.GREATER_THAN, 20),
                                                                new MetricCriteria(CaDETMetric.TCC, OperationEnum.LESS_THAN, 0.33))),
                                SmellType.GOD_CLASS);

            double wmcThreshold = calculateDynamicThreshold(caDetClassDtoList, CaDETMetric.WMC, 10);

            Rule rule2 = new Rule("10.1109/TOOLS.2001.941671",
                                new OrCriteria(new OrCriteria(new MetricCriteria(CaDETMetric.ATFD, OperationEnum.GREATER_THAN, 3),
                                                                new MetricCriteria(CaDETMetric.WMC, OperationEnum.GREATER_THAN, wmcThreshold)),
                                                new MetricCriteria(CaDETMetric.TCC, OperationEnum.LESS_THAN, 0.33)),
                                SmellType.GOD_CLASS);
            _dynamicRules.Add(rule1);
            _dynamicRules.Add(rule2);

        }

        private double calculateDynamicThreshold(List<CaDETClassDTO> caDetClassDtoList, CaDETMetric metric, int indexOfMetricValue)
        {
            List<double> metricValues = caDetClassDtoList.Select(c => c.CodeSnippetMetrics[metric]).ToList();
            metricValues.Sort();
            return metricValues[indexOfMetricValue];
        }

        private int calculateIndexBasedOnPercentage(List<CaDETClassDTO> caDetClassDtoList, int percentage)
        {
            return caDetClassDtoList.Count * percentage / 100;
        }

        public PartialSmellDetectionReport FindIssues(List<CaDETClassDTO> caDetClassDtoList)
        {
            //createDynamicRules(caDetClassDtoList);
            PartialSmellDetectionReport partialReport = new PartialSmellDetectionReport();

            foreach(CaDETClassDTO caDETClassDTO in caDetClassDtoList)
            {
                List<Issue> issues = ApplyRule(caDETClassDTO);
                foreach (Issue issue in issues)
                {
                    if (issue != null)
                    {
                        partialReport.AddIssue(issue.CodeSnippetId, issue);
                    }
                }
            }
            return partialReport;
        }

        public PartialSmellDetectionReport FindIssues(CaDETClassDTO caDetClassDto)
        {
            throw new NotImplementedException();
        }

        private List<Issue> ApplyRule(CaDETClassDTO c)
        {
            List<Issue> issues = _rules.Select(r => r.Validate(c.FullName, c.CodeSnippetMetrics)).ToList();
            issues.AddRange(_dynamicRules.Select(r => r.Validate(c.FullName, c.CodeSnippetMetrics)).ToList());
            return issues;
        }
    }
}
