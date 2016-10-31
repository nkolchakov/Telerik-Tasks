using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ProcessingXML
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            string filename = "../../catalogue.xml";
            var doc = GetXmlDom(filename);

            // 2.
            var artists = GetArtistsAndAlbumsCount(doc.GetElementsByTagName("artist"));
            AlbumsCount(artists);

            // 3.
            var artistsXPath = GetArtistsAndAlbumsCountXPath("Mo");
            Console.WriteLine("With XPath");
            AlbumsCount(artistsXPath);

            // 4. delete albums with price bigger than 20
            DeleteAlbums(filename, 20);

            // 5. all songs with xmldocument
            var titles = ExtractSongTitles(filename);
            Console.WriteLine("\nSong titles: ");
            //foreach (var title in titles)
            //{
            //    Console.WriteLine(title);
            //}

            // 6. all songs with linq to xml
            ///Console.WriteLine("\nLINQ-------------------------\n");
            var titlesLINQ = ExtractSongTitlesLINQ(filename);

            // 7. extract text file to xml file 
            var personInfoText = File.ReadAllLines("../../person-info.txt");
            var newXml = TextFileToXml(personInfoText);
            Console.WriteLine(newXml.ToString());

            // 8. from catalog.xml to album.xml
            FromXmlToXml(filename, "../../albums.xml");
            Console.WriteLine(File.ReadAllText("../../albums.xml"));

            // 9. traverse directory
            string startFrom = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //TraverseDirectoryToXml(startFrom, "../../dirs.xml");



        }

        public static void TraverseDirectoryToXml(string startFrom, string outputFile)
        {
            File.Create(outputFile).Close();

            using (XmlWriter writer = XmlWriter.Create(outputFile))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };
                writer.WriteStartDocument();
                writer.WriteStartElement("Directory");


                TraverseDirectory(startFrom, writer);

                writer.WriteEndDocument();

            }
        }

        private static void TraverseDirectory(string startDir, XmlWriter writer)
        {
            foreach (var directory in Directory.GetDirectories(startDir))
            {
                Console.WriteLine(directory);
                writer.WriteStartElement("dir", directory);
                TraverseDirectory(directory, writer);
                writer.WriteEndElement();
            }

            foreach (var file in Directory.GetFiles(startDir))
            {
                Console.WriteLine(file);
                writer.WriteStartElement("file", file);
                writer.WriteEndElement();

            }

        }

        public static void FromXmlToXml(string fromFile, string toFile)
        {
            File.Create(toFile).Close();

            using (XmlWriter writer = XmlWriter.Create(toFile))
            {

                writer.WriteStartDocument();
                writer.WriteStartElement("albums");
                string albumName = "";
                string author = "";

                using (XmlReader reader = XmlReader.Create(fromFile))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "name":
                                    albumName = reader.ReadInnerXml();
                                    break;

                                case "artist":
                                    author = reader.ReadInnerXml();

                                    // now we already now the album name because it's the previous sibling
                                    writer.WriteElementString("album_name", albumName);
                                    writer.WriteElementString("artist", author);

                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public static XElement TextFileToXml(string[] textLines)
        {
            var doc = new XElement("person");
            foreach (var line in textLines)
            {
                var tagAndValue = line.Split(new char[] { ':' },
                    StringSplitOptions.RemoveEmptyEntries);

                var newElement = new XElement(tagAndValue[0], tagAndValue[1]);
                doc.Add(newElement);

            }

            return doc;
        }

        public static IEnumerable<string> ExtractSongTitlesLINQ(string filename)
        {
            XDocument doc = XDocument.Load(filename);

            var titles = doc.Descendants()
                .Where(e => e.Name == "title")
                .Select(f => f.Value)
                .ToList();

            return titles;
        }

        public static IEnumerable<string> ExtractSongTitles(string filename)
        {
            var titles = new HashSet<string>();

            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "song")
                        {
                            while (reader.Read())
                            {
                                if (reader.Name == "title")
                                {
                                    titles.Add(reader.ReadInnerXml());
                                }
                            }
                        }

                    }
                }
            }

            return titles;

        }

        public static void DeleteAlbums(string filename, double priceBiggerThan)
        {
            var doc = GetXmlDom(filename);
            var catalog = doc.GetElementsByTagName("catalog")[0];

            for (int i = catalog.ChildNodes.Count - 1; i >= 0; i--)
            {
                var el = catalog.ChildNodes[i];
                if (double.Parse(el["price"].InnerXml) > 20)
                {
                    el.ParentNode.RemoveChild(el);
                }
            }
        }

        public static XmlDocument GetXmlDom(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            return doc;
        }

        public static Dictionary<string, int> GetArtistsAndAlbumsCountXPath(string artist)
        {
            var docNav = new XPathDocument("../../catalogue.xml");
            var sss = docNav.CreateNavigator().Select($"/catalog/album/artist[text() = '{artist}']");

            return new Dictionary<string, int>()
            {
                {artist, sss.Count }
            };
        }

        public static Dictionary<string, int> GetArtistsAndAlbumsCount(XmlNodeList catalog)
        {
            var artists = catalog
                            .Cast<XmlNode>()
                            .GroupBy(x => x.InnerText)
                            .ToDictionary(gr => gr.Key, gr => gr.Count());

            return artists;

        }

        public static void AlbumsCount(Dictionary<string, int> dict)
        {
            foreach (var item in dict)
            {
                Console.WriteLine($"{item.Key} has {item.Value} albums.");
            }
        }

    }
}
