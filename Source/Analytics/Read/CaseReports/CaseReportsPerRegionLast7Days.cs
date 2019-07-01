using Concepts;
using Dolittle.ReadModels;
using System.Collections.Generic;

namespace Read.CaseReports
{
    public class CaseReportsPerRegionLast7Days : IReadModel
    {
        public Day Id { get; set; }
        public IList<CaseReportsInRegionLast7Days> CaseReportsPerRegion { get; set; }
    }
}