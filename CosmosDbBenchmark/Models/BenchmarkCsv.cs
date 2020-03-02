using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbBenchmark.Models
{
    public class BenchmarkCsv
    {
        public string BlogType { get; set; }

        public string  Entitytype { get; set; }

        public string BenchmarkOperation { get; set; }

        public string DbOperation { get; set; }

        public string Size { get; set; }

        public double ConsumedRU { get; set; }

    }
}
