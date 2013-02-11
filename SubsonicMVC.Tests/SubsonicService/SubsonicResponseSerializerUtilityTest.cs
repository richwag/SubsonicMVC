using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubsonicMVC.Models;
using SubsonicMVC.SubsonicService;

namespace SubsonicMVC.Tests.SubsonicService
{
    [TestClass]
    public class SubsonicResponseSerializerUtilityTest
    {
        private static string xmlFormat = "<subsonic-response xmlns=\"http://subsonic.org/restapi\">{0}</subsonic-response>";

        private static SubsonicResponse GetResponse(string xmlContent)
        {
            var someXml = string.Format(xmlFormat, xmlContent);
            return SubsonicResponseSerializerUtility.Deserialize(someXml);
        }

        [TestMethod]
        public void TestDeserializeNoFolders()
        {
            var response = GetResponse("");

            Assert.IsNotNull(response.MusicFolders, "Folders is not null");
            Assert.IsTrue(response.MusicFolders.Count() == 0, "No folders present");
        }

        [TestMethod]
        public void TestDeserializeFolders()
        {
            var response = GetResponse("<musicFolders><musicFolder id='5' name='Folder'></musicFolder></musicFolders>");

            Assert.IsNotNull(response.MusicFolders, "Folders is not null");
            Assert.IsTrue(response.MusicFolders.Count() == 1, "One folder present");
            Assert.IsTrue(response.MusicFolders.First().Id == 5, "Folder id should be 5");
            Assert.IsTrue(response.MusicFolders.First().Name == "Folder", "Folder name should Folder");
        }

        [TestMethod]
        public void TestDeserializeError()
        {
            var response = GetResponse("<error code='50' message='This is the error'></error>");
            Assert.IsTrue(response.Error.Message == "This is the error", "Error message correct");
            Assert.IsTrue(response.Error.Code == 50, "Error code correct");
        }

        [TestMethod]
        public void TestDeserializeArtists()
        {
            var response = GetResponse("<artists><index name='A'><artist id='1' name='artist'></artist></index></artists>");

            Assert.IsTrue(response.ArtistIndeces.Count(i => i.Name == "A") == 1, "Indexes = 1");
            Assert.IsTrue(response.ArtistIndeces.First().Artists.Count(a => a.Id == 1 && a.Name == "artist") == 1);
        }

        [TestMethod]
        public void TestDeserializeArtist()
        {
            var response = GetResponse("<artist id='1' name='artist'><album id='2' name='MyAlbum' artistId='1'/></artist>");
            Assert.IsTrue(response.Artist != null, "Artist Found");
            Assert.IsTrue(response.Artist.Id == 1);
            Assert.IsTrue(response.Artist.Name == "artist");
            Assert.IsTrue(response.Artist.Albums != null);
            Assert.IsTrue(response.Artist.Albums.Count() == 1);
        }

        [TestMethod]
        public void TestDeserializeAlbum()
        {
            var response = GetResponse("<album id='2' name='MyAlbum' artistId='1'><song id='1' title='MySong' track='0'/></album>");
            Assert.IsTrue(response.Album != null);
            Assert.IsTrue(response.Album.Id == 2 && response.Album.ArtistId == 1 && response.Album.Name == "MyAlbum");
            Assert.IsTrue(response.Album.Songs.Count() == 1);
        }
    }
}
