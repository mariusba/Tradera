using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog;
using Tradera.Models;
using Tradera.Models.ServiceResolvers;

namespace Tradera.Binance
{
    public class BinanceMapper : IMapper
    {
        public ExchangeName Name { get; set; } = ExchangeName.Binance;

        public List<ExchangeTicker> Process(string json)
        {
            var items = new List<ExchangeTicker>();
            try
            {
                if (json.Contains("}{"))
                {
                    foreach (var s in json.Split("}{"))
                    {
                        var mapped = Map(CleanUpAndDeserialize(s));
                        items.Add(mapped);
                    }
                }
                else
                {
                    var mapped = Map(CleanUpAndDeserialize(json));
                    items.Add(mapped);
                }

                return items;
            }
            catch (Exception e)
            {
                Log.Error("error deserializing json {e} for string {json}", e, json);
            }
            
            return items;
        }

        private static ExchangeTicker Map(BinanceResponse model)
        {
            return new()
            {
                Identifier = new ProcessorIdentifier(ExchangeName.Binance, model.s.ToLower()),
                EventTime = model.E,
                Price = model.p
            };
        }

        private static BinanceResponse CleanUpAndDeserialize(string s)
        {
            if (!s.StartsWith("{"))
            {
                s = "{" + s;
            }
            if (!s.EndsWith("}"))
            {
                s = s + "}";
            }
            return JsonConvert.DeserializeObject<BinanceResponse>(s);
        }

    }
}