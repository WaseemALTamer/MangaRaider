using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MangaRaider;
using System;
using System.Collections.Generic;

class HomeWindow : Canvas
{

    public UInt32 Backgruond = 0xff000000;
    public MangaContent[] MangasData;
    public WindowsStruct Windows;


    public MainWindow Parent;

    public MangaContentSearcher MangaSearcher;
    private MangaCoversScrollViewer MangaScrollViewerWidget;
    

    public HomeWindow(MainWindow parentWindow)
    {
        Parent = parentWindow;


        //Creates the Manga Holder
        MangaScrollViewerWidget = new MangaCoversScrollViewer(this);
        MangaSearcher = new MangaContentSearcher(this);

        MangaSearcher.MangaCoversGrid = MangaScrollViewerWidget.MangaGrid;

        MangaScrollViewerWidget.Focusable = true;
        MangaSearcher.Focusable = true;


        MangaScrollViewerWidget.PointerPressed += OnClick;
        MangaSearcher.PointerPressed += OnClick;
        

        AttachedToVisualTree += OnDisplay;
        Parent.SizeChanged += OnResize;
    }

    private void OnResize(object sender, SizeChangedEventArgs e)
    {
        UpdateWidget();
    }

    private void OnDisplay(object sender, VisualTreeAttachmentEventArgs e) {
        UpdateWidget();
    }

    private void UpdateWidget()
    {
        Width = Parent.Width;
        Height = Parent.Height;

        MangaScrollViewerWidget.Width = Width;
        MangaScrollViewerWidget.Height = Height;

        Background = new SolidColorBrush(Color.FromUInt32(Backgruond));
    }

    private void OnClick(object sender, object e) {

        if (sender is MangaCoversScrollViewer MangaSearcher)
        {
            if (MangaSearcher.Focusable)
            {
                MangaSearcher.Focus();
            }
        }

        if (sender is MangaCoversScrollViewer MangaScrollViewerWidget)
        {
            if (MangaScrollViewerWidget.Focusable)
            {
                MangaScrollViewerWidget.Focus();
            }
        }
    }

    public void PassContent(MangaContent[] mangasData, WindowsStruct WindowData)
    {
        MangasData = mangasData;
        Windows = WindowData;
        MangaScrollViewerWidget.PassContent(MangasData, Windows);
    }
}
