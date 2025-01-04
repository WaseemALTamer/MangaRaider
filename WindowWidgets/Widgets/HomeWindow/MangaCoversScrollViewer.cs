using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


class MangaCoversScrollViewer : Panel
{

    public UInt32 Backgruond = 0xff1b1b1b;
    public MangaContent[] MangasData;
    public WindowsStruct Windows;


    public Canvas Parent;
    public ScrollViewer MangaScrollViewer;
    public MangaCoversCanvasGrid MangaGrid;

    private SmoothScrolling SmoothScrollingAnimation;


    public MangaCoversScrollViewer(Canvas ParentWindow)
    {
        Parent = ParentWindow;
        Parent.Children.Add(this);


        MangaScrollViewer = new ScrollViewer(); // Create the scroll viwer
        Children.Add(MangaScrollViewer);

        SmoothScrollingAnimation = new SmoothScrolling();

        MangaGrid = new MangaCoversCanvasGrid(MangaScrollViewer);
        


        MangaScrollViewer.PointerWheelChanged += SmoothScrollingAnimation.OnPointerWheelChanged;
        MangaGrid.PointerWheelChanged += SmoothScrollingAnimation.OnPointerWheelChanged;
        SmoothScrollingAnimation.Trigger += SmoothVerticalScrollerTrigger;



        ClipToBounds = true;


        AttachedToVisualTree += OnDisplay;
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
        Canvas.SetLeft(this, (Parent.Width - Width) * 0.5);
        Canvas.SetTop(this, (Parent.Height - Height) * 0.85);

        Width = Parent.Width * 0.95;
        Height = Parent.Height * 0.85;

        Background = new SolidColorBrush(Color.FromUInt32(Backgruond));


        MangaGrid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;


        //ScrollViwer
        MangaScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
        MangaScrollViewer.Width = Width;
        MangaScrollViewer.Height = Height;

    }


    private void SmoothVerticalScrollerTrigger(double Value) {
        MangaScrollViewer.Offset = new Vector(MangaScrollViewer.Offset.X, MangaScrollViewer.Offset.Y + Value);
    }

    public void PassContent(MangaContent[] mangasData, WindowsStruct WindowData)
    {
        MangasData = mangasData;
        Windows = WindowData;
        MangaGrid.PassContent(MangasData, Windows);
    }
}


