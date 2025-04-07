using System.Collections.Generic;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using Avalonia.Controls;
using System.Text.Json;
using System.Linq;
using System.IO;
using System;
using Avalonia.Threading;
using System.Threading;
using Avalonia.Animation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.CodeAnalysis.Scripting.Hosting;



namespace MangaRaider
{
    public partial class MainWindow : Window
    {

        string MangaDirecotryFileName = @"MangasDir.txt";
        string MangaDirecotryIcon = @"Assets\Icons\MangaRaider.Ico";
        
        WindowsStruct Windows = new WindowsStruct();
        MangaContent[] MangasData;


        public MainWindow(){
            InitializeComponent();
        }

        private void OnLoaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Windows.MasterWindow = this;
            Icon = new WindowIcon(MangaDirecotryIcon);


            Windows.Assets = new AssetsLoader();

            Windows.FirstWindow = new HomeWindow(this);
            Windows.SecondWindow = new MangaHolder(this);
            Windows.ThirdWindow = new PagesReader(this);


            Content = Windows.FirstWindow;
            PassContent(); // we can pass the content for the windows

            Task.Run(() => LoadContent()); //this will load the content on a thread
        }



        //this is where the data gets loaded
        private void LoadContent() {

            string directoryPath = "Manga"; // we access the folder that is called Manga if we did not find a file that contain a direcotry


            if (File.Exists(MangaDirecotryFileName))
            {
                using (StreamReader reader = new StreamReader(MangaDirecotryFileName))
                {
                    string content = reader.ReadToEnd();
                    directoryPath = content;
                }
            }



            if (Directory.Exists(directoryPath))
            {
                //Graps all the folers
                string[] Folders = Directory.GetDirectories(directoryPath);
                MangasData = new MangaContent[Folders.Length];

                PassContent(); // we pass the content after we create the MangaContent


                int _indexManga = 0;
                foreach (string _folder in Folders)
                {
                    MangasData[_indexManga] = new MangaContent();
                    MangasData[_indexManga].FolderName = Path.GetFileName(_folder);
                    MangasData[_indexManga].Path = _folder;
                    string[] _chaptersDir = Directory.GetDirectories(_folder);

                    //Grps all the Chapters
                    MangasData[_indexManga].Chapters = new int[_chaptersDir.Length];


                    //Append the Chapters
                    int _indexChapters = 0;
                    foreach (string _chapter in _chaptersDir)
                    {
                        MangasData[_indexManga].Chapters[_indexChapters] = Int32.Parse(Path.GetFileName(_chapter));
                        _indexChapters += 1;
                    }

                    //Graps the JsonFile For Names
                    string _jsonFilePath = $"{_folder}/Data.json";
                    if (File.Exists(_jsonFilePath))
                    {
                        string _jsonContent = File.ReadAllText(_jsonFilePath);
                        var _data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(_jsonContent);



                        if (_data.ContainsKey("Names"))
                        {
                            var _altirnativeNamesArray = _data["Names"].EnumerateArray()
                                .Select(element => element.GetString()) // Convert each JsonElement to a string
                                .ToArray(); // Convert to a string array

                            MangasData[_indexManga].AltirnativeNames = _altirnativeNamesArray;
                            MangasData[_indexManga].MangaName = _altirnativeNamesArray[0];

                            
                        }
                        if (_data.ContainsKey("Description"))
                        {
                            MangasData[_indexManga].Description = _data["Description"].GetString();
                        }
                        if (_data.ContainsKey("LastUpdate"))
                        {
                            MangasData[_indexManga].LastUpdate = _data["LastUpdate"].GetString();
                        }

                        if (_data.ContainsKey("Chapters"))
                        {
                            var chaptersList = new List<Chapter>();
                            foreach (var chapterElement in _data["Chapters"].EnumerateArray())
                            {
                                var chapterDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(chapterElement.GetRawText());
                                var chapter = new Chapter();

                                chapter.ChapterID = chapterDict.ContainsKey("ChapterID") ? chapterDict["ChapterID"].GetInt32() : 0;


                                // update the code later for the line below to to be used for more efficnacy
                                // bad  news the graper graps the values as ints and you also need to change
                                // them to be as strings so before updating  this  code  update  the  graper
                                // first and update the /data.json files for most of the Series you have

                                //chapter.ChapterNumber = chapterDict.ContainsKey("ChapterNumber") ? chapterDict["ChapterNumber"].GetString() : null;


                                chapter.SeasonTag = chapterDict.ContainsKey("SeasonTag") ? chapterDict["SeasonTag"].GetString() : null;
                                chapter.Season = chapterDict.ContainsKey("Season") ? chapterDict["Season"].GetInt32() : 0;
                                chapter.Date = chapterDict.ContainsKey("Date") ? chapterDict["Date"].GetString() : null;

                                chaptersList.Add(chapter);
                            }
                            MangasData[_indexManga].ChaptersContent = chaptersList.ToArray();
                        }
                        
                        //Graps the Image
                        string _imagePath = $"{_folder}/Cover.jpg";
                        if (File.Exists(_imagePath))
                        {
                            using (var stream = File.OpenRead(_imagePath)) // we use the "using" so memory leaks does not occure
                            {
                                Bitmap bitmap = new Bitmap(stream);

                                int _localIndex = _indexManga;
                                Dispatcher.UIThread.InvokeAsync(() =>
                                {

                                    Image imageControl = new Image
                                    {
                                        Source = bitmap,
                                        Stretch = Avalonia.Media.Stretch.Fill
                                    };
                                    MangasData[_localIndex].CoverImage = imageControl;
                                });


                            }
                        }



                        //test.Text = $"{MangasData[0].AltirnativeNames[0]}";

                        //Graps the JsonFile For BookMakrs
                    
                    }
                    

                    _jsonFilePath = $"{_folder}/BookMark.json";
                    if (File.Exists(_jsonFilePath))
                    {
                        string _jsonContent = File.ReadAllText(_jsonFilePath);
                        var _data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(_jsonContent);

                        if (_data.ContainsKey("Chapter") && _data["Chapter"].ValueKind != JsonValueKind.Null)
                        {

                            MangasData[_indexManga].BookMarkedChapter = _data["Chapter"].GetInt32();
                        }

                        if (_data.ContainsKey("Page") && _data["Page"].ValueKind != JsonValueKind.Null)
                        {
                            MangasData[_indexManga].BookMarkedPage = _data["Page"].GetInt32();
                        }

                        if (_data.ContainsKey("ReadChapters") && _data["ReadChapters"].ValueKind == JsonValueKind.Array)
                        {
                            var _readChapters = _data["ReadChapters"].EnumerateArray()
                                .Select(element => element.GetInt32())
                                .ToArray();

                            MangasData[_indexManga].ChaptersRead = _readChapters;
                        }

                        if (_data.ContainsKey("Tags") && _data["Tags"].ValueKind == JsonValueKind.Array)
                        {
                            var _tags = _data["Tags"].EnumerateArray()
                                .Select(element => element.GetString()) // Convert each JsonElement to a string
                                .ToArray(); // Convert to a string array

                            MangasData[_indexManga].Tags = _tags;
                        }
                        //End of Content Grapping
                    }

                    if (MangasData[_indexManga].ChaptersContent != null) {
                        int[] _tempArray = new int[MangasData[_indexManga].Chapters.Length];
                        int LostIndex = 0;
                        for (int i = 0; i < MangasData[_indexManga].ChaptersContent.Length; i++) {
                            if (MangasData[_indexManga].Chapters.Contains(MangasData[_indexManga].ChaptersContent[i].ChapterID)) {
                                _tempArray[i - LostIndex] = MangasData[_indexManga].ChaptersContent[i].ChapterID;
                            }
                            else {
                                LostIndex++;
                            }
                        }
                        Array.Copy(_tempArray, MangasData[_indexManga].Chapters, _tempArray.Length);
                    }

                    _indexManga += 1;
                }
            }
        }
        void PassContent()
        {
            Windows.FirstWindow.PassContent(MangasData , Windows);
            Windows.SecondWindow.PassContent(Windows);
            Windows.ThirdWindow.PassContent(Windows);
        }
    }
}