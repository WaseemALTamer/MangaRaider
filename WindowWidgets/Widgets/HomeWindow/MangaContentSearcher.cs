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
using Fastenshtein;
using System.Collections;
using System.Linq;


class MangaContentSearcher : Canvas
{

    public SolidColorBrush Backgruond = new SolidColorBrush(Color.FromUInt32(0xff1E1E1E));

    public SolidColorBrush GreyTextBackground = new SolidColorBrush(Color.FromUInt32(0xff929292));
    public SolidColorBrush NormalTextBackground = new SolidColorBrush(Color.FromUInt32(0xffffffff));


    public Canvas Parent;
    
    
    public MangaCoversCanvasGrid MangaCoversGrid;
    public WindowsStruct Windows;

    public TextBox SearchBar;




    public MangaContentSearcher(Canvas ParentWindow)
    {
        Parent = ParentWindow;
        Parent.Children.Add(this);

        Background = Backgruond;

        SearchBar = new TextBox {
            Watermark = "Search",
            Foreground = GreyTextBackground,
            Width = 250,
            Height = 30,
        };

        Children.Add(SearchBar);


        SearchBar.GotFocus += OnGetFocus;
        SearchBar.LostFocus += OnLossFocus;
        SearchBar.TextChanged += OnType;


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
        Canvas.SetTop(this, (Parent.Height - Height) * 0.02);

        Canvas.SetLeft(SearchBar, Width * 0.70);
        if (Width * 0.30 <= SearchBar.Width) {
            Canvas.SetLeft(SearchBar, Width - SearchBar.Width);
        }

        Canvas.SetTop(SearchBar, Height * 0.2);



        Width = Parent.Width * 0.95;
        Height = Parent.Height * 0.09;
    }


    private void OnGetFocus(object sender, object e) {
        SearchBar.Foreground = NormalTextBackground;
    }

    private void OnLossFocus(object sender, object e){
        SearchBar.Foreground = GreyTextBackground;
    }


    private void OnType(object sender, object e) {

        var _text = SearchBar.Text.ToLower();
        var queryTokens = _text.ToLower().Split(' ');
        queryTokens = queryTokens.Take(queryTokens.Length - 1).ToArray();


        if (queryTokens.Length == 0){
            MangaCoversGrid.ShowAllCovers();
            return;
        }

        MangaCoversGrid.ClearVisiableCovers();
        var _lostIndex = 0; // this index is subtracted from the i index to give the next element that is null rather than skip elemetns that are null



        for (int i = 0; i < MangaCoversGrid.MangasData.Length; i++) {

            bool _showFound = false;
            foreach (string _showName in MangaCoversGrid.MangasCovers[i].MangaData.AltirnativeNames)
            {
                var titleTokens = _showName.ToLower().ToLower().Split(' ');
                int matches = queryTokens.Count(q => titleTokens.Contains(q));
                double matchPercentage = (double)matches / queryTokens.Length;

                if (matchPercentage >= 0.6)
                {
                    MangaCoversGrid.VisableCovers[i - _lostIndex] = MangaCoversGrid.MangasCovers[i];
                    _showFound = true;
                    break;
                }
            }
            if (!_showFound) { 
                _lostIndex += 1; 
            }
        }

        MangaCoversGrid.PlaceCovers();
    }
}
