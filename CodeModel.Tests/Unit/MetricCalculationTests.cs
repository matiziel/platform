﻿using CodeModel.CaDETModel.CodeItems;
using CodeModel.Tests.DataFactories;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CodeModel.Tests.Unit
{
    public class MetricCalculationTests
    {
        private static readonly CodeFactory _testDataFactory = new CodeFactory();

        [Fact]
        public void Calculates_lines_of_code_for_CSharp_class_elements()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetDoctorClassText()).Classes;

            var doctorClass = classes.First();
            doctorClass.Metrics[CaDETMetric.CLOC].ShouldBe(22);
            doctorClass.FindMember("Email").Metrics[CaDETMetric.MLOC].ShouldBe(1);
            doctorClass.FindMember("IsAvailable").Metrics[CaDETMetric.MLOC].ShouldBe(8);
        }

        [Fact]
        public void Calculates_weighted_methods_per_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetGitAdapterClassText()).Classes;

            var gitClass = classes.First();
            gitClass.Metrics[CaDETMetric.WMC].ShouldBe(17);
        }


        [Fact]
        public void Calculates_access_to_foreign_data()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetATFDMultipleClassTexts()).Classes;

            var class1 = classes.Find(c => c.Name.Equals("Class1"));
            var class3 = classes.Find(c => c.Name.Equals("Class3"));
            var class5 = classes.Find(c => c.Name.Equals("Class5"));
            var class7 = classes.Find(c => c.Name.Equals("Class7"));
            var class9 = classes.Find(c => c.Name.Equals("Class9"));

            class1.Metrics[CaDETMetric.ATFD].ShouldBe(2);
            class3.Metrics[CaDETMetric.ATFD].ShouldBe(1);
            class5.Metrics[CaDETMetric.ATFD].ShouldBe(1);
            class7.Metrics[CaDETMetric.ATFD].ShouldBe(3);
            class9.Metrics[CaDETMetric.ATFD].ShouldBe(2);
        }

        [Fact]
        public void Calculates_number_of_return_statements_in_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.Metrics[CaDETMetric.CNOR].ShouldBe(3);
        }

        [Fact]
        public void Calculates_number_of_loops_in_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.Metrics[CaDETMetric.CNOL].ShouldBe(7);
        }

        [Fact]
        public void Calculates_number_of_comparisons_in_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.Metrics[CaDETMetric.CNOC].ShouldBe(7);
        }

        [Fact]
        public void Calculates_number_of_assignments_in_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.Metrics[CaDETMetric.CNOA].ShouldBe(2);
        }

        [Fact]
        public void Calculates_number_of_private_methods_in_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.Metrics[CaDETMetric.NOPM].ShouldBe(3);
        }

        [Fact]
        public void Calculates_number_of_protected_fields_in_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.Metrics[CaDETMetric.NOPF].ShouldBe(2);
        }
        
        [Fact]
        public void Calculates_max_nested_blocks_in_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.Metrics[CaDETMetric.CMNB].ShouldBe(4);
        }

        [Fact]
        public void Calculates_number_of_dependencies_in_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetMultipleClassTexts()).Classes;

            var dataRange = classes.Find(c => c.Name.Equals("DateRange"));
            var doctor = classes.Find(c => c.Name.Equals("Doctor"));
            var doctorService = classes.Find(c => c.Name.Equals("DoctorService"));

            dataRange.Metrics[CaDETMetric.CBO].ShouldBe(2);
            doctor.Metrics[CaDETMetric.CBO].ShouldBe(2);
            doctorService.Metrics[CaDETMetric.CBO].ShouldBe(2);
        }

        [Theory]
        [MemberData(nameof(DITTest))]
        public void Calculates_number_of_hierarchy_levels(IEnumerable<string> classCode, string className, double ditMetric)
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(classCode).Classes;

            var classToEvaluate = classes.Find(c => c.Name.Equals(className));
            classToEvaluate.Metrics[CaDETMetric.DIT].ShouldBe(ditMetric);
        }

        public static IEnumerable<object[]> DITTest =>

            new List<object[]>
            {
                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/Level.txt"),
                    "Level",
                    0
                },

                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/AsepriteReader.txt"),
                    "AsepriteReader",
                    0
                },

                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/AsepriteFile.txt"),
                    "AsepriteFile",
                    0
                },

                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/AsepriteWriter.txt"),
                    "AsepriteWriter",
                    0
                },

                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/AudioWriter.txt"),
                    "AudioWriter",
                    0
                }
            };

        [Theory]
        [MemberData(nameof(DCCTest))]
        public void Calculates_class_coupling(IEnumerable<string> classCode, string className, double dccMetric)
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(classCode).Classes;

            var classToEvaluate = classes.Find(c => c.Name.Equals(className));
            classToEvaluate.Metrics[CaDETMetric.DCC].ShouldBe(dccMetric);
        }

        public static IEnumerable<object[]> DCCTest =>

            new List<object[]>
            {
                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/Level.txt"),
                    "Level",
                    0
                },

                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/AsepriteReader.txt"),
                    "AsepriteReader",
                    0
                },

                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/AsepriteFile.txt"),
                    "AsepriteFile",
                    0
                },

                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/AsepriteWriter.txt"),
                    "AsepriteWriter",
                    0
                },

                new object[]
                {
                    _testDataFactory.readClassFromFile("../../../DataFactories/TestClasses/SmellyClasses/AudioWriter.txt"),
                    "AudioWriter",
                    0
                }
            };

        [Fact]
        public void Calculates_invoked_methods_in_a_class()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetMultipleClassTexts()).Classes;

            var dateRange = classes.Find(c => c.Name.Equals("DateRange"));
            var service = classes.Find(c => c.Name.Equals("DoctorService"));

            dateRange.Metrics[CaDETMetric.RFC].ShouldBe(0);
            service.Metrics[CaDETMetric.RFC].ShouldBe(4);
        }

        [Fact]
        public void Calculates_method_cyclomatic_complexity()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetGitAdapterClassText()).Classes;

            var gitClass = classes.First();

            gitClass.FindMember("CheckoutCommit").Metrics[CaDETMetric.CYCLO].ShouldBe(2);
            gitClass.FindMember("ParseDocuments").Metrics[CaDETMetric.CYCLO].ShouldBe(4);
        }

        [Fact]
        public void Calculates_member_effective_lines_of_code()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetEffectiveLinesOfCodeTest()).Classes;

            var doctor = classes.First();
            doctor.FindMember("Doctor").Metrics[CaDETMetric.MELOC].ShouldBe(1);
            doctor.FindMember("IsAvailable").Metrics[CaDETMetric.MELOC].ShouldBe(2);
        }

        [Fact]
        public void Calculates_number_of_parameters()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetGitAdapterClassText()).Classes;

            var gitClass = classes.First();
            gitClass.FindMember("CheckForNewCommits").Metrics[CaDETMetric.NOP].ShouldBe(0);
            gitClass.FindMember("PullChanges").Metrics[CaDETMetric.NOP].ShouldBe(0);
            gitClass.FindMember("GetCommits").Metrics[CaDETMetric.NOP].ShouldBe(1);
            gitClass.FindMember("CheckoutCommit").Metrics[CaDETMetric.NOP].ShouldBe(1);
        }

        [Fact]
        public void Calculates_number_of_local_variables()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetGitAdapterClassText()).Classes;

            var gitClass = classes.First();
            gitClass.FindMember("CheckForNewCommits").Metrics[CaDETMetric.NOLV].ShouldBe(2);
            gitClass.FindMember("GetActiveCommit").Metrics[CaDETMetric.NOLV].ShouldBe(0);
        }

        [Fact]
        public void Calculates_number_of_try_catch_blocks()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.NOTC].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders1").Metrics[CaDETMetric.NOTC].ShouldBe(1);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.NOTC].ShouldBe(2);
            firstClass.FindMember("CreateClassMemberBuilders3").Metrics[CaDETMetric.NOTC].ShouldBe(3);
        }

        [Fact]
        public void Calculates_number_of_loops()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.MNOL].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders1").Metrics[CaDETMetric.MNOL].ShouldBe(1);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.MNOL].ShouldBe(2);
            firstClass.FindMember("CreateClassMemberBuilders3").Metrics[CaDETMetric.MNOL].ShouldBe(4);
        }

        [Fact]
        public void Calculates_number_of_return_statements()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.MNOR].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders1").Metrics[CaDETMetric.MNOR].ShouldBe(1);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.MNOR].ShouldBe(2);
        }

        [Fact]
        public void Calculates_number_of_comparisons()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.MNOC].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.MNOC].ShouldBe(4);
            firstClass.FindMember("CreateClassMemberBuilders3").Metrics[CaDETMetric.MNOC].ShouldBe(3);
        }

        [Fact]
        public void Calculates_number_of_assignments()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.MNOA].ShouldBe(2);
            firstClass.FindMember("CreateClassMemberBuilders1").Metrics[CaDETMetric.MNOA].ShouldBe(0);
        }

        [Fact]
        public void Calculates_number_of_numeric_literals()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.NONL].ShouldBe(4);
            firstClass.FindMember("CreateClassMemberBuilders1").Metrics[CaDETMetric.NONL].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.NONL].ShouldBe(12);
        }

        [Fact]
        public void Calculates_number_of_string_literals()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.NOSL].ShouldBe(1);
            firstClass.FindMember("CreateClassMemberBuilders1").Metrics[CaDETMetric.NOSL].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.NOSL].ShouldBe(2);
        }

        [Fact]
        public void Calculates_number_of_math_operations()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.NOMO].ShouldBe(2);
            firstClass.FindMember("CreateClassMemberBuilders1").Metrics[CaDETMetric.NOMO].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.NOMO].ShouldBe(6);
        }

        [Fact]
        public void Calculates_number_of_parenthesized_expressions()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.NOPE].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.NOPE].ShouldBe(2);
        }

        [Fact]
        public void Calculates_number_of_lambda_expressions()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.NOLE].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.NOLE].ShouldBe(2);
        }

        [Fact]
        public void Calculates_max_nested_blocks()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var firstClass = classes.First();
            firstClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.MMNB].ShouldBe(0);
            firstClass.FindMember("CreateClassMemberBuilders1").Metrics[CaDETMetric.MMNB].ShouldBe(2);
            firstClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.MMNB].ShouldBe(4);
            firstClass.FindMember("CreateClassMemberBuilders3").Metrics[CaDETMetric.MMNB].ShouldBe(4);
        }

        [Fact]
        public void Calculates_number_of_unique_words()
        {
            CodeModelFactory factory = new CodeModelFactory();

            List<CaDETClass> classes = factory.CreateProject(_testDataFactory.GetCodeBlocksClass()).Classes;

            var gitClass = classes.First();
            gitClass.FindMember("CSharpCodeParserInit").Metrics[CaDETMetric.NOUW].ShouldBe(5);
            gitClass.FindMember("CreateClassMemberBuilders1").Metrics[CaDETMetric.NOUW].ShouldBe(22);
            gitClass.FindMember("CreateClassMemberBuilders2").Metrics[CaDETMetric.NOUW].ShouldBe(33);
        }
    }
}
