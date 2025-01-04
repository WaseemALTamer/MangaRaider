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


class PagesScrollViwer : Panel
{

    public ChapterContent ChapterData;
    public SolidColorBrush Backgruond = new SolidColorBrush(Color.FromUInt32(0xff000000));


    public Canvas Parent;
    public ScrollViewer ChapterScrollViewer;


    private SmoothScrolling SmoothScrollingAnimation;
    private PagesCanvasGrid CanvasGrid;


    public PagesScrollViwer(Canvas ParentWindow)
    {
        Parent = ParentWindow;
        Parent.Children.Add(this);


        ChapterScrollViewer = new ScrollViewer(); // Create the scroll viwer
        ChapterScrollViewer.Background = Background;
        Children.Add(ChapterScrollViewer);


        CanvasGrid = new PagesCanvasGrid(ChapterScrollViewer);
        





        // creates the smooth animation and map it to the functions
        SmoothScrollingAnimation = new SmoothScrolling();
        CanvasGrid.SmoothScrollingAnimation = SmoothScrollingAnimation;

        ChapterScrollViewer.PointerWheelChanged += SmoothScrollingAnimation.OnPointerWheelChanged;
        CanvasGrid.PointerWheelChanged += SmoothScrollingAnimation.OnPointerWheelChanged;
        PointerWheelChanged += SmoothScrollingAnimation.OnPointerWheelChanged;
        SmoothScrollingAnimation.Trigger += SmoothVerticalScrollerTrigger;






        AttachedToVisualTree += OnDisplay;
        Parent.PropertyChanged += OnPropertyChanged;
        ChapterScrollViewer.PropertyChanged += OnPropertyChanged;
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

        ChapterScrollViewer.Width = Width;
        ChapterScrollViewer.Height = Height;

        Background = Backgruond;



        CanvasGrid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
        ChapterScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;

    }


    private void SmoothVerticalScrollerTrigger(double Value){
        ChapterScrollViewer.Offset = new Vector(ChapterScrollViewer.Offset.X, ChapterScrollViewer.Offset.Y + Value);
    }





    public void Populate(ChapterContent chapterData)
    {
        ChapterData = chapterData;
        CanvasGrid.Populate(chapterData);
    }
}
