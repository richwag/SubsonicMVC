using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ServiceModel;

namespace SubsonicMVC.Models
{
    [XmlRoot(ElementName = "subsonic-response", Namespace = "http://subsonic.org/restapi")]
    public class SubsonicResponse
    {
        // Artists
        [XmlArray(ElementName = "artists")]
        [XmlArrayItem(ElementName = "index")]
        public List<Index> ArtistIndeces { get; set; }

        // Folders
        [XmlArray(ElementName = "musicFolders")]
        [XmlArrayItem(ElementName = "musicFolder")]
        public List<MusicFolder> MusicFolders { get; set; }

        // Error
        [XmlElement(ElementName = "error")]
        public Error Error { get; set; }

        // Artist
        [XmlElement(ElementName = "artist")]
        public Artist Artist { get; set; }

        // Album
        [XmlElement(ElementName = "album")]
        public Album Album { get; set; }
    }

    public class Error
    {
        [XmlAttribute(AttributeName = "message")]
        public string Message { get; set; }

        [XmlAttribute(AttributeName="code")]
        public int Code { get; set; }
    }
}