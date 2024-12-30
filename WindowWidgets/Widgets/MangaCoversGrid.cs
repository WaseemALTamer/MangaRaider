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


class MangaCoversGrid : Canvas
{

    public MangaContent[] MangasData;
    public ScrollViewer Parent;



    private DispatcherTimer Subproccess;




    public MangaCoversGrid(ScrollViewer ParentWindow)
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

    private void UpdateGrid(object sender, object e) {
        int columnsPerRow = 4; // Number of items per row
        int currentRow = 0;

        bool _moreCoverBorderToCreate = false;
        for (int i = 0; i < MangasData.Length; i++)
        {
            if (MangasData[i].CoverBorder == null && MangasData[i].CoverImage != null)
            {
                MangasData[i].CoverBorder = new MangaCoverBorder(MangasData[i]); //Create the panles
                //Children.Add(MangasData[i].CoverBorder);
            }
            else {
                _moreCoverBorderToCreate = true;
            }
        }

        if (!_moreCoverBorderToCreate) {
            Subproccess.Stop();
        }

        PlaceCovers();
    }


    void PlaceCovers() {

        if (MangasData == null) return;


        double PadY = 20;
        double PadX = 20;


        for (int i = 0; i < MangasData.Length; i++)
        {
            var Cover = MangasData[i].CoverBorder;
            
            if (Cover != null) {
                if (Cover.Parent != this) {
                    if (Cover.Parent != null) {
                        Cover.Parent.Children.Remove(Cover);
                    }
                    Children.Add(Cover);
                    Cover.Parent = this;
                }

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
    }


    public void PassContent(MangaContent[] Data)
    {
        MangasData = Data;
    }
}
