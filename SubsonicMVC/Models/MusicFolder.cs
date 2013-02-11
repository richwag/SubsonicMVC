using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SubsonicMVC.Models
{
    public class MusicFolder
    {
        [XmlAttribute(AttributeName="id")]
        public int Id { get; set; }
        [XmlAttribute(AttributeName="name")]
        public string Name { get; set; }
    }
}