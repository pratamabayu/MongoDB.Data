using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace MongoDB.Data
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class Model : IModel
    {
        private DateTime createdOn;

        /// <summary>
        /// id in string format
        /// </summary>
        [BsonElement(Order = 0), JsonProperty("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        /// <summary>
        /// create date
        /// </summary>
        [BsonElement("createdOn", Order = 1), JsonProperty("createdOn")]
        public DateTime CreatedOn
        {
            get
            {
                if (createdOn == null || createdOn == DateTime.MinValue)
                    createdOn = ObjectId.CreationTime;
                return createdOn;
            }
            set
            {
                createdOn = value;
            }
        }

        /// <summary>
        /// modify date
        /// </summary>
        [BsonElement("modifiedOn", Order = 2), JsonProperty("modifiedOn")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime ModifiedOn { get; set; }

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
