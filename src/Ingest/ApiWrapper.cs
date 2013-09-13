using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SC2Balance.Models;
using System.Net.Http;

namespace SC2Balance.Ingest
{
    public class ApiWrapper
    {
        private HttpClient GetClient(LadderRegion region)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(String.Format(@"http://{0}.battle.net/api/sc2/", region.RegionCode()), UriKind.Absolute)
            };
        }

        public async Task<IEnumerable<LadderMember>> GetGrandmasterMembers(LadderRegion region)
        {
            var client = GetClient(region);
            var response = await client.GetAsync(new Uri("ladder/grandmaster", UriKind.Relative));
            if (!response.IsSuccessStatusCode)
            {
                //TODO: Logging
                return new List<LadderMember>();
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var objects = await JsonConvert.DeserializeObjectAsync<Ladder>(responseString);

            return objects.LadderMembers;
        }

        public async Task<IEnumerable<Match>> GetRecentMatchesForPlayer(string profilePath, LadderRegion region)
        {
            var client = GetClient(region);
            var responseMessage = await client.GetAsync(String.Format(@"{0}{1}", profilePath, @"matches").TrimStart('/'));
            if (!responseMessage.IsSuccessStatusCode)
            {
                //TODO: Logging
                return new List<Match>();
            }
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            
            var objects = await JsonConvert.DeserializeObjectAsync<MatchHistory>(responseString);

            return objects.Matches;
        }
    }
}
