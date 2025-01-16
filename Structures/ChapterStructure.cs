using Avalonia.Controls;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

class ChapterContent{


    public MangaContent MangaData;

    public int[] Chapters;

    public int CurrentPageNum;
    public int NumberOfPages;
    public int Chapter;

    public string Path;


    public Action NextPage;
    public Action PrevPage;


    public void GrapPages()
    {
        string folderPath = MangaData.Path + $@"\{Chapter}";
        if (Directory.Exists(folderPath))
        {
            NumberOfPages = Directory.GetFiles(folderPath).Length;
            Path = folderPath;
            //Task.Run(() => LoadPagesImagesThread()); //this will load the content on a thread
        }

        if (MangaData.ChaptersRead == null) {
            MangaData.ChaptersRead = new int[0];
        }

        if (MangaData.ChaptersRead.Contains(Chapter)) return;

        int[] _tempArray = new int[MangaData.ChaptersRead.Length + 1];
        Array.Copy(MangaData.ChaptersRead, _tempArray, MangaData.ChaptersRead.Length);
        _tempArray[MangaData.ChaptersRead.Length] = Chapter;

        MangaData.ChaptersRead = _tempArray;
        WriteDataToDisk();
    }


    public void BookMark() {
        MangaData.BookMarkedChapter = Chapter;
        MangaData.BookMarkedPage = CurrentPageNum;
        WriteDataToDisk();
    }

    public void WriteDataToDisk() {

        var bookmarkData = new
        {
            Chapter = MangaData.BookMarkedChapter,
            Page = MangaData.BookMarkedPage,
            ReadChapters = MangaData.ChaptersRead,
            Tags = MangaData.Tags
        };

        string jsonString = JsonSerializer.Serialize(bookmarkData, new JsonSerializerOptions { WriteIndented = true });
        string filePath = MangaData.Path + @"\BookMark.json";
        File.WriteAllText(filePath, jsonString);
    }
}