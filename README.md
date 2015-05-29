# Simple Dependency Free Data Access Code Generator

## Generator Usage:

    Generator.SimpleDataAccess.exe connectionstring outputfile namespace classname

    Generator.SimpleDataAccess.exe "Data Source=POWERPC\SQLEXPRESS;Initial Catalog=Chinook;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False" "..\\..\\..\\Generator.TestConsole\\ChinookDatabase.cs" "Generator.SimpleDataAccess.Samples" "ChinookDatabase"

## Programming interface

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

        database.RollbackTransaction();
    }

## Sample database: 

<https://chinookdatabase.codeplex.com/>

## More
<http://robb83.github.io>