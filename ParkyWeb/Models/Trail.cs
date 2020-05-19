using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ParkyWeb.Models
{
    /*
     * You must apply a DataContractAttribute or SerializableAttribute
     * to a class to have it serialized by the DataContractSerializer.
     */
    //[DataContract(Name = "TrailDTO")]
    [JsonObject(Title = "TrailDTO")]
    public class Trail
    {
        //[DataMember(Name = "Id")]
        [JsonProperty("Id")]
        public int Id { get; set; }

        [Required]
        //[DataMember(Name = "Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        //[DataMember(Name = "Distance")]
        [JsonProperty("Distance")]
        public double Distance { get; set; }

        [Required]
        //[DataMember(Name = "Elevation")]
        [JsonProperty("Elevation")]
        public double Elevation { get; set; }

        [Required]
        //[DataMember(Name = "NationalParkId")]
        [JsonProperty("NationalParkId")]
        public int NationalParkId { get; set; }

        //[DataMember(Name = "DifficultyType")]
        [JsonProperty("DifficultyType")]
        public DifficultyType Difficulty { get; set; }

        //[DataMember(Name = "NationalParkDTO")]
        [JsonProperty("NationalParkDTO")]
        public NationalPark NationalPark { get; set; }

        public enum DifficultyType
        {
            Easy,
            Moderate,
            Difficult,
            Expert
        }
    }
}
