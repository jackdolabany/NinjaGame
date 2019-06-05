using Microsoft.Xna.Framework.Content.Pipeline;
using System.Xml.Serialization;
using System.IO;
using TileEngine;

namespace CustomContentProcessorLibrary
{
    [ContentImporter(".mmd", DefaultProcessor = "PassThroughProcessor", DisplayName = "Map Meta Data Importer")]
    public class MapMetaDataImporter : ContentImporter<MapMetaData>
    {
        public override MapMetaData Import(string filename, ContentImporterContext context)
        {
            var serializer = new XmlSerializer(typeof(MapMetaData));
            using (var sr = new StreamReader(filename))
            {
                return (MapMetaData)serializer.Deserialize(sr);
            }
        }
    }
}
