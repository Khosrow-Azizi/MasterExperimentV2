﻿/* Copyright 2010-2014 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoDB.Driver.GeoJsonObjectModel.Serializers
{
    /// <summary>
    /// Represents a serializer for a GeoJsonMultiPoint value.
    /// </summary>
    /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
    public class GeoJsonMultiPointSerializer<TCoordinates> : GeoJsonGeometrySerializer<TCoordinates> where TCoordinates : GeoJsonCoordinates
    {
        // private fields
        private readonly IBsonSerializer _coordinatesSerializer = BsonSerializer.LookupSerializer(typeof(GeoJsonMultiPointCoordinates<TCoordinates>));

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="nominalType">The nominal type of the object.</param>
        /// <param name="actualType">The actual type of the object.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>
        /// An object.
        /// </returns>
        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            return DeserializeGeoJsonObject(bsonReader, new MultiPointData());
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="value">The object.</param>
        /// <param name="options">The serialization options.</param>
        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            SerializeGeoJsonObject(bsonWriter, (GeoJsonObject<TCoordinates>)value);
        }

        // protected methods
        /// <summary>
        /// Deserializes a field.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="name">The name.</param>
        /// <param name="data">The data.</param>
        protected override void DeserializeField(BsonReader bsonReader, string name, ObjectData data)
        {
            var multiPointData = (MultiPointData)data;
            switch (name)
            {
                case "coordinates": multiPointData.Coordinates = DeserializeCoordinates(bsonReader); break;
                default: base.DeserializeField(bsonReader, name, data); break;
            }
        }

        /// <summary>
        /// Serializes the fields.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="obj">The GeoJson object.</param>
        protected override void SerializeFields(BsonWriter bsonWriter, GeoJsonObject<TCoordinates> obj)
        {
            var multiPoint = (GeoJsonMultiPoint<TCoordinates>)obj;
            SerializeCoordinates(bsonWriter, multiPoint.Coordinates);
        }

        // private methods
        private GeoJsonMultiPointCoordinates<TCoordinates> DeserializeCoordinates(BsonReader bsonReader)
        {
            return (GeoJsonMultiPointCoordinates<TCoordinates>)_coordinatesSerializer.Deserialize(bsonReader, typeof(GeoJsonMultiPointCoordinates<TCoordinates>), null);
        }

        private void SerializeCoordinates(BsonWriter bsonWriter, GeoJsonMultiPointCoordinates<TCoordinates> coordinates)
        {
            bsonWriter.WriteName("coordinates");
            _coordinatesSerializer.Serialize(bsonWriter, typeof(GeoJsonMultiPointCoordinates<TCoordinates>), coordinates, null);
        }

        // nested classes
        private class MultiPointData : ObjectData
        {
            // private fields
            private GeoJsonMultiPointCoordinates<TCoordinates> _coordinates;

            // constructors
            public MultiPointData()
                : base("MultiPoint")
            {
            }

            // public properties
            public GeoJsonMultiPointCoordinates<TCoordinates> Coordinates
            {
                get { return _coordinates; }
                set { _coordinates = value; }
            }

            // public methods
            public override object CreateInstance()
            {
                return new GeoJsonMultiPoint<TCoordinates>(Args, _coordinates);
            }
        }
    }
}
