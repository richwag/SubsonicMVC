using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SubsonicMVC.Models
{
    public class Index
    {
        [XmlAttribute(AttributeName="name")]
        public string Name { get; set; }

        [XmlElement(ElementName="artist")]
        public List<Artist> Artists { get; set; }
    }
}