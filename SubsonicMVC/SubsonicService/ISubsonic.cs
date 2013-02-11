using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubsonicMVC.Models;

namespace SubsonicMVC.SubsonicService
{
    interface ISubsonic
    {
        SubsonicResponse MusicFolders { get; }
        SubsonicResponse Artists { get; }
        SubsonicResponse Artist(int id);
        SubsonicResponse Album(int id);
    }
}
