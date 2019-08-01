/*---------------------------------------------------------------------------------------------
 *  Copyright (c) The International Federation of Red Cross and Red Crescent Societies. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Dolittle.Events.Processing;
using Dolittle.Logging;
using Dolittle.ReadModels;
using Events.Reporting.CaseReports;
using Dolittle.Runtime.Events;
using Read.Management.DataCollectors;
using Read.Reporting.CaseReportsForListing;
using Read.Reporting.HealthRisks;

namespace Read.Reporting.CaseReports
{
    public class EventProcessor : ICanProcessEvents
    {
        readonly ILogger _logger;
        readonly IReadModelRepositoryFor<CaseReport> _caseReports;
        readonly IReadModelRepositoryFor<CaseReportFromUnknownDataCollector> _caseReportsFromUnknownDataCollectors;
        readonly IReadModelRepositoryFor<TrainingReport> _trainingReports;
        readonly IReadModelRepositoryFor<DataCollector> _dataCollectors;
        readonly IReadModelRepositoryFor<HealthRisk> _healthRisks;

        public EventProcessor(
            ILogger logger,
            IReadModelRepositoryFor<CaseReport> caseReports,
            IReadModelRepositoryFor<CaseReportFromUnknownDataCollector> caseReportsFromUnknownDataCollectors,
            IReadModelRepositoryFor<TrainingReport> trainingReports,
            IReadModelRepositoryFor<DataCollector> dataCollectors,
            IReadModelRepositoryFor<HealthRisk> healthRisks)
        {
            _logger = logger;
            _caseReports = caseReports;
            _caseReportsFromUnknownDataCollectors = caseReportsFromUnknownDataCollectors;
            _trainingReports = trainingReports;
            _dataCollectors = dataCollectors;
            _healthRisks = healthRisks;
        }
        [EventProcessor("5d3c679a-bd2e-4bd3-92f9-903fcdbda064")]
        public void Process(CaseReportReceived @event, EventSourceId caseReportId)
        {
            var dataCollector = _dataCollectors.GetById(@event.DataCollectorId);
            if (dataCollector.InTraining)
            {
                var healthRisk = _healthRisks.GetById(@event.HealthRiskId);
                var caseReport = new TrainingReport(caseReportId.Value)
                {
                    Status = CaseReportStatus.Success,
                    Message = @event.Message,
                    DataCollectorId = dataCollector.Id,
                    DataCollectorDisplayName = dataCollector.DisplayName,
                    DataCollectorDistrict = dataCollector.District,
                    DataCollectorRegion = dataCollector.Region,
                    DataCollectorVillage = dataCollector.Village,
                    Location = dataCollector.Location,
                    Origin = @event.Origin,

                    HealthRiskId = healthRisk.Id,
                    HealthRisk = healthRisk.Name,

                    NumberOfMalesUnder5 = @event.NumberOfMalesUnder5,
                    NumberOfMalesAged5AndOlder = @event.NumberOfMalesAged5AndOlder,
                    NumberOfFemalesUnder5 = @event.NumberOfFemalesUnder5,
                    NumberOfFemalesAged5AndOlder = @event.NumberOfFemalesAged5AndOlder,
                    Timestamp = @event.Timestamp,
                };

                _trainingReports.Insert(caseReport);
            }
        }
    }
}
