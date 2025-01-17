using Avalonia.Controls;
using Avalonia.Media;
using MangaRaider;
using Avalonia;
using System;
using Avalonia.Input;

class MangaHolder : Canvas
{

    public SolidColorBrush Backgruond = new SolidColorBrush(Color.FromUInt32(0xff000000));
    public SolidColorBrush BordersColor = new SolidColorBrush(Color.FromUInt32(0xff1f1f1f));

    public MangaContent MangaData;
    public WindowsStruct Windows;


    public MainWindow Parent;


    private Button BackButton;
    private Button BookMarkedLoad;

    private Border LeftCanvasBorders;
    private Canvas LeftCanvas;
    private HolderMangaCoverBorder CoverHolder;

    private Border BottomRightCanvasBorder;
    private Canvas BottomRightCanvas;
    private ChaptersScrollViewer chaptersScrollViewer;




    public MangaHolder(MainWindow parentWindow)
    {
        Parent = parentWindow;


        BackButton = new Button
        {
            Content = "<",
            Background = BordersColor,
            Width = 30,
            Height = 30,
            ZIndex = 1,
        };
        BackButton.Click += OnClickBackButton;
        Children.Add(BackButton);

        BookMarkedLoad = new Button
        {
            Content = "BookMark",
            Background = BordersColor,
            Width = 90,
            Height = 30,
            ZIndex = 1,
        };
        BookMarkedLoad.Click += OnClickBookMarkedLoad;
        Children.Add(BookMarkedLoad);

        LeftCanvasBorders = new Border();
        Children.Add(LeftCanvasBorders);



        LeftCanvas = new Canvas();
        LeftCanvas.ClipToBounds = true;
        LeftCanvasBorders.Child = LeftCanvas;

        CoverHolder = new HolderMangaCoverBorder(LeftCanvas);


        BottomRightCanvasBorder = new Border();
        Children.Add(BottomRightCanvasBorder);

        BottomRightCanvas = new Canvas();
        BottomRightCanvas.ClipToBounds = true;
        BottomRightCanvasBorder.Child = BottomRightCanvas;

        chaptersScrollViewer = new ChaptersScrollViewer(BottomRightCanvas);





        AttachedToVisualTree += OnDisplay;
        Parent.SizeChanged += OnResize;
    }



    private void OnResize(object sender, SizeChangedEventArgs e)
    {
        UpdateWidget();
    }

    private void OnDisplay(object sender, VisualTreeAttachmentEventArgs e)
    {
        UpdateWidget();
    }

    private void UpdateWidget()
    {
        Background = Backgruond;

        Width = Parent.Width;
        Height = Parent.Height;

        LeftCanvasBorders.Width = Width * 0.3;
        LeftCanvasBorders.Height = Height;
        LeftCanvasBorders.Background = BordersColor;


        LeftCanvas.Width = LeftCanvasBorders.Width - 10;
        LeftCanvas.Height = LeftCanvasBorders.Height - 10;
        LeftCanvas.Background = Background;


        BottomRightCanvasBorder.Width = Width * 0.7;
        BottomRightCanvasBorder.Height = Height * 0.6;
        BottomRightCanvasBorder.Background = BordersColor;
        Canvas.SetLeft(BottomRightCanvasBorder, LeftCanvasBorders.Width);
        Canvas.SetTop(BottomRightCanvasBorder, Height - BottomRightCanvasBorder.Height);


        BottomRightCanvas.Width = BottomRightCanvasBorder.Width - 10;
        BottomRightCanvas.Height = BottomRightCanvasBorder.Height - 10;
        BottomRightCanvas.Background = Background;

        //SET BUTTON POS
        Canvas.SetLeft(BackButton, Width - BackButton.Width - 25);
        Canvas.SetTop(BackButton, 10);

        Canvas.SetLeft(BookMarkedLoad, LeftCanvasBorders.Width + 10);
        Canvas.SetTop(BookMarkedLoad, Height - BottomRightCanvasBorder.Height - BookMarkedLoad.Height - 10);

        if (MangaData != null && MangaData.BookMarkedChapter == 0) BookMarkedLoad.Opacity = 0;
        else BookMarkedLoad.Opacity = 1;
    }

    public void Populate(MangaContent mangaData)
    {
        MangaData = mangaData;
        // we can now populate the Window
        CoverHolder.Populate(MangaData);
        chaptersScrollViewer.Populate(MangaData);
    }

    private void OnClickBackButton(object sender, object e)
    {
        Windows.MasterWindow.Content = Windows.FirstWindow;
    }


    private void OnClickBookMarkedLoad(object sender, object e)
    {
        if (MangaData.BookMarkedChapter == 0) return;

        // Populate the Third Window and update it and move to it
        Windows.ThirdWindow.Populate(MangaData, MangaData.BookMarkedChapter, MangaData.BookMarkedPage);
        Windows.MasterWindow.Content = Windows.ThirdWindow;
    }

    public void PassContent(WindowsStruct windows)
    {
        Windows = windows;
        chaptersScrollViewer.PassContent(windows);
    }


}