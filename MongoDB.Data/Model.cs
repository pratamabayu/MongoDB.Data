using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace MongoDB.Data
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class Model : IModel
    {
        private DateTime createdAt;

        /// <summary>
        /// id in string format
        /// </summary>
        [BsonElement(Order = 0), JsonProperty("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        /// <summary>
        /// create date
        /// </summary>
        [BsonElement("createdAt", Order = 1), JsonProperty("createdAt")]
        public DateTime CreatedAt
        {
            get
            {
                if (createdAt == DateTime.MinValue)
                    createdAt = ObjectId.CreationTime;
                return createdAt;
            }
            set
            {
                createdAt = value;
            }
        }

        /// <summary>
        /// modify date
        /// </summary>
        [BsonElement("modifiedAt", Order = 2), JsonProperty("modifiedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime ModifiedAt { get; set; }

        /// <summary>
        /// id in objectId format
        /// </summary>
        public ObjectId ObjectId => ObjectId.Parse(Id);

        public Model()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        public void ReId()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}
