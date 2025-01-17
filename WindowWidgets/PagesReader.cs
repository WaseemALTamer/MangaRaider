using Avalonia.Controls;
using Avalonia.Media;
using MangaRaider;
using Avalonia;
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

class PagesReader : Canvas
{

    public SolidColorBrush Backgruond = new SolidColorBrush(Color.FromUInt32(0xff000000));
    public SolidColorBrush BordersColor = new SolidColorBrush(Color.FromUInt32(0xff1f1f1f));

    public MangaContent MangaData;
    public WindowsStruct Windows;
    public ChapterContent ChapterData  = new ChapterContent();


    public MainWindow Parent;




    private Button BackButton;
    private Button FullScrrenButton;

    private PagesScrollViwer pageScrollViwer;

    private Button ChapterButton;
    private Button PageButton;



    public PagesReader(MainWindow parentWindow)
    {
        Parent = parentWindow;



        BackButton = new Button
        {
            Content = "<",
            Background = BordersColor,
            Width = 30,
            Height = 30,
            ZIndex = 1,
        };
        BackButton.Click += OnClickBackButton;
        Children.Add(BackButton);

        FullScrrenButton = new Button
        {
            Content = "⛶",
            Background = BordersColor,
            Width = 30,
            Height = 30,
            ZIndex = 1,
        };

        FullScrrenButton.Click += OnClickFullScreenButton;
        Children.Add(FullScrrenButton);


        ChapterButton = new Button
        {
            Background = BordersColor,
            MinWidth = 100,
        };
        Children.Add(ChapterButton);

        PageButton = new Button
        {
            Background = BordersColor,
            MinWidth = 100,
        };
        Children.Add(PageButton);


        pageScrollViwer = new PagesScrollViwer(this);



        AttachedToVisualTree += OnDisplay;
        Parent.SizeChanged += OnResize;
    }



    private void OnResize(object sender, SizeChangedEventArgs e)
    {
        UpdateWidget();
    }

    private void OnDisplay(object sender, VisualTreeAttachmentEventArgs e)
    {
        UpdateWidget();
    }

    private void UpdateWidget()
    {
        Background = Backgruond;

        Width = Parent.Width;
        Height = Parent.Height;

        ChapterButton.Content = $"Chapter: ";
        PageButton.Content = $"Page: ";


        //SET BUTTONS POS
        Canvas.SetLeft(BackButton, Width - BackButton.Width - 25);
        Canvas.SetTop(BackButton, 10);


        Canvas.SetLeft(FullScrrenButton, Width - FullScrrenButton.Width - 75);
        Canvas.SetTop(FullScrrenButton, 10);


        Canvas.SetLeft(ChapterButton, 50);
        Canvas.SetTop(ChapterButton, 10);

        Canvas.SetLeft(PageButton, 50);
        Canvas.SetTop(PageButton, 60);

    }

    public void Populate(MangaContent mangaData, int Chapter, int PageNumber = 1){
        MangaData = mangaData;
        ChapterData.Chapter = Chapter;
        ChapterData.CurrentPageNum = PageNumber;
        ChapterData.Chapters = MangaData.Chapters;
        ChapterData.MangaData = MangaData;
        ChapterData.GrapPages();

        pageScrollViwer.Populate(ChapterData);
    }

    



    // lets not use this approch lets just load the image when we need it only other wise we dont need it at all
    private void LoadPagesImagesThread() {

        string _imagePath = MangaData.Path + $@"\{ChapterData.Chapter}";

        for (int i = 1; (ChapterData.NumberOfPages + 1) > 0; i++) {
            
            if (File.Exists(_imagePath))
            {
                _imagePath += $@"\{i}";

                using (var stream = File.OpenRead(_imagePath)) // we use the "using" so memory leaks does not occure
                {
                    Bitmap bitmap = new Bitmap(stream);
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {

                        Image imageControl = new Image
                        {
                            Source = bitmap,
                            Stretch = Avalonia.Media.Stretch.Uniform,
                        };
                        //ChapterData.ImageArray[i] = new GridPageBorders(imageControl);
                    });
                }
            }
        }
    }


    private void OnClickBackButton(object sender, object e)
    {
        if (Windows.MasterWindow.WindowState == WindowState.FullScreen)
        {
            Windows.MasterWindow.WindowState = WindowState.Normal;
        }
        Windows.SecondWindow.Populate(MangaData);
        Windows.MasterWindow.Content = Windows.SecondWindow;


        pageScrollViwer.CanvasGrid.Children.Clear();
        pageScrollViwer.CanvasGrid.ImageHolder = default;
        pageScrollViwer.CanvasGrid.ImageHolders = default;
    }

    private void OnClickFullScreenButton(object sender, object e) {
        if (Windows.MasterWindow.WindowState == WindowState.FullScreen)
        {
            Windows.MasterWindow.WindowState = WindowState.Normal;
        }
        else {
            Windows.MasterWindow.WindowState = WindowState.FullScreen;
        }

        Avalonia.Threading.Dispatcher.UIThread.Post(() => { // this ensure that we set focase on the parent after the avolana thread is excuated
            pageScrollViwer.ChapterScrollViewer.Focus();
        });
    }

    public void PassContent(WindowsStruct windows)
    {
        Windows = windows;
    }
}
