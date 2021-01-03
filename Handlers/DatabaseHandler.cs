using ATL;
using FRESHMusicPlayer.Handlers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using FRESHMusicPlayer.Utilities;

namespace FRESHMusicPlayer.Handlers
{
    public class DatabaseUtils // TODO: move this to FMP Core
    {
        private LiteDatabase library;
        public DatabaseUtils(LiteDatabase library) => this.library = library;
        public List<DatabaseTrack> Read(string filter = "Title") => library.GetCollection<DatabaseTrack>("tracks").Query().OrderBy(filter).ToList();
        public List<DatabaseTrack> ReadTracksForArtist(string artist) => library.GetCollection<DatabaseTrack>("tracks").Query().Where(x => x.Artist == artist).OrderBy("Title").ToList();
        public List<DatabaseTrack> ReadTracksForAlbum(string album) => library.GetCollection<DatabaseTrack>("tracks").Query().Where(x => x.Album == album).OrderBy("TrackNumber").ToList();
        public List<DatabaseTrack> ReadTracksForPlaylist(string playlist)
        {
            var x = library.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist);
            var z = new List<DatabaseTrack>();
            foreach (string path in x.Tracks) z.Add(GetFallbackTrack(path));
            return z;
        }
        public void AddTrackToPlaylist(string playlist, string path)
        {
            var x = library.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist);
            if (library.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist) is null)
            {
                x = CreatePlaylist(playlist, path);
                x.Tracks.Add(path);
            }
            else
            {
                x.Tracks.Add(path);
                library.GetCollection<DatabasePlaylist>("playlists").Update(x);
            }
        }
        public void RemoveTrackFromPlaylist(string playlist, string path)
        {
            var x = library.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist);
            x.Tracks.Remove(path);
            library.GetCollection<DatabasePlaylist>("playlists").Update(x);
        }
        public DatabasePlaylist CreatePlaylist(string playlist, string path = null)
        {
            var newplaylist = new DatabasePlaylist
            {
                Name = playlist,
                Tracks = new List<string>()
            };
            if (library.GetCollection<DatabasePlaylist>("playlists").Count() == 0) newplaylist.DatabasePlaylistID = 0;
            else newplaylist.DatabasePlaylistID = library.GetCollection<DatabasePlaylist>("playlists").Query().ToList().Last().DatabasePlaylistID + 1;
            if (path != null) newplaylist.Tracks.Add(path);
            library.GetCollection<DatabasePlaylist>("playlists").Insert(newplaylist);
            return newplaylist;
        }
        public void DeletePlaylist(string playlist) => library.GetCollection<DatabasePlaylist>("playlists").DeleteMany(x => x.Name == playlist);
        public void Import(string[] tracks)
        {
            var stufftoinsert = new List<DatabaseTrack>();
            int count = 0;
            foreach (string y in tracks)
            {
                var track = new Track(y);
                stufftoinsert.Add(new DatabaseTrack { Title = track.Title, Artist = track.Artist, Album = track.Album, Path = track.Path, TrackNumber = track.TrackNumber, Length = track.Duration });
                count++;
            }
            library.GetCollection<DatabaseTrack>("tracks").InsertBulk(stufftoinsert);
        }
        public void Import(List<string> tracks)
        {
            var stufftoinsert = new List<DatabaseTrack>();
            foreach (string y in tracks)
            {
                var track = new Track(y);
                stufftoinsert.Add(new DatabaseTrack { Title = track.Title, Artist = track.Artist, Album = track.Album, Path = track.Path, TrackNumber = track.TrackNumber, Length = track.Duration });
            }
            library.GetCollection<DatabaseTrack>("tracks").InsertBulk(stufftoinsert);
        }
        public void Import(string path)
        {
            var track = new Track(path);
            library.GetCollection<DatabaseTrack>("tracks")
                                .Insert(new DatabaseTrack { Title = track.Title, Artist = track.Artist, Album = track.Album, Path = track.Path, TrackNumber = track.TrackNumber, Length = track.Duration });
        }
        public void Remove(string path)
        {
            library.GetCollection<DatabaseTrack>("tracks").DeleteMany(x => x.Path == path);
        }
        public void Nuke(bool nukePlaylists = true)
        {
            library.GetCollection<DatabaseTrack>("tracks").DeleteAll();
            if (nukePlaylists) library.GetCollection<DatabasePlaylist>("playlists").DeleteAll();
        }

        public DatabaseTrack GetFallbackTrack(string path)
        {
            var dbTrack = library.GetCollection<DatabaseTrack>("tracks").FindOne(x => path == x.Path);
            if (dbTrack != null) return dbTrack;
            else
            {
                var track = new Track(path);
                return new DatabaseTrack { Artist = track.Artist, Title = track.Title, Album = track.Album, Length = track.Duration, Path = path, TrackNumber = track.TrackNumber };
            }
        }
    }
}
