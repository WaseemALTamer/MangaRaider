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


class ChaptersScrollViewer : Panel
{

    public MangaContent MangaData;
    public WindowsStruct Windows;

    public SolidColorBrush Backgruond = new SolidColorBrush(Color.FromUInt32(0xff000000));



    public Canvas Parent;
    public ScrollViewer ChapterScrollViewer;


    private SmoothScrolling SmoothScrollingAnimation;
    private ChaptersCanvasGrid CanvasGrid;


    public ChaptersScrollViewer(Canvas ParentWindow)
    {
        Parent = ParentWindow;
        Parent.Children.Add(this);


        ChapterScrollViewer = new ScrollViewer(); // Create the scroll viwer
        Children.Add(ChapterScrollViewer);


        CanvasGrid = new ChaptersCanvasGrid(ChapterScrollViewer);
        




        // creates the smooth animation and map it to the functions
        SmoothScrollingAnimation = new SmoothScrolling();
        ChapterScrollViewer.PointerWheelChanged += SmoothScrollingAnimation.OnPointerWheelChanged;
        CanvasGrid.PointerWheelChanged += SmoothScrollingAnimation.OnPointerWheelChanged;
        SmoothScrollingAnimation.Trigger += SmoothVerticalScrollerTrigger;



        


        AttachedToVisualTree += OnDisplay;
        Parent.PropertyChanged += OnPropertyChanged;
        ClipToBounds = true;
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
        Height = Parent.Height;

        Background = Backgruond;



        //ScrollViwer

        ChapterScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
        ChapterScrollViewer.Width = Width;
        ChapterScrollViewer.Height = Height;
    }


    private void SmoothVerticalScrollerTrigger(double Value)
    {
        ChapterScrollViewer.Offset = new Vector(ChapterScrollViewer.Offset.X, ChapterScrollViewer.Offset.Y + Value);
    }




    
    public void Populate(MangaContent mangaData)
    {
        MangaData = mangaData;
        CanvasGrid.Populate(mangaData);
    }
    
    public void PassContent(WindowsStruct WindowData)
    {
        Windows = WindowData;
        CanvasGrid.PassContent(Windows);
    }

    
}


