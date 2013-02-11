using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SubsonicMVC.Models
{
    public class Artist
    {
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "coverArt")]
        public string CoverArt { get; set; }

        [XmlAttribute(AttributeName = "albumCount")]
        public int AlbumCount { get; set; }

        [XmlElement(ElementName = "album")]
        public List<Album> Albums { get; set; }
    }
}