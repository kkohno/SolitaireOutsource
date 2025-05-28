using System;
using Newtonsoft.Json;

namespace Scripts.DecksDataBase.Services
{
    public sealed class DataBase
    {
        [JsonProperty("1")]
        public int[][] C1 { get; set; }
        [JsonProperty("2")]
        public int[][] C2 { get; set; }
        [JsonProperty("3")]
        public int[][] C3 { get; set; }
        [JsonProperty("4")]
        public int[][] C4 { get; set; }
        [JsonProperty("5")]
        public int[][] C5 { get; set; }
        [JsonProperty("6")]
        public int[][] C6 { get; set; }
        [JsonProperty("7")]
        public int[][] C7 { get; set; }
        [JsonProperty("8")]
        public int[][] C8 { get; set; }
        [JsonProperty("9")]
        public int[][] C9 { get; set; }
        [JsonProperty("10")]
        public int[][] C10 { get; set; }
        [JsonProperty("11")]
        public int[][] C11 { get; set; }
        [JsonProperty("12")]
        public int[][] C12 { get; set; }
        [JsonProperty("13")]
        public int[][] C13 { get; set; }
        [JsonProperty("14")]
        public int[][] C14 { get; set; }
        [JsonProperty("15")]
        public int[][] C15 { get; set; }
        [JsonProperty("16")]
        public int[][] C16 { get; set; }
        [JsonProperty("17")]
        public int[][] C17 { get; set; }

        [JsonIgnore]
        public int[][] this[int i]
        {
            get
            {
                switch (i) {
                    case 1: return C1;
                    case 2: return C2;
                    case 3: return C3;
                    case 4: return C4;
                    case 5: return C5;
                    case 6: return C6;
                    case 7: return C7;
                    case 8: return C8;
                    case 9: return C9;
                    case 10: return C10;
                    case 11: return C11;
                    case 12: return C12;
                    case 13: return C13;
                    case 14: return C14;
                    case 15: return C15;
                    case 16: return C16;
                    case 17: return C17;
                }

                throw new ArgumentOutOfRangeException($"uncnown complex number");
            }
            set
            {
                switch (i) {
                    case 1:
                        C1 = value;
                        return;
                    case 2:
                        C2 = value;
                        return;
                    case 3:
                        C3 = value;
                        return;
                    case 4:
                        C4 = value;
                        return;
                    case 5:
                        C5 = value;
                        return;
                    case 6:
                        C6 = value;
                        return;
                    case 7:
                        C7 = value;
                        return;
                    case 8:
                        C8 = value;
                        return;
                    case 9:
                        C9 = value;
                        return;
                    case 10:
                        C10 = value;
                        return;
                    case 11:
                        C11 = value;
                        return;
                    case 12:
                        C12 = value;
                        return;
                    case 13:
                        C13 = value;
                        return;
                    case 14:
                        C14 = value;
                        return;
                    case 15:
                        C15 = value;
                        return;
                    case 16:
                        C16 = value;
                        return;
                    case 17:
                        C17 = value;
                        return;
                }

                throw new ArgumentOutOfRangeException($"uncnown complex number");
            }
        }
    }
}