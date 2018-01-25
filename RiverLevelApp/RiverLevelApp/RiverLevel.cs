using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Web;

namespace RiverLevelApp
{


    public static class Helpers
    {
        public static async Task<string> GetResponse(string endpoint)
        {




            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(3);
                Task<string> downloadTask = httpClient.GetStringAsync(endpoint);
                return await downloadTask;

            }
        }

    }


    public static class Urls
    {

        public static string BaseUrl = "http://environment.data.gov.uk/flood-monitoring";
    }

    public class StationDetail
    {

        public static async Task<StationDetail> GetAsync(string stationid)
        {


            var url = $"{Urls.BaseUrl}/id/stations/{stationid}.json";
            var restext = await Helpers.GetResponse(url);

            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<StationDetail>(restext);
            return res;
        }

        public string context { get; set; }
        public Meta meta { get; set; }
        public Items items { get; set; }
        

        public class Meta
        {
            public string publisher { get; set; }
            public string licence { get; set; }
            public string documentation { get; set; }
            public string version { get; set; }
            public string comment { get; set; }
            public string[] hasFormat { get; set; }
        }

        public class Items
        {
            public string id { get; set; }
            public string RLOIid { get; set; }
            public string catchmentName { get; set; }
            public string dateOpened { get; set; }
            public Downstagescale downstageScale { get; set; }
            public string eaAreaName { get; set; }
            public string eaRegionName { get; set; }
            public int easting { get; set; }
            public string label { get; set; }
            public float lat { get; set; }
            public float _long { get; set; }
            public Measure measures { get; set; }
            public int northing { get; set; }
            public string notation { get; set; }
            public string riverName { get; set; }
            public Stagescale stageScale { get; set; }
            public string stationReference { get; set; }
            public string status { get; set; }
            public string town { get; set; }
            public string[] type { get; set; }
            public string wiskiID { get; set; }
        }

        public class Downstagescale
        {
            public string id { get; set; }
            public float datum { get; set; }
            public Highestrecent highestRecent { get; set; }
            public Maxonrecord maxOnRecord { get; set; }
            public Minonrecord minOnRecord { get; set; }
            public int scaleMax { get; set; }
            public float typicalRangeHigh { get; set; }
            public float typicalRangeLow { get; set; }
        }

        public class Highestrecent
        {
            public string id { get; set; }
            public DateTime dateTime { get; set; }
            public float value { get; set; }
        }

        public class Maxonrecord
        {
            public string id { get; set; }
            public DateTime dateTime { get; set; }
            public float value { get; set; }
        }

        public class Minonrecord
        {
            public string id { get; set; }
            public DateTime dateTime { get; set; }
            public float value { get; set; }
        }

        public class Stagescale
        {
            public string id { get; set; }
            public float datum { get; set; }
            public Level highestRecent { get; set; }
            public Level maxOnRecord { get; set; }
            public Level minOnRecord { get; set; }
            public int scaleMax { get; set; }
            public float typicalRangeHigh { get; set; }
            public float typicalRangeLow { get; set; }
        }




        public class Measure
        {
            public string id { get; set; }
            public string datumType { get; set; }
            public string label { get; set; }
            public Level latestReading { get; set; }
            public string notation { get; set; }
            public string parameter { get; set; }
            public string parameterName { get; set; }
            public int period { get; set; }
            public string qualifier { get; set; }
            public string station { get; set; }
            public string stationReference { get; set; }
            public string[] type { get; set; }
            public string unit { get; set; }
            public string unitName { get; set; }
            public string valueType { get; set; }
        }



    }


    public class Stations
    {


        public string context { get; set; }
        public Meta meta { get; set; }
        public Item[] items { get; set; }


        public class Meta
        {
            public string publisher { get; set; }
            public string licence { get; set; }
            public string documentation { get; set; }
            public string version { get; set; }
            public string comment { get; set; }
            public string[] hasFormat { get; set; }
        }

        public class Item
        {
            public string id { get; set; }
            public string RLOIid { get; set; }
            public string catchmentName { get; set; }
            public string dateOpened { get; set; }
            public int easting { get; set; }
            public string gridReference { get; set; }
            public string label { get; set; }
            public float lat { get; set; }
            public float _long { get; set; }
            public Measure[] measures { get; set; }
            public int northing { get; set; }
            public string notation { get; set; }
            public string riverName { get; set; }
            public string stageScale { get; set; }
            public string stationReference { get; set; }
            public string status { get; set; }
            public string town { get; set; }
            public string wiskiID { get; set; }
        }

        public class Measure
        {
            public string id { get; set; }
            public string parameter { get; set; }
            public string parameterName { get; set; }
            public int period { get; set; }
            public string qualifier { get; set; }
            public string unitName { get; set; }
        }



        public static async Task<Stations> GetAsync(string riverName)
        {


            var url = $"{Urls.BaseUrl}/id/stations?riverName={riverName.Replace(" ", "+")}";
            var restext = await Helpers.GetResponse(url);

            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<Stations>(restext);
            return res;
        }

        public string StationReference { get; set; }
        public string Label { get; set; }
    }

    public class RiverLevel
    {
        public static async Task<RiverLevel> GetAsync(string stationRef, bool latestOnly = false)
        {
            var query = latestOnly ? "latest" : $@"startdate={DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd")}&enddate={DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")}";
            var url = $@"http://environment.data.gov.uk/flood-monitoring/id/stations/{stationRef}/readings?{query}&parameter=level";
            var restext = await Helpers.GetResponse(url);
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<RiverLevel>(restext);
            return res;



        }



        public string context { get; set; }
        public Meta meta { get; set; }
        public Level[] items { get; set; }


        public class Meta
        {
            public string publisher { get; set; }
            public string licence { get; set; }
            public string documentation { get; set; }
            public string version { get; set; }
            public string comment { get; set; }
            public string[] hasFormat { get; set; }
            public int limit { get; set; }
        }



    }


    public class Level
    {
        public float changecm { get; set; }

        public string id { get; set; }
        public DateTime dateTime { get; set; }
        public string measure { get; set; }
        public float value { get; set; }

        public float valuecm => value * 100.0f;
    }
}
