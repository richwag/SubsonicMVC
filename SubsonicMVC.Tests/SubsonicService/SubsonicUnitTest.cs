using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubsonicMVC.Models;
using SubsonicMVC.SubsonicService;

namespace SubsonicMVC.Tests.SubsonicService
{
    [TestClass]
    public class SubsonicUnitTest
    {
        [TestMethod]
        public void TestFolders()
        {
            var s = new Subsonic();
            var response = s.MusicFolders;
        }

        [TestMethod]
        public void TestArtists()
        {
            var s = new Subsonic();
            var response = s.Artists;
        }
    }
}
