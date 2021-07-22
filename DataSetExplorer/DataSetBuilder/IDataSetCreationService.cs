﻿using DataSetExplorer.DataSetBuilder.Model;
using DataSetExplorer.DataSetSerializer.ViewModel;
using FluentResults;

namespace DataSetExplorer
{
    public interface IDataSetCreationService
    {
        public DataSet CreateDataSet(string basePath, string projectName, string projectAndCommitUrl);
        public Result<string> CreateDataSetSpreadsheet(string basePath, string projectName, NewSpreadSheetColumnModel columnModel, DataSet dataSet);
    }
}
