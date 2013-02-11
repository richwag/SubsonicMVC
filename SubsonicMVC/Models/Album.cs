using System.Xml.Serialization;
using System.Collections.Generic;

namespace SubsonicMVC.Models
{
    public class Album
    {
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "artistId")]
        public int ArtistId { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "coverArt")]
        public string CoverArtId { get; set; }

        [XmlElement(ElementName = "song")]
        public List<Song> Songs { get;  set; }

        public string CoverArtUrl { get; set; }
    }
}