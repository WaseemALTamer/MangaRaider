using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MangaReader;
using System;
using System.Collections.Generic;

class HomeWindow : Canvas
{

    public UInt32 Backgruond = 0xff1f1f1f;
    public MangaContent[] MangasData;


    public MainWindow Parent;
    private MangaCoversScrollViewer MangaScrollViewerWidget;

    public HomeWindow(MainWindow parentWindow)
    {
        Parent = parentWindow;


        //Creates the Manga Holder
        MangaScrollViewerWidget = new MangaCoversScrollViewer(this);


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
        Background = new SolidColorBrush(Color.FromUInt32(Backgruond));
    }

    public void PassContent(MangaContent[] Data)
    {
        MangasData = Data;
        MangaScrollViewerWidget.PassContent(Data);
    }

}
