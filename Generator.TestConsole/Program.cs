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

                Customer customer = new Customer();
                customer.Address = "Alma u. 1";
                customer.City = "Budapest";
                customer.Company = null;
                customer.Country = "Magyarország";
                customer.Email = "robb83@gmail.com";
                customer.Fax = null;
                customer.FirstName = "Róbert";
                customer.LastName = "Kovács";
                customer.Phone = null;
                customer.PostalCode = "1188";
                customer.State = null;
                customer.SupportRepId = null;

                database.InsertCustomer(customer);

                int customerID = customer.CustomerId;
                String fullName = customer.FullName;
                String fullDetail = customer.FullDetail;

                customer.LastName = "Oláh";

                database.UpdateCustomer(customer);

                customerID = customer.CustomerId;
                fullName = customer.FullName;
                fullDetail = customer.FullDetail;

                Artist artist2 = new Artist();
                artist2.Name = "Kis Pál és a borz";

                database.UpsertArtist(artist2);

                artist2.Name = "Kispál és a Borz";

                database.UpsertArtist(artist2);

                int artitsCount = database.SelectArtistCount();
                int pageSize = 10, pageIndex = 0;
                while(true)
                {
                    List<Artist> page = database.SelectArtistPaged(pageIndex * pageSize + 1, pageIndex * pageSize + pageSize - 1);

                    if (page.Count == 0)
                    {
                        break;
                    }

                    ++pageIndex;
                }

                database.RollbackTransaction();
            }
        }
    }
}
