using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Reflection;


class MangaCoversCanvasGrid : Canvas
{

    public MangaContent[] MangasData;
    public WindowsStruct Windows;


    public ScrollViewer Parent;
    private DispatcherTimer Subproccess;


    public double PadY = 20;
    public double PadX = 20;



    public MangaCoversCanvasGrid(ScrollViewer ParentWindow)
    {
        Parent = ParentWindow;
        Parent.Content = this;

        Background = new SolidColorBrush(Color.FromUInt32(0x00000000)); // Set Color to be transparent so we can capture the cursor


        //this runs a functions every 10ms
        Subproccess = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(10) // 10ms interval
        };
        Subproccess.Tick += UpdateGrid;
        Subproccess.Start();
        //================

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
        Width = Parent.Width;
        //Height = Parent.Height + 1000;

        PlaceCovers();

    }

    public GridMangaCoverBorder[] MangasCovers; // create the ImagesCover array
    public GridMangaCoverBorder[] VisableCovers;


    private void UpdateGrid(object sender, object e) {

        bool _moreCoverBorderToCreate = false;
        bool UpdatePlacement = false;

        if (MangasData != null)
        {

            if (MangasCovers == null || MangasCovers.Length != MangasData.Length)
            {
                Children.Clear(); // clear the array so the grabage collector can remove the covers
                MangasCovers = new GridMangaCoverBorder[MangasData.Length];
            }



            for (int i = 0; i < MangasData.Length; i++)
            {
                if (MangasData[i] != null && MangasData[i].CoverImage != null)
                {
                    // Check if the cover for this manga already exists, if not, create it
                    if (MangasCovers[i] == null)
                    {
                        MangasCovers[i] = new GridMangaCoverBorder(MangasData[i], Windows); // Create new cover
                        Children.Add(MangasCovers[i]); // Add the new cover to the Children collection
                        MangasCovers[i].IsVisible = false;
                        UpdatePlacement = true;
                    }
                }
                else
                {
                    _moreCoverBorderToCreate = true;
                }
            }

            if (!_moreCoverBorderToCreate)
            {
                Subproccess.Interval = TimeSpan.FromMilliseconds(1000); // Update interval to 1 second
                Subproccess.Stop();
            }
        }

        // Only update placement if necessary
        if (UpdatePlacement)
        {
            ShowAllCovers();
            PlaceCovers();
        }
    }


    public void ClearVisiableCovers() { // this function will not delet the child from the parent rather it will only make it invisable
                                 // and remove it from the array which objects are made visible  this  also  insure  that  other
                                 // objects take its place

        if (VisableCovers != null)
        {
            for (int i = 0; i < VisableCovers.Length; i++)
            {
                if (VisableCovers[i] == null) break;
                VisableCovers[i].IsVisible = false;
            }
        }
         VisableCovers = new GridMangaCoverBorder[MangasCovers.Length];
    }

    public void ShowAllCovers()
    {

        ClearVisiableCovers();


        int VisibleIndex = 0; // this will insure that if a show is not visible other show cover takes its place

        for (int i = 0; i < MangasCovers.Length; i++)
        {
            if (MangasCovers[i] != null)
            {
                VisableCovers[i - VisibleIndex] = MangasCovers[i]; // Move the refrence to the Visable array so they get displayed
            }
            else VisibleIndex += 1; 
        }

        PlaceCovers();

    }

    public void PlaceCovers() {

        
        if (VisableCovers == null) return;


        Height = 0;
        for (int i = 0; i < VisableCovers.Length; i++)
        {
            if (VisableCovers[i] == null) continue;

            var Cover = VisableCovers[i];
            
            if (Cover != null) {
                double _ColumnsNum = Math.Floor(Width / (Cover.Width + PadX));
                double _RowNum = Math.Floor(i / _ColumnsNum);

                if (_ColumnsNum == 0) {
                    _RowNum = i;
                }
                
                double _PosX = (PadX * (i% _ColumnsNum)) + (Cover.Width * (i % _ColumnsNum)) + PadX;
                double _PosY = (PadY * ((_RowNum) + 1)) + (Cover.Height * (_RowNum));

                SetLeft(Cover, _PosX);
                SetTop(Cover, _PosY);

                Height = _PosY + Cover.Height + PadY;
            }
        }

        ShowOnlyVisibleScreen(Parent.Offset.Y);
    }


    public void ShowOnlyVisibleScreen(double ScrollViwerYOffset)
    {

        if (VisableCovers == null) return;

        for (int i = 0; i < VisableCovers.Length; i++)
        {

            var Cover = VisableCovers[i];
            if (Cover == null) return;

            double _LowerBounds = ScrollViwerYOffset - Cover.BHeight;
            double _UpperBounds = ScrollViwerYOffset + Parent.Height;

            double _ColumnsNum = Math.Floor(Width / (Cover.Width + PadX));
            if (_ColumnsNum <= 0) _ColumnsNum = 1;
            double _RowNum = Math.Floor(i / _ColumnsNum);

            

            double _PosY = (PadY * ((_RowNum) + 1)) + (Cover.Height * (_RowNum));



            if (_PosY >= _LowerBounds && _PosY <= _UpperBounds)
            {
                VisableCovers[i].IsVisible = true;
            }
            else
            {
                VisableCovers[i].IsVisible = false;
            }
        }
    }


    public void PassContent(MangaContent[] mangasData, WindowsStruct WindowData)
    {
        MangasData = mangasData;
        Windows = WindowData;
    }
}
