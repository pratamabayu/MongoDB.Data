using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace MongoDB.Data
{
    public interface IModel
    {
        /// <summary>
        /// create date
        /// </summary>
        DateTime CreatedOn { get; }

        /// <summary>
        /// id in string format
        /// </summary>
        [BsonId]
        string Id { get; set; }

        /// <summary>
        /// modify date
        /// </summary>
        DateTime ModifiedOn { get; }

        /// <summary>
        /// id in objectId format
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        ObjectId ObjectId { get; }

        void ReId();
    }
}
