using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;



class GridChaptersBorder : Border
{

    public Canvas Parent;
    public WindowsStruct Windows;

    public double BWidth = 300; // this will be updated based on the Parent Width  ;-)
    public double BHeight = 30; // the height will be a constant and will always be used

    public SolidColorBrush OutlineColor = new SolidColorBrush(Color.FromUInt32(0xff1f1f1f));
    public SolidColorBrush BackgruondColor = new SolidColorBrush(Color.FromUInt32(0xff292929));
    public SolidColorBrush OnHoverColor = new SolidColorBrush(Color.FromUInt32(0xff000000));

    public SolidColorBrush ChapterNormalColor = new SolidColorBrush(Color.FromUInt32(0xff0075a4));
    public SolidColorBrush ChapterReadColor = new SolidColorBrush(Color.FromUInt32(0xff808080));
    public SolidColorBrush ChapterBookMarked = new SolidColorBrush(Color.FromUInt32(0xffbaa200));

    public CornerRadius CornerRadiusCut = new CornerRadius(30);

    private MangaContent MangaData;
    private int Chapter;

    private TextBlock ChapterText;
    private TextBlock DateText;

    private Border ChapterTextBorder;
    private Canvas ChapterTextCanvas;

    private UniformTransation OpacityAnimation;

    private double ActaulChapter;
    private double Season;

    private DateTime ChapterRealseDate;
    private Border NewLabelImageBoarder;
    private Avalonia.Controls.Image NewLabelImage;

    public GridChaptersBorder(Canvas parent, WindowsStruct windows, MangaContent data, int chapter) {
        Parent = parent;
        MangaData = data;
        Chapter = chapter;
        Windows = windows;

        ClipToBounds = true;
        Parent.Children.Add(this);
        CornerRadius = CornerRadiusCut;

        Background = OutlineColor;

        ChapterTextBorder = new Border { 
            Background = BackgruondColor,
            ClipToBounds = true,
            CornerRadius = CornerRadiusCut,
        };
        Child = ChapterTextBorder;

        ChapterTextCanvas = new Canvas ();
        ChapterTextBorder.Child = ChapterTextCanvas;

        ChapterText = new TextBlock {
                //Text = $"  {Chapter.ToString()}",
                Foreground = ChapterNormalColor,
                FontSize = 20,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };
        ChapterTextCanvas.Children.Add(ChapterText);

        DateText = new TextBlock {
            Text ="Null",
            Foreground = ChapterReadColor,
            FontSize = 20,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Width = 150
        };
        ChapterTextCanvas.Children.Add(DateText);


        Season = Math.Floor(((double)Chapter /100000));
        ActaulChapter = (Chapter - (100000 * Season))/10;


        ChapterText.Text = $"{ActaulChapter.ToString()}";
        if (Season > 1) {
            ChapterText.Text = $"Season {Season} Chapter {ActaulChapter}";
        }

        foreach (Chapter _data in MangaData.ChaptersContent) {
            if (_data.ChapterID == Chapter && _data.Date != null) {
                ChapterText.Text = $" {_data.SeasonTag} {ActaulChapter}";
                ChapterRealseDate = DateTime.Parse(_data.Date);
                DateText.Text = $"{ChapterRealseDate.Year}/{ChapterRealseDate.Month}/{ChapterRealseDate.Day}";
                DateText.Width = 150;
                break;
            }
        }


        if (ChapterRealseDate != null) {
            DateTime _currentDateTime = DateTime.Now;
            TimeSpan _timeDifference = _currentDateTime - ChapterRealseDate;
            if (_timeDifference.Days <= 30){
                NewLabelImageBoarder = new Border
                {
                    Height = 42
                };
                NewLabelImage = new Avalonia.Controls.Image
                {
                    Source = Windows.Assets.NewLabel,
                    Stretch = Stretch.Uniform
                };
                NewLabelImageBoarder.Child = NewLabelImage;
                ChapterTextCanvas.Children.Add(NewLabelImageBoarder);
            }
        }




        //this detects if the chapter is read 
        if (MangaData.ChaptersRead != null && MangaData.ChaptersRead.Contains(Chapter)){
            ChapterText.Foreground = ChapterReadColor;
        }

        //this detects if the chapter is book marked
        if (MangaData.BookMarkedChapter == Chapter) {
            ChapterText.Foreground = ChapterBookMarked;

        }

        //Create the animation
        OpacityAnimation = new UniformTransation
        {
            StartingValue = 1,
            EndingValue = 0.5,
            Duration = 50,
        };
        OpacityAnimation.Trigger = OpacityAnimationTrigger;

        Parent.SizeChanged += OnPropertyChanged;
        AttachedToVisualTree += OnDisplay;

        PointerEntered += OpacityAnimation.TranslateForward;
        PointerExited += OpacityAnimation.TranslateBackward;

        PointerReleased += OnClickRelase;
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
        BWidth = Parent.Width;
        // Height is constant (lets hope)

        Width = BWidth - 50; // -15 to leave space for the scrollbar
        Height = BHeight;

        ChapterTextBorder.Width = Width - 8;
        ChapterTextBorder.Height = Height - 8;

        ChapterTextCanvas.Width = ChapterTextBorder.Width - 8;
        ChapterTextCanvas.Height = ChapterTextBorder.Height;
        Canvas.SetLeft(DateText, ChapterTextCanvas.Width - DateText.Width);

        if (NewLabelImageBoarder != null) {
            NewLabelImageBoarder.Height = Height;
            Canvas.SetLeft(NewLabelImageBoarder, ChapterTextBorder.Width - 50);
        }


        ChapterText.Width = ChapterTextBorder.Width;
        ChapterText.Height = ChapterTextBorder.Height;
    }


    private void OnClickRelase(object sender, PointerReleasedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
        {
            if (sender is Control control)
            {
                var pointerPosition = e.GetPosition(control);

                if (pointerPosition.X < 0 || pointerPosition.Y < 0) return;
                if (pointerPosition.X > Width || pointerPosition.Y > Height) return;


                // Populate the Second Window and update it
                Windows.ThirdWindow.Populate(MangaData, Chapter);
                Windows.MasterWindow.Content = Windows.ThirdWindow;
            }
        }
    }


    void OpacityAnimationTrigger(double Value) {
        ChapterTextBorder.Opacity = Value;
    }

}
