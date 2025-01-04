using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PagesCanvasGrid : Canvas
{

    public SolidColorBrush BordersColor = new SolidColorBrush(Color.FromUInt32(0xff1f1f1f));
    public ChapterContent ChapterData;
    public WindowsStruct Windows;

    public ScrollViewer Parent;
    public bool LongStrip = false;



    public double PadY = 10;
    public double PadX = 10; // this will not be used as of 1/1/2025

    public double AfterImagesPadY = 0.2; // this is precentage from the image size 0.2 precent of the image size
    public double BeforeImagePadY = 0.2; // this is not implemented currently

    public SmoothScrolling SmoothScrollingAnimation;

    private GridPageBorders ImageHolder;

    private double ImageScaler = 1;

    private Button ChapterButton;
    private Button PageButton;
    private Button BookMarkButton;


    public PagesCanvasGrid(ScrollViewer parent)
    {
        Parent = parent;
        Parent.Content = this;
        Background = new SolidColorBrush(Color.FromUInt32(0x00000000)); // Set Color to be transparent so we can capture the cursor

        Parent.Focusable = true;  // Make sure this canvas can get focus

        ChapterButton = new Button
        {
            Background = BordersColor,
            MinWidth = 50,
        };
        Children.Add(ChapterButton);

        PageButton = new Button
        {
            Background = BordersColor,
            MinWidth = 50,
        };
        Children.Add(PageButton);

        BookMarkButton = new Button
        {
            Content = "BookMark",
            Background = BordersColor,
            MinWidth = 50,
        };
        BookMarkButton.Click += OnBookMarkButtonClicked;
        Children.Add(BookMarkButton);
        

        Parent.KeyDown += OnKeyDown;
        Parent.PropertyChanged += OnPropertyChanged;
        Parent.PointerReleased += OnClickRelase;
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

        if (ChapterData != null) {
            double _season = Math.Floor((double)ChapterData.Chapter / 100000);
            double _chapter = (ChapterData.Chapter - (_season * 100000))/10;
            ChapterButton.Content = $"Chapter: {_chapter}";
            PageButton.Content = $"Page: {ChapterData.CurrentPageNum}";
        }



        Canvas.SetLeft(ChapterButton, 10);
        Canvas.SetTop(ChapterButton, 10);

        Canvas.SetLeft(PageButton, 10);
        Canvas.SetTop(PageButton, 60);

        Canvas.SetLeft(BookMarkButton, 10);
        Canvas.SetTop(BookMarkButton, 110);

        PlaceData();
    }




    public void Populate(ChapterContent chapterData)
    {
        ChapterData = chapterData;

        ChapterData.NextPage = NextPage;
        ChapterData.PrevPage = PrevPage;

        DisplayPage();
        
      
    }

    public void NextPage(){
        ChapterData.CurrentPageNum += 1;
        if (ChapterData.CurrentPageNum > ChapterData.NumberOfPages) {
            ChapterData.CurrentPageNum = ChapterData.NumberOfPages;

            bool NextChapterExist = false;
            for (int i = 0; i < ChapterData.Chapters.Length; i++)
            {
                if (ChapterData.Chapter == ChapterData.Chapters[i])
                {
                    if (i + 1 < ChapterData.Chapters.Length)
                    {
                        ChapterData.Chapter = ChapterData.Chapters[i + 1];
                        ChapterData.GrapPages();
                        ChapterData.CurrentPageNum = 1;
                        NextChapterExist = true;
                        break;
                    };
                }
            }

            if (!NextChapterExist) {
                return;
            }
        }
        
        DisplayPage();

    }

    public void PrevPage() {

        ChapterData.CurrentPageNum -= 1;
        if (ChapterData.CurrentPageNum < 1)
        {
            ChapterData.CurrentPageNum = 1;
            bool PrevChapterExist = false;
            for (int i = 0; i < ChapterData.Chapters.Length; i++)
            {
                if (ChapterData.Chapter == ChapterData.Chapters[i])
                {
                    if (i - 1 >= 0)
                    {
                        ChapterData.Chapter = ChapterData.Chapters[i - 1];
                        ChapterData.GrapPages();
                        ChapterData.CurrentPageNum = ChapterData.NumberOfPages;
                        PrevChapterExist = true;
                        break;
                    };
                }
            }

            if (!PrevChapterExist)
            {
                return;
            }
        }


        DisplayPage();
    }


    public void DisplayPage() {
        Children.Clear();

        Children.Add(ChapterButton);
        Children.Add(PageButton);
        Children.Add(BookMarkButton);

        ImageHolder = new GridPageBorders(this, ChapterData.Path + $@"\{ChapterData.CurrentPageNum}.png", ImageScaler);
        Parent.Width = ImageHolder.Width;
        Height = ImageHolder.Height + (ImageHolder.Height * AfterImagesPadY);
        SmoothScrollingAnimation.StopCurrentAnimation = true;
        Parent.Offset = new Point(0, 0);
        UpdateWidget();
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            Parent.Focus();  // Set focus to the ScrollViewer after layout update
        });
    }


    private void OnKeyDown(object? sender, KeyEventArgs e)
    {



        if (e.Key == Key.Right) 
        {
            NextPage();
        }
        if (e.Key == Key.Left)
        {
            PrevPage();
        }if (e.Key == Key.Up) {
            ImageScaler += 0.05;
            DisplayPage();
        }
        if (e.Key == Key.Down){
            ImageScaler -= 0.05;
            DisplayPage();
        }
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

                if (pointerPosition.X >= Width / 2)
                {
                    NextPage();
                }
                else {
                    PrevPage();
                }

            }
        }
    }


    private void OnBookMarkButtonClicked(object sender, object e) {

        ChapterData.BookMark();
    }


    public void PlaceData()
    {
        if (ImageHolder != null)
        {
            Canvas.SetLeft(ImageHolder, Width / 2 - ImageHolder.Width / 2);
        }

        
        ShowOnlyVisibleScreen(Parent.Offset.Y);
    }

    public void ShowOnlyVisibleScreen(double ScrollViwerYOffset)
    {


    }

    public void PassContent(WindowsStruct WindowData)
    {
        Windows = WindowData;
    }


}
