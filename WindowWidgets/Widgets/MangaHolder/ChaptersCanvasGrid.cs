using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class ChaptersCanvasGrid : Canvas
{

    public MangaContent MangaData;
    public WindowsStruct Windows;

    public ScrollViewer Parent;


    public double PadY = 10;
    public double PadX = 10; // this will not be used as of 1/1/2025

    public ChaptersCanvasGrid(ScrollViewer parent) { 
        Parent = parent;
        Parent.Content = this;
        Background = new SolidColorBrush(Color.FromUInt32(0x00000000)); // Set Color to be transparent so we can capture the cursor




        Parent.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, object e)
    {
        UpdateWidget();
    }

    private void OnDisplay(object sender, VisualTreeAttachmentEventArgs e)
    {
        UpdateWidget();
    }



    private void UpdateWidget()
    {
        Width = Parent.Width;

        PlaceData();
    }


    public GridChaptersBorder[] ChaptersBorders;

    public void Populate(MangaContent mangaData)
    {
        if (!ReferenceEquals(MangaData, null) && 
            MangaData.FolderName == mangaData.FolderName && 
            Windows.MasterWindow.Content == Windows.FirstWindow) return;


        MangaData = mangaData;

        if (ChaptersBorders != null) {
            Children.Clear();
        }

        ChaptersBorders = new GridChaptersBorder[MangaData.Chapters.Length];

        for (int i = 0; i < ChaptersBorders.Length; i++)
        {
            ChaptersBorders[(ChaptersBorders.Length - 1) - i] = new GridChaptersBorder(this, Windows, MangaData, MangaData.Chapters[i]);
            ChaptersBorders[(ChaptersBorders.Length - 1) - i].IsVisible = false;
        }
        PlaceData();
    }


    public void PlaceData()
    {
        if (ChaptersBorders == null || ChaptersBorders.Length == 0) return;

        double _height = ChaptersBorders[0].BHeight;
        for (int i = 0; i < ChaptersBorders.Length; i++){
            Canvas.SetTop(ChaptersBorders[i], (_height * i) + (PadY * (i + 1)));
            Canvas.SetLeft(ChaptersBorders[i], PadX);
        }
        double _totalHeight = 0;
        Height = (_height * ChaptersBorders.Length) + (PadY * (ChaptersBorders.Length + 1));
        ShowOnlyVisibleScreen(Parent.Offset.Y);
    }

    public void ShowOnlyVisibleScreen(double ScrollViwerYOffset) {

        if (ChaptersBorders == null) return;


        double _height = ChaptersBorders[0].BHeight;

        double _LowerBounds = ScrollViwerYOffset - _height;
        double _UpperBounds = ScrollViwerYOffset + Parent.Height;
        

        for (int i = 0; i < ChaptersBorders.Length; i++){
            double _yPos = (_height * i) + (PadY * (i + 1));

            if (_yPos >= _LowerBounds && _yPos <= _UpperBounds)
            {
                ChaptersBorders[i].IsVisible = true;
            }
            else {
                ChaptersBorders[i].IsVisible = false;
            }
        }
    }

    public void PassContent(WindowsStruct WindowData)
    {
        Windows = WindowData;
    }
}
