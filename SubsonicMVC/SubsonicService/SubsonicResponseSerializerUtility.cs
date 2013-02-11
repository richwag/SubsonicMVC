using System.IO;
using System.Xml.Serialization;
using SubsonicMVC.Models;

namespace SubsonicMVC.SubsonicService
{
    public static class SubsonicResponseSerializerUtility
    {
        public static SubsonicResponse Deserialize(string rawResult)
        {
            XmlSerializer xs = new XmlSerializer(typeof(SubsonicResponse));
            SubsonicResponse response = (SubsonicResponse)xs.Deserialize(new StringReader(rawResult));
            return response;
        }

    }
}