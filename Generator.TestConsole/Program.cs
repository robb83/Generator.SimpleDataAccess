using Generator.SimpleDataAccess.Samples;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Generator.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            String defaultConnectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            using (ChinookDatabase database = new ChinookDatabase(defaultConnectionString))
            {
                database.BeginTransaction();
                
                List<Album> albums = database.SelectAlbum();
                
                Artist artist = new Artist();
                artist.Name = "Heaven Street Seven";

                database.InsertArtist(artist);

                Album album = new Album();
                album.ArtistId = artist.ArtistId;
                album.Title = "Gesztenyefák alatt";

                database.InsertAlbum(album);

                album.Title = "Felkeltem a Reggelt";
                database.UpdateAlbum(album);

                database.DeleteAlbumByAlbumId(album.AlbumId);

                int genreCount = database.SelectGenreCount();
                if (genreCount > 0)
                {
                    int genreIdentity;
                    database.ExecuteInsertGenre("Favorite", out genreIdentity);
                }

                database.RollbackTransaction();
            }
        }
    }
}
