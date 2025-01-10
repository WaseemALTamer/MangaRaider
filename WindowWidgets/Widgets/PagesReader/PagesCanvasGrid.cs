using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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

    public GridPageBorders ImageHolder;
    public GridPageBorders[] ImageHolders; // this is for the long strip

    private double ImageScaler = 1;

    private Button ChapterButton;
    private Button PageButton;
    private Button BookMarkButton;

    private Button ImageScalerButton;


    private ToggleButton LongStripToggleButton;


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

        ImageScalerButton = new Button
        {
            Content = "Test",
            Background = BordersColor,
            MinWidth = 50,
        };
        Children.Add(ImageScalerButton);


        LongStripToggleButton = new ToggleButton
        {
            Content = "LongStrip",
            Background = BordersColor,
            MinWidth = 50,
        };
        LongStripToggleButton.Click += OnClickLongStripToggleButton;
        Children.Add(LongStripToggleButton);

        

        ChapterButton.KeyDown += OnKeyDown;
        PageButton.KeyDown += OnKeyDown;
        BookMarkButton.KeyDown += OnKeyDown;
        ImageScalerButton.KeyDown += OnKeyDown;
        LongStripToggleButton.KeyDown += OnKeyDown;


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
            ImageScalerButton.Content = $"Size x{Math.Round(ImageScaler,2)}";
        }

        //Add the button to the grid if needed 
        if (ChapterButton.Parent == null) Children.Add(ChapterButton);
        if (PageButton.Parent == null) Children.Add(PageButton);
        if (BookMarkButton.Parent == null) Children.Add(BookMarkButton);
        if (ImageScalerButton.Parent == null) Children.Add(ImageScalerButton);
        if (LongStripToggleButton.Parent == null) Children.Add(LongStripToggleButton);



        Canvas.SetLeft(ChapterButton, 10);
        Canvas.SetTop(ChapterButton, 10 + Parent.Offset.Y);

        Canvas.SetLeft(PageButton, 10);
        Canvas.SetTop(PageButton, 60 + Parent.Offset.Y);

        Canvas.SetLeft(BookMarkButton, 10);
        Canvas.SetTop(BookMarkButton, Parent.Height + Parent.Offset.Y - 50);


        Canvas.SetLeft(LongStripToggleButton, 10);
        Canvas.SetTop(LongStripToggleButton, Parent.Height + Parent.Offset.Y - 100);

        Canvas.SetLeft(ImageScalerButton, 10);
        Canvas.SetTop(ImageScalerButton, Parent.Height + Parent.Offset.Y - 150);

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
        if (ChapterData.CurrentPageNum > ChapterData.NumberOfPages || LongStrip == true) {
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

        if (!LongStrip){
            DisplayPage();
        }
        else {
            DisplayAllPages();
        }

    }

    public void PrevPage() {

        ChapterData.CurrentPageNum -= 1;
        if (ChapterData.CurrentPageNum < 1 || LongStrip == true)
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

        if (!LongStrip){
            DisplayPage();
        }
        else{
            DisplayAllPages();
        }

    }


    public void DisplayPage() {

        if (LongStrip) {
            DisplayAllPages();
            return;
        }

        Children.Clear();

        UpdateWidget();

        if (ImageHolders != null)
        {
            ImageHolders = null; // we can remove the single image for the grabage collector
        }

        ImageHolder = new GridPageBorders(this, ChapterData.Path + $@"\{ChapterData.CurrentPageNum}.png", ImageScaler);
        SmoothScrollingAnimation.StopCurrentAnimation = true; // stop the scroll bar from going forward and set velocity to zero
        Parent.Offset = new Point(0, 0); // reset the scrollbar progress

        PlaceData();
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>{ // this ensure that we set focase on the parent after the avolana thread is excuated
            Parent.Focus();
        });
    }

    

    async public void DisplayAllPages() {
        Children.Clear();
        UpdateWidget();

        if (ImageHolder != null) {
            ImageHolder = null; // we can remove the single image for the grabage collector
        }
        SmoothScrollingAnimation.StopCurrentAnimation = true;
        Parent.Offset = new Point(0, 0);

        ImageHolders = new GridPageBorders[ChapterData.NumberOfPages];

        for (int i = 0; i < ChapterData.NumberOfPages; i++) {
            if (LongStrip == false || ImageHolders == null) return;


            ImageHolders[i] = new GridPageBorders(this, ChapterData.Path + $@"\{i + 1}.png", ImageScaler);
            ImageHolders[i].IsVisible = false;
            PlaceData();
            await Task.Delay(20); // we can await delay to leave other operations to run perfectly
        }

        Avalonia.Threading.Dispatcher.UIThread.Post(() => { // this ensure that we set focase on the parent after the avolana thread is excuated
            Parent.Focus();
        });
    }


    public void PlaceData(double ScroolBarProggressPrecentage = -1)
    {
        if (ImageHolder != null)
        {
            ImageHolder.XPos = Width / 2 - ImageHolder.Width / 2;
            Canvas.SetLeft(ImageHolder, ImageHolder.XPos);
            Height = ImageHolder.Height + (ImageHolder.Height * AfterImagesPadY);
        }

        if (ImageHolders != null)
        {
            double _yPos = 0;
            foreach (var _imageHolder in ImageHolders)
            {
                // this sets the X position
                if (_imageHolder == null) return;

                _imageHolder.XPos = Width / 2 - _imageHolder.Width / 2;
                Canvas.SetLeft(_imageHolder, _imageHolder.XPos);

                _imageHolder.YPos = _yPos;
                Canvas.SetTop(_imageHolder, _imageHolder.YPos);

                _yPos += _imageHolder.Height;

                Height = _yPos + (_imageHolder.Height * AfterImagesPadY);
                ShowOnlyVisibleScreen(Parent.Offset.Y);
            }
        }

        if (ScroolBarProggressPrecentage != -1){
            Parent.Offset = new Point(0, ScroolBarProggressPrecentage * Height);
        }

        
    }

    public void ShowOnlyVisibleScreen(double ScrollViwerYOffset)
    {
        if (ImageHolders == null) return;

        bool _break = false; // this will stop the loop from contunuaning if we detect that any images that comes after the ones we are seeing are not vissable 
        foreach (var _imageHolder in ImageHolders)
        {
            if (_imageHolder == null) return;
            double _lowerbound = Parent.Offset.Y - _imageHolder.Height;
            double _upperbound = Parent.Offset.Y + Parent.Height;

            if (_imageHolder.YPos <= _upperbound && _imageHolder.YPos >= _lowerbound)
            {
                _imageHolder.IsVisible = true;
                _break = true;
            }
            else{
                _imageHolder.IsVisible = false;
                if (_break) {
                    return; // this will only break when we go from stuff that should be visiable to stuff that are isnt to not was proccessing power
                }
            }
        }

    }


    public void ResizeData() {
        if (ImageHolder != null) {
            ImageHolder.ImageScaler = ImageScaler;
            ImageHolder.UpdateWidget();
        }
        if (ImageHolders != null)
        {
            foreach (var _imageHolder in ImageHolders) { 
                if (_imageHolder == null) break;
                _imageHolder.ImageScaler = ImageScaler;
                _imageHolder.UpdateWidget();
            }

            
        }

        PlaceData(Parent.Offset.Y / Height);
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
        }if (e.Key == Key.OemPlus) {
            ImageScaler += 0.05;
            ResizeData();
        }
        if (e.Key == Key.OemMinus){
            ImageScaler -= 0.05;
            ResizeData();
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

    private void OnClickLongStripToggleButton(object sender, object e) { 
        LongStrip = !LongStrip;
        if (LongStrip){
            DisplayAllPages();
        }
        else {
            DisplayPage();
        }
        
    }



    public void PassContent(WindowsStruct WindowData)
    {
        Windows = WindowData;
    }


}
