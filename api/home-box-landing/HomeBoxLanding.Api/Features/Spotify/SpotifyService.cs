using System.Web;
using HomeBoxLanding.Api.Features.Spotify.Types;
using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Spotify;

public class SpotifyService
{
    private const string Songs = "Obywatel G.C. - NIE PYTAJ O POLSKĘ\", \"Lao Che - WOJENKA\", \"Maanam - KRAKOWSKI SPLEEN\", \"Kult - ARAHJA\", \"Myslovitz - DŁUGOŚĆ DŹWIĘKU SAMOTNOŚCI\", \"Perfect - AUTOBIOGRAFIA\", \"Czesław Niemen - DZIWNY JEST TEN ŚWIAT\", \"Kult - POLSKA\", \"Zbigniew Wodecki - LUBIĘ WRACAĆ TAM GDZIE BYŁEM\", \"Andrzej Zaucha - BYŁAŚ SERCA BICIEM\", \"Fisz Emade Tworzywo - OK, BOOMER!\", \"Sztywny Pal Azji - WIEŻA RADOŚCI, WIEŻA SAMOTNOŚCI\", \"Męskie Granie Orkiestra 2021 - I CIEBIE TEŻ, BARDZO\", \"Dżem - LIST DO M.\", \"Republika - BIAŁA FLAGA\", \"Marek Grechuta & Anawa - DNI, KTÓRYCH NIE ZNAMY\", \"Chłopcy z Placu Broni - KOCHAM WOLNOŚĆ\", \"Kazik - 12 GROSZY\", \"Martyna Jakubowicz - W DOMACH Z BETONU NIE MA WOLNEJ MIŁOŚCI\", \"Kaśka Sochacka - NIEBO BYŁO RÓŻOWE\", \"Aya RL - SKÓRA\", \"Edmund Fetting - NIM WSTANIE DZIEŃ (Z FILMU \"PRAWO I PIĘŚĆ\")\", \"Daab - W MOIM OGRODZIE\", \"Kobranocka - KOCHAM CIĘ JAK IRLANDIĘ\", \"Rezerwat - ZAOPIEKUJ SIĘ MNĄ\", \"Lao Che - HYDROPIEKŁOWSTĄPIENIE\", \"Czesław Niemen - SEN O WARSZAWIE\", \"Hey & Edyta Bartosiewicz - MOJA I TWOJA NADZIEJA\", \"Raz, Dwa, Trzy - TRUDNO NIE WIERZYĆ W NIC\", \"Budka Suflera - JOLKA, JOLKA, PAMIĘTASZ\", \"Grażyna Łobaszewska - CZAS NAS UCZY POGODY\", \"Krystyna Prońko - JESTEŚ LEKIEM NA CAŁE ZŁO\", \"Paktofonika - JESTEM BOGIEM\", \"Tadeusz Woźniak - ZEGARMISTRZ ŚWIATŁA\", \"Lombard - PRZEŻYJ TO SAM\", \"Stanisław Soyka - TOLERANCJA / NA MIŁY BÓG\", \"Małgorzata Ostrowska - MELUZYNA\", \"Franek Kimono - KING BRUCE LEE KARATE MISTRZ\", \"Hey - TEKSAŃSKI\", \"Maanam - KOCHAM CIĘ KOCHANIE MOJE\", \"Męskie Granie Orkiestra 2018 - POCZĄTEK\", \"Breakout - MODLITWA\", \"Kult - DO ANI\", \"Dawid Podsiadło - MORI\", \"Róże Europy i Edyta Bartosiewicz - JEDWAB\", \"Happysad - ZANIM PÓJDĘ\", \"Obywatel G.C. - TAK, TAK... TO JA\", \"Turbo - DOROSŁE DZIECI\", \"KSU - MOJE BIESZCZADY\", \"Klaus Mitffoch - JEZU JAK SIĘ CIESZĘ\", \"Dżem - WEHIKUŁ CZASU\", \"Grzegorz Turnau - BRACKA\", \"TSA - 51\", \"Krystyna Janda - NA ZAKRĘCIE\", \"Jerzy Stuhr - ŚPIEWAĆ KAŻDY MOŻE\", \"Pidżama Porno - NIKT TAK PIĘKNIE NIE MÓWIŁ, ŻE SIĘ BOI MIŁOŚCI\", \"Hurt - ZAŁOGA G\", \"Kwiat Jabłoni - MOGŁO BYĆ NIC\", \"Tilt - JESZCZE BĘDZIE PRZEPIĘKNIE\", \"Daria Zawiałow - JESZCZE W ZIELONE GRAMY\", \"Republika - ODCHODZĄC\", \"Krystyna Prońko - PSALM STOJĄCYCH W KOLEJCE\", \"Anna Jantar - NIC NIE MOŻE WIECZNIE TRWAĆ\", \"Katarzyna Nosowska - JEŚLI WIESZ CO CHCĘ POWIEDZIEĆ\", \"Spięty - BLUE\", \"Kult - BARANEK\", \"T.Love - WARSZAWA\", \"Zbigniew Wodecki - ZACZNIJ OD BACHA\", \"Brygada Kryzys - TO CO CZUJESZ, TO CO WIESZ\", \"Lombard - SZKLANA POGODA\", \"Maanam - CYKADY NA CYKLADACH\", \"Andrzej Zaucha - C'EST LA VIE - PARYŻ Z POCZTÓWKI\", \"Varius Manx - PIOSENKA KSIĘŻYCOWA\", \"O.N.A. - KIEDY POWIEM SOBIE DOŚĆ\", \"Artur Andrus - PIŁEM W SPALE, SPAŁEM W PILE\", \"Budka Suflera - JEST TAKI SAMOTNY DOM\", \"Jacek Kaczmarski - NASZA KLASA\", \"Dżem - DO KOŁYSKI\", \"Perfect - NIE PŁACZ EWKA\", \"Stare Dobre Małżeństwo - BIESZCZADZKIE ANIOŁY\", \"Myslovitz i Marek Grechuta - KRAKÓW\", \"Hey - ZAZDROŚĆ\", \"Kazik - TWÓJ BÓL JEST LEPSZY NIŻ MÓJ\", \"Voo Voo - NIM STANIE SIĘ TAK, JAK GDYBY NIGDY NIC\", \"Hey - LIST\", \"Dżem - SEN O VICTORII\", \"Edyta Geppert - OCH ŻYCIE, KOCHAM CIĘ NAD ŻYCIE\", \"Kacperczyk feat. Artur Rojek - SYN OKIENNIKA\", \"Jacek Kaczmarski - OBŁAWA\", \"Bogusław Mec - JEJ PORTRET\", \"Kaczmarski, Gintrowski, Łapiński - MURY\", \"Stare Dobre Małżeństwo - CZARNY BLUES O CZWARTEJ NAD RANEM\", \"Lech Janerka - ROWER\", \"Zbigniew Wodecki with Mitch & Mitch Orchestra And Choir - RZUĆ TO WSZYSTKO CO ZŁE\", \"Edyta Bartosiewicz - OSTATNI\", \"Mikromusic - TAK MI SIĘ NIE CHCE\", \"Matylda / Łukasiewicz - MATKA\", \"Breakout - KIEDY BYŁEM MAŁYM CHŁOPCEM\", \"Mr. Z'oob - KAWAŁEK PODŁOGI\", \"Perfect - NIEPOKONANI\", \"Lady Pank - ZAWSZE TAM GDZIE TY\", \"Coma - SPADAM\", \"Artur Rojek - BEKSA\", \"Coma - LOS, CEBULA I KROKODYLE ŁZY\", \"Stan Borys - JASKÓŁKA UWIĘZIONA\", \"Krzysztof Zalewski - MIŁOŚĆ, MIŁOŚĆ\", \"Strachy na Lachy - DZIEŃ DOBRY, KOCHAM CIĘ\", \"Luxtorpeda - AUTYSTYCZNY\", \"De Mono - KOCHAĆ INACZEJ\", \"Kaśka Sochacka - WIŚNIA\", \"2 plus 1 - CHODŹ, POMALUJ MÓJ ŚWIAT\", \"T.Love - KING\", \"Sosnowski - PYŁ\", \"Budka Suflera - CIEŃ WIELKIEJ GÓRY\", \"Artur Rojek - SYRENY\", \"Renata Przemyk & Kasia Nosowska - KOCHANA\", \"Chłopcy z Placu Broni - KOCHAM CIĘ\", \"Dżem - WHISKY\", \"Grzegorz Turnau - NAPRAWDĘ NIE DZIEJE SIĘ NIC\", \"Czesław Niemen - WSPOMNIENIE\", \"Marek Grechuta & Anawa - KOROWÓD\", \"Dawid Podsiadło - NIEZNAJOMY\", \"Republika - TELEFONY\", \"Bajm - CO MI PANIE DASZ\", \"Wojciech Młynarski - JESZCZE W ZIELONE GRAMY\", \"Daria ze Śląska - KILL BILL\", \"Marek Biliński - UCIECZKA Z TROPIKU\", \"Lady Pank - SZTUKA LATANIA\", \"Urszula - DMUCHAWCE, LATAWCE, WIATR\", \"Lady Pank - MNIEJ NIŻ ZERO\", \"Perfect - CHCEMY BYĆ SOBĄ\", \"Blenders - CIĄGNIK\", \"Budka Suflera - SEN O DOLINIE\", \"Lady Pank - KRYZYSOWA NARZECZONA\", \"Izabela Trojanowska - WSZYSTKO CZEGO DZIŚ CHCĘ\", \"Akurat - DO PROSTEGO CZŁOWIEKA\", \"Czesław Niemen - POD PAPUGAMI\", \"Kult - HEJ, CZY NIE WIECIE\", \"Republika - ZAPYTAJ MNIE CZY CIĘ KOCHAM\", \"Alicja Majewska - ODKRYJEMY MIŁOŚĆ NIEZNANĄ\", \"Kaliber 44 - PLUS I MINUS\", \"Lady Pank - MARCHEWKOWE POLE\", \"Elektryczne Gitary - DZIECI\", \"Myslovitz - CHCIAŁBYM UMRZEĆ Z MIŁOŚCI\", \"IRA - NADZIEJA\", \"Taco Hemingway - NASTĘPNA STACJA\", \"Kortez - Z IMBIREM\", \"Dawid Podsiadło - MAŁOMIASTECZKOWY\", \"Krzysztof Komeda - ROSEMARY'S BABY\", \"Strachy na Lachy - PIŁA TANGO\", \"Kobranocka - I NIKOMU NIE WOLNO SIĘ Z TEGO ŚMIAĆ\", \"Tilt - MÓWIĘ CI ŻE\", \"Kortez - ZOSTAŃ\", \"Basia Stępniak-Wilk i Grzegorz Turnau - BOMBONIERKA\", \"Pablopavo i Ludziki - KARWOSKI\", \"Ewa Bem - SERCE TO JEST MUZYK\", \"Maanam - SZAŁ NIEBIESKICH CIAŁ\", \"Coma - LESZEK ŻUKOWSKI\", \"Maryla Rodowicz - NIECH ŻYJE BAL\", \"Kombi - SŁODKIEGO MIŁEGO ŻYCIA\", \"Maanam - BOSKIE BUENOS\", \"Marek Grechuta & Anawa - ŚWIECIE NASZ\", \"Perfect - NIEWIELE CI MOGĘ DAĆ\", \"Grzegorz Turnau - CICHOSZA\", \"Nosowska - LEDWIE WCZORAJ\", \"Grzegorz Turnau - MIĘDZY CISZĄ A CISZĄ\", \"Świetliki i Linda - FILANDIA\", \"Zbigniew Wodecki i Zdzisława Sośnicka - Z TOBĄ CHCĘ OGLĄDAĆ ŚWIAT\", \"Ørganek - FOTOGRAF BROK\", \"Renata Przemyk - BABĘ ZESŁAŁ BÓG\", \"Raz, Dwa, Trzy - I TAK WARTO ŻYĆ\", \"Kazik - SPALAM SIĘ\", \"Wilki - ELI LAMA SABACHTANI\", \"Krzysztof Krawczyk i Edyta Bartosiewicz - TRUDNO TAK (RAZEM BYĆ NAM ZE SOBĄ...)\", \"Budka Suflera - NOC KOMETY\", \"Magda Umer - KONCERT JESIENNY NA DWA ŚWIERSZCZE I WIATR W KOMINIE\", \"Zdzisława Sośnicka - ALEJA GWIAZD\", \"Hanna Banaszak - W MOIM MAGICZNYM DOMU\", \"Akurat - LUBIĘ MÓWIĆ Z TOBĄ\", \"Ørganek - MISSISSIPPI W OGNIU\", \"Wodecki/Pater feat. Wiktor Waligóra - NAD WSZYSTKO UŚMIECH TWÓJ\", \"Maria Peszek - SORRY POLSKO\", \"Edyta Geppert - JAKA RÓŻA, TAKI CIERŃ\", \"Kaczmarski, Gintrowski, Łapiński - MODLITWA O WSCHODZIE SŁOŃCA\", \"Riverside - FRIEND OR FOE\", \"Formacja Nieżywych Schabuff - KLUB WESOŁEGO SZAMPANA\", \"Maanam - LIPSTICK ON THE GLASS\", \"Fisz Emade Tworzywo - ZA MAŁO CZASU\", \"Kult - GDY NIE MA DZIECI\", \"Grzegorz Markowski - BALLADA 07 (Z SERIALU TV \"07 ZGŁOŚ SIĘ\")\", \"Hey - MIMO WSZYSTKO\", \"Elektryczne Gitary - CZŁOWIEK Z LIŚCIEM\", \"Oddział Zamknięty - OBUDŹ SIĘ\", \"Ania Dąbrowska - CHARLIE, CHARLIE\", \"Stare Dobre Małżeństwo - Z NIM BĘDZIESZ SZCZĘŚLIWSZA\", \"Męskie Granie Orkiestra 2016 - WATAHA\", \"Taco Hemingway - DESZCZ NA BETONIE\", \"Zbigniew Zamachowski i Grupa MoCarta - KOBIETY JAK TE KWIATY\", \"Męskie Granie Orkiestra 2019 - SOBIE I WAM\", \"Bluszcz - LAMPARTY\", \"Edyta Bartosiewicz - SEN\", \"Czerwone Gitary - PŁONĄ GÓRY, PŁONĄ LASY\", \"Muniek Staszczyk - POLA\", \"Golden Life - 24.11.94\", \"Męskie Granie Orkiestra 2023 (Igo, Mrozu, Vito Bambino) - SUPERMOCE\", \"Varius Manx - ZANIM ZROZUMIESZ\", \"Banda i Wanda - HI-FI\", \"Perfect - KOŁYSANKA DLA NIEZNAJOMEJ\", \"Alicja Majewska - JESZCZE SIĘ TAM ŻAGIEL BIELI\", \"Ralph Kaminski - KOSMICZNE ENERGIE\", \"Lombard - GOŁĘBI PUCH\", \"Kaśka Sochacka - MADISON\", \"Król - TE SMAKI I ZAPACHY\", \"Dżamble - WYMYŚLIŁEM CIEBIE\", \"Kazik & Yugoton - MALCZIKI\", \"Dżem - CZERWONY JAK CEGŁA\", \"Big Cyc - BERLIN ZACHODNI\", \"Budka Suflera - ZA OSTATNI GROSZ\", \"Marek Grechuta & Anawa - OCALIĆ OD ZAPOMNIENIA\", \"Jonasz Kofta - PAMIĘTAJCIE O OGRODACH\", \"Wilki - SON OF THE BLUE SKY\", \"Kult - CELINA\", \"Bajm - DWA SERCA, DWA SMUTKI\", \"Fisz Emade Tworzywo feat. Justyna Święs - ŚLADY\", \"Armia - NIEZWYCIĘŻONY\", \"Raz, Dwa, Trzy - POD NIEBEM\", \"Anita Lipnicka - I WSZYSTKO SIĘ MOŻE ZDARZYĆ\", \"Muchy - SZARORÓŻOWE\", \"Lao Che - KAPITAN POLSKA\", \"Anna Maria Jopek & Pat Metheny - TAM, GDZIE NIE SIĘGA WZROK\", \"Edyta Bartosiewicz - JENNY\", \"Maanam - PO PROSTU BĄDŹ\", \"Andrzej Kurylewicz - TEMAT SERIALU \"POLSKIE DROGI\"\", \"Kortez - HEJ WY\", \"Republika - RAZ NA MILION LAT\", \"Myslovitz - CHŁOPCY\", \"Klaus Mitffoch - STRZEŻ SIĘ TYCH MIEJSC\", \"Maanam - WYJĄTKOWO ZIMNY MAJ\", \"Anna Jantar - TYLE SŁOŃCA W CAŁYM MIEŚCIE\", \"Hanna Banaszak - SAMBA PRZED ROZSTANIEM\", \"Halina Frąckowiak - PAPIEROWY KSIĘŻYC\", \"Seweryn Krajewski - UCIEKAJ MOJE SERCE\", \"TSA - TRZY ZAPAŁKI\", \"Myslovitz - SCENARIUSZ DLA MOICH SĄSIADÓW\", \"Lech Janerka - KONSTYTUCJE\", \"Lao Che - NIE RAJ\", \"Fisz Emade Tworzywo - DWA OGNIE\", \"Bajm - BIAŁA ARMIA\", \"Chłopcy z Placu Broni - O! ELA\", \"2 plus 1 - WINDĄ DO NIEBA\", \"Król - Z TOBĄ / DO DOMU\", \"Kabaret Starszych Panów - PIOSENKA JEST DOBRA NA WSZYSTKO\", \"Maanam - LUCCIOLA\", \"Bajm - JEZIORO SZCZĘŚCIA\", \"Marek Grechuta & Anawa - WIOSNA, ACH TO TY\", \"Anna Maria Jopek - JA WYSIADAM\", \"Brodka - GRANDA\", \"Obywatel G.C. - PARYŻ - MOSKWA 17.15\", \"Kapitan Nemo - TWOJA LORELEI\", \"Sidney Polak - OTWIERAM WINO\", \"Maryla Rodowicz - MAŁGOŚKA\", \"De Mono - STATKI NA NIEBIE\", \"Myslovitz - PEGGY BROWN\", \"Domowe Melodie - GRAŻKA\", \"Wojciech Młynarski - JESTEŚMY NA WCZASACH\", \"Kult - KREW BOGA\", \"Illusion - NÓŻ\", \"Kwiat Jabłoni - DZIŚ PÓŹNO PÓJDĘ SPAĆ\", \"Kasia Kowalska - A TO CO MAM\", \"Cool Kids Of Death - BUTELKI Z BENZYNĄ I KAMIENIE\", \"Myslovitz - DLA CIEBIE\", \"Republika - MOJA KREW\", \"T.Love - IV LICEUM\", \"Raz, Dwa, Trzy - JUTRO MOŻEMY BYĆ SZCZĘŚLIWI\", \"Kazik na Żywo - TATA DILERA\", \"Basia - CRUISING FOR BRUISING\", \"Kult - DZIEWCZYNA BEZ ZĘBA NA PRZEDZIE\", \"Lady Pank - WCIĄŻ BARDZIEJ OBCY\", \"Stanisław Soyka - CUD NIEPAMIĘCI\", \"Krystyna Prońko - MAŁE TĘSKNOTY\", \"Skubas - NIE MAM DLA CIEBIE MIŁOŚCI\", \"Kazik & Edyta Bartosiewicz - CZTERY POKOJE\", \"Ewa Demarczyk - KARUZELA Z MADONNAMI\", \"Czesław Niemen - JEDNEGO SERCA\", \"Anna Jurksztowicz - STAN POGODY\", \"Republika - MAMONA\", \"Budka Suflera - NIE WIERZ NIGDY KOBIECIE\", \"IRA - MÓJ DOM\", \"Maanam - TO TYLKO TANGO\", \"Marek Jackowski - OPRÓCZ\", \"Mieczysław Fogg - TO OSTATNIA NIEDZIELA\", \"Voo Voo - GDYBYM\", \"Brodka - VARSOVIE\", \"Sidney Polak - CHOMICZÓWKA\", \"Czerwony Tulipan - JEDYNE CO MAM\", \"Kaśka Sochacka & Vito Bambino - BOJĘ SIĘ O CIEBIE\", \"Kaśka Sochacka - CICHE DNI\", \"Krzysztof Krawczyk - BO JESTEŚ TY\", \"Czerwone Gitary - KWIATY WE WŁOSACH\", \"Pidżama Porno - EZOTERYCZNY POZNAŃ\", \"Kult - CZARNE SŁOŃCA\", \"Czesław Śpiewa - MASZYNKA DO ŚWIERKANIA\", \"Paweł Domagała - WEŹ NIE PYTAJ\", \"GrubSon - NA SZCZYCIE\", \"T.Love - TO WYCHOWANIE\", \"Ryszard Rynkowski - SZCZĘŚLIWEJ DROGI JUŻ CZAS\", \"Republika - ŚMIERĆ W BIKINI\", \"Zakopower - BOSO\", \"Czesław Niemen - BEMA PAMIĘCI ŻAŁOBNY - RAPSOD\", \"Brodka - HORSES\", \"Lordofon - FRANCOISE HARDY\", \"Dżem - MODLITWA III - POZWÓL MI\", \"Kayah & Bregović - ŚPIJ KOCHANIE, ŚPIJ\", \"Raz, Dwa, Trzy - W WIELKIM MIEŚCIE\", \"Mrozu - ZŁOTO\", \"Urszula Dudziak - PAPAJA\", \"Karaś / Rogucki - KILKA WESTCHNIEŃ\", \"Męskie Granie Orkiestra 2020 - PŁONĄ GÓRY, PŁONĄ LASY\", \"Piotr Bukartyk - KOBIETY JAK TE KWIATY\", \"Dżem - PARTYZANT\", \"Maryla Rodowicz - ŁATWOPALNI\", \"Mesajah - KAŻDEGO DNIA\", \"Michał Lorenc - TANIEC ELENY\", \"Emigranci - NA FALOCHRONIE\", \"Skaldowie i Łucja Prus - W ŻÓŁTYCH PŁOMIENIACH LIŚCI\", \"Kult - PIOSENKA MŁODYCH WIOŚLARZY\", \"Zbigniew Hołdys - CO SIE STAŁO Z MAGDĄ K.?\", \"Krystyna Prońko - DESZCZ W CISNEJ\", \"Kayah & Bregović - BYŁAM RÓŻĄ\", \"Oddział Zamknięty - ANDZIA I JA\", \"De Press - BO JO CIE KOCHOM\", \"Czerwone Gitary - BIAŁY KRZYŻ\", \"Wilki - BAŚKA\", \"Perfect - WYSPA, DRZEWO, ZAMEK\", \"Raz, Dwa, Trzy - OCZY TEJ MAŁEJ\", \"Kult - LEWE LEWE LOFF\", \"Marek Grechuta - NIEPEWNOŚĆ\", \"Irena Jarocka - KAWIARENKI\", \"Edyta Bartosiewicz - SKŁAMAŁAM\", \"Męskie Granie Orkiestra 2014 - ELEKTRYCZNY\", \"Lombard - ADRIATYK, OCEAN GORĄCY\", \"Kortez - STARE DRZEWA\", \"Sistars - SUTRA\", \"Strachy na Lachy - ŻYJĘ W KRAJU\", \"T.Love - LUCY PHERE\", \"Lady Pank - TAŃCZ GŁUPIA, TAŃCZ\", \"Maanam - SIE ŚCIEMNIA\", \"Perfect - OBJAZDOWE NIEME KINO\", \"Oddział Zamknięty - TEN WASZ ŚWIAT\", \"Brodka & A_GIM - WSZYSTKO CZEGO DZIŚ CHCĘ\", \"Anna Maria Jopek - ALE JESTEM\", \"Ørganek - WIOSNA\", \"Kasia Kowalska - JAK RZECZ\", \"Krzysztof Zalewski - PTAKI (MTV UNPLUGGED)\", \"Budka Suflera - CZAS OŁOWIU\", \"Michał Bajor - NIE CHCĘ WIĘCEJ\", \"Kult - PO CO WOLNOŚĆ";
    private const string PlaylistId = "0pBFkvFtjydwKq4fLGfcij";
    
    public SpotifyImportSongsResponse GetActivity(SpotifyTestRequest request)
    {
        return new SpotifyImportSongsResponse();
        
        Console.WriteLine("Authenticating with Spotify...");
        
        var authToken = GetAuthenticationToken(request.Code);

        if (authToken == null)
            return new SpotifyImportSongsResponse();

        var parsedSongs = Songs.Split("\", \"");
        
        Console.WriteLine("Starting search for songs...");

        var index = 1;
        foreach (var song in parsedSongs)
        {
            var searchResult = SearchForSong(authToken, song);
            
            if (searchResult?.SpotifyUri is not null)
            {
                Console.WriteLine($"[{index}/{parsedSongs.Length}] Adding '{searchResult.Artists.FirstOrDefault()?.Name} - {searchResult.Name}' to playlist...");
                InsertSongsIntoPlaylist(authToken, [searchResult.SpotifyUri]);
            }

            index++;
        }
            
        Console.WriteLine("Done!");
        
        return new SpotifyImportSongsResponse
        {
            HasError = false
        };
    }

    private string? GetAuthenticationToken(string authCode)
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        httpClient.DefaultRequestHeaders.Add("Authorization","Basic NDk1ZTllM2FkMDdmNGM1YjlhODU0ZGYxMTVlMmNmNDM6MWRiNTIwZDU5ZDMzNDc0NWFhNDI3ZWQ5OWZiYWNkMzM=");
        
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "authorization_code"),
            new("code", authCode),
            new("redirect_uri", "http://localhost:4200/spotify")
        };
        var result = httpClient.PostAsync($"https://accounts.spotify.com/api/token", new FormUrlEncodedContent(parameters)).Result;
        
        var response = result.Content.ReadAsStringAsync().Result;

        SpotifyTokenResponse? parsedResponse;
        try
        {
            parsedResponse = JsonConvert.DeserializeObject<SpotifyTokenResponse>(response);
        }
        catch (Exception)
        {
            return null;
        }

        return parsedResponse?.AccessToken;
    }

    private SpotifySearchTrackItem SearchForSong(string authToken, string song)
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        httpClient.DefaultRequestHeaders.Add("Authorization",$"Bearer {authToken}");
        
        var builder = new UriBuilder("https://api.spotify.com/v1/search")
        {
            Port = -1
        };
        
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["q"] = song;
        query["type"] = "track";
        builder.Query = query.ToString();
        
        var result = httpClient.GetAsync(builder.ToString()).Result;
        
        var response = result.Content.ReadAsStringAsync().Result;

        SpotifySearchResponse? parsedResponse;
        try
        {
            parsedResponse = JsonConvert.DeserializeObject<SpotifySearchResponse>(response);
        }
        catch (Exception)
        {
            return new SpotifySearchTrackItem();
        }

        return parsedResponse?.Tracks?.Items?.FirstOrDefault() ?? new SpotifySearchTrackItem();
    }

    private bool InsertSongsIntoPlaylist(string authToken, List<string> spotifyUris)
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        httpClient.DefaultRequestHeaders.Add("Authorization",$"Bearer {authToken}");

        var request = new
        {
            uris = spotifyUris
        };
        
        var result = httpClient.PostAsync($"https://api.spotify.com/v1/playlists/{PlaylistId}/tracks", new StringContent(JsonConvert.SerializeObject(request))).Result;
        
        var response = result.Content.ReadAsStringAsync().Result;

        SpotifyInsertTracksToPlaylistResponse? parsedResponse;
        try
        {
            parsedResponse = JsonConvert.DeserializeObject<SpotifyInsertTracksToPlaylistResponse>(response);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}