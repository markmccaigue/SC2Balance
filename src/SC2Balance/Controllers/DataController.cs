using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SC2Balance.Models;
using SC2Balance.Ingest;

namespace SC2Balance.Controllers
{
    public class DataController : ApiController
    {
        //Rename the API that mentions a time span
        //Long term plan is to take in a param for this

        # region private

        private HttpResponseMessage BuildResponseFromJson(string json)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }

        private HttpResponseMessage getResultsForPostProcessingJob(String postProcessingJobType)
        {
            var json = String.Empty;
            using (var db = new DataContext())
            {
                json = db.PostProcessingOutputs.OrderByDescending(x => x.Id).First(x => x.PostProcessingJobType == postProcessingJobType).JsonResults;
            }

            return BuildResponseFromJson(json);
        }

        #endregion
 
        public HttpResponseMessage GetBalanceForSevenDays()
        {
            return getResultsForPostProcessingJob("BALANCE");
        }

        public HttpResponseMessage GetRaceBalanceForSevenDays()
        {
            return getResultsForPostProcessingJob("RACEBALANCE");
        }

        public HttpResponseMessage GetMapBalanceHistory()
        {
            return getResultsForPostProcessingJob("MAPBALANCEHISTORY");
        }

        public HttpResponseMessage GetBalanceHistory()
        {
            return getResultsForPostProcessingJob("BALANCEHISTORY");
        }

        public HttpResponseMessage GetMapRaceBalanceForSevenDays()
        {
            return getResultsForPostProcessingJob("MAPRACEBALANCE");
        }

        public HttpResponseMessage GetMapBalanceForSevenDays()
        {
            return getResultsForPostProcessingJob("MAPBALANCE");
        }

        public int GetHoursSinceUpdate()
        {
            using (var db = new DataContext())
            {
                var lastUpdated = db.ProcessingRuns.OrderByDescending(r => r.Id).First().DateTime;
                return (int)TimeSpan.FromTicks(DateTime.UtcNow.Ticks - lastUpdated.Ticks).TotalHours;
            }
        }

    }
         
}
//TODO: Convert magic strings to enum

