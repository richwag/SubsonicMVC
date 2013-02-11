using System.IO;
using System.Net;
using System.Xml.Serialization;
using SubsonicMVC.Models;
using System.Runtime.Serialization;
using System.Drawing;
using System.Configuration;

namespace SubsonicMVC.SubsonicService
{
    public class Subsonic : ISubsonic
    {
        private static string serverUrl = ConfigurationManager.AppSettings["ServerUrl"];

        private static SubsonicResponse CallSubsonic(string request, string callParams)
        {
            WebClient client = new WebClient();
            var rawResult = client.DownloadString(string.Format("{2}{0}?c=me&v=1.8.0&u=anyone&p=anyone&{1}", request, callParams, serverUrl));
            return SubsonicResponseSerializerUtility.Deserialize(rawResult);
        }

        private static SubsonicResponse CallSubsonic(string request)
        {
            return CallSubsonic(request, "");
        }

        public static Image CoverArt(string id)
        {            
            WebClient client = new WebClient();
            var bytes = client.DownloadData(string.Format("{0}/getCoverArt.view?c=me&v=1.8.0&u=anyone&p=anyone&id={1}", serverUrl, id));
            return Image.FromStream(new MemoryStream(bytes));
        }

        #region ISubsonic Members

        public SubsonicResponse MusicFolders
        {
            get
            {
                return CallSubsonic("getMusicFolders.view");
            }
        }

        public SubsonicResponse Artists
        {
            get
            {
                return CallSubsonic("getArtists.view");
            }
        }

        public SubsonicResponse Artist(int id)
        {
            return CallSubsonic("getArtist.view", string.Format("id={0}", id));
        }

        public SubsonicResponse Album(int id)
        {
            return CallSubsonic("getAlbum.view", string.Format("id={0}", id));
        }

        #endregion
    }
}