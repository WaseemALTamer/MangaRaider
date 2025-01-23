using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using System;
using Avalonia.LogicalTree;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.Linq;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;




class GridMangaCoverBorder : Border
{

    public SolidColorBrush Backgruond = new SolidColorBrush(Color.FromUInt32(0xff000000));
    public MangaContent MangaData;
    public WindowsStruct Windows;

    public Canvas Parent;
    public Image CoverImageCopy;

    public int BWidth = 800/3;
    public int BHeight = 800/2;

    private Canvas ImageBorders;
    private Panel CoverImageHolder;
    private TextBox MangaNameBlock;


    private UniformTransation ImageTransation;
    private UniformTransation TextTransation;


    private SolidColorBrush PinnedImageBackground = new SolidColorBrush(Color.FromUInt32(0x99000000));
    private Border PinnedBorder;
    private Image PinnedImage;



    private ContextMenu contextMenu;
    private MenuItem MenuItemCopy = new MenuItem{Header = "Copy Name"};
    private MenuItem MenuItemPins = new MenuItem { Header = "Pin" };

    private Border NewLabelBorder;
    private Image NewLabelImage;

    public bool NewUpdate = false;
    


    public GridMangaCoverBorder(MangaContent mangaData, WindowsStruct window)
    {
        MangaData = mangaData;
        Windows = window;
        //You shuold Manually add it to to the parent and also it is recomended to add it to the Parent varable "Parent"

        ImageBorders = new Canvas();
        CoverImageHolder = new Panel();
        // we create a copy of the image and then add it
        CoverImageCopy = new Image { 
            Source = MangaData.CoverImage.Source,
            Stretch = Stretch.Fill,
        };
        CoverImageHolder.Children.Add(CoverImageCopy); // Add Image
        ImageBorders.Children.Add(CoverImageHolder);


        MangaNameBlock = new TextBox
        {
            Text = MangaData.MangaName,
            Background = new SolidColorBrush(Color.FromUInt32(0xcc000000)),
            IsReadOnly = true,
            IsTabStop = false,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap, // Enables text wrapping
            MinHeight = 0,
            AcceptsReturn = true, // Allows multiline text
            FocusAdorner = null,
            Focusable = false,
            IsHitTestVisible = false,
        };

        ImageBorders.Children.Add(MangaNameBlock); // add it to the canvase

        MangaNameBlock.SizeChanged += (sender, e) =>
        {
            MangaNameBlock.Height = e.NewSize.Height;
        };






        //Create the translations for the image
        ImageTransation = new UniformTransation
        {
            StartingValue = 1,
            EndingValue = 1.2,
            Duration = 150,
        };
        ImageTransation.Trigger += ImageTransationTrigger; // attach the trigger of the transformation to the function

        PointerEntered += ImageTransation.TranslateForward; // attach it to the event that we need
        PointerExited += ImageTransation.TranslateBackward;


        //Create the translation for the text
        TextTransation = new UniformTransation
        {
            StartingValue = 0,
            EndingValue = 1,
            Duration = 150,
        };
        TextTransation.Trigger += TextTransationTrigger; // same as before



        PointerEntered += TextTransation.TranslateForward;
        PointerExited += TextTransation.TranslateBackward;
        PointerReleased += OnClickRelase;
        AttachedToVisualTree += OnDisplay;






        // this (PinnedBorder) will be mapped to the same animation to the Text when it pops up as it uses values from
        // 0-1 at the same time that i will use it to show this
        PinnedBorder = new Border { 
            Width = 40,
            Height = 40,
            CornerRadius = new CornerRadius(10),
            Background = PinnedImageBackground,
            IsHitTestVisible = true,
        };
        ImageBorders.Children.Add(PinnedBorder);
        PinnedBorder.PointerReleased += OnClickRealsePin;


        PinnedImage = new Image
        {
            Stretch = Avalonia.Media.Stretch.Uniform,
            Focusable = true,
            IsHitTestVisible = true,
        };  

        PinnedBorder.Child = PinnedImage;

        contextMenu = new ContextMenu();
        contextMenu.ItemsSource = new[]
        {
            MenuItemCopy,
            MenuItemPins,
        };


        MenuItemCopy.Click += OnClickCopyName;
        MenuItemPins.Click += OnClickPins;
        ContextMenu = contextMenu;

        if (MangaData.Tags != null && MangaData.Tags.Contains("Pined"))
        {
            MenuItemPins.Header = "UnPin";
            PinnedImage.Source = Windows.Assets.PinActive;
            PinnedBorder.Opacity = 1;
        }
        else {
            PinnedImage.Source = Windows.Assets.PinUnActive;
            PinnedBorder.Opacity = 0;
        }




        DateTime _currentDateTime = DateTime.Now;
        DateTime _seiresLastChapterUpdate = DateTime.Parse(MangaData.ChaptersContent[^1].Date);

        if ((_currentDateTime - _seiresLastChapterUpdate).Days <= 3) {
            NewUpdate = true;
            NewLabelBorder = new Border()
            {
                Width = 50,
            };
            NewLabelImage = new Image
            {
                Source = Windows.Assets.NewLabel,
            };
            NewLabelBorder.Child = NewLabelImage;
            ImageBorders.Children.Add(NewLabelBorder);
        }



        



        Child = ImageBorders;
        ImageBorders.ClipToBounds = true;
        ClipToBounds = true;
    }


    private void OnPropertyChanged(object sender, object e){
        UpdateWidget();
    }

    private void OnDisplay(object sender, VisualTreeAttachmentEventArgs e){
        UpdateWidget();
    }

    private void UpdateWidget(){
        Width = BWidth + 10;
        Height = BHeight + 10;


        ImageBorders.Width = BWidth;
        ImageBorders.Height = BHeight;
        CoverImageHolder.Width = ImageBorders.Width;
        CoverImageHolder.Height = ImageBorders.Height;

        MangaNameBlock.Width = BWidth;
        Canvas.SetTop(MangaNameBlock, ImageBorders.Height);


        Canvas.SetTop(PinnedBorder, 10);
        Canvas.SetLeft(PinnedBorder, 10);

        if (NewLabelBorder != null){
            Canvas.SetLeft(NewLabelBorder, Width - NewLabelBorder.Width - 10);
        }

        Background = Backgruond;
    }

    private void ImageTransationTrigger(double Value) {
        CoverImageHolder.RenderTransform = new ScaleTransform(Value, Value);
    }

    private void TextTransationTrigger(double Value){
        Canvas.SetTop(MangaNameBlock, ImageBorders.Height - (MangaNameBlock.Height*Value));


        if (MangaData.Tags == null || !MangaData.Tags.Contains("Pined"))
        {
            PinnedBorder.Opacity = Value;
        }
    }


    private void OnClickRealsePin(object sender, PointerReleasedEventArgs e) {
        
        e.Handled = true;

        if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
        {
            if (sender is Control control)
            {
                var pointerPosition = e.GetPosition(control);
                if (pointerPosition.X < 0 || pointerPosition.Y < 0) return;
                if (pointerPosition.X > PinnedBorder.Width || pointerPosition.Y > PinnedBorder.Height) return;
                OnClickPins(null, null);
            }
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


                // Populate the Second Window and update it
                Windows.SecondWindow.Populate(MangaData);
                Windows.MasterWindow.Content = Windows.SecondWindow;
            }
        }
    }


    private async void OnClickCopyName(object? sender, RoutedEventArgs args)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        var dataObject = new DataObject();
        dataObject.Set(DataFormats.Text, MangaNameBlock.Text);
        await clipboard.SetDataObjectAsync(dataObject);
    }

    private async void OnClickPins(object? sender, RoutedEventArgs args)
    {
        if (MangaData.Tags == null || !MangaData.Tags.Contains("Pined")) {
            if (MangaData.Tags == null)
            {
                MangaData.Tags = new string[] { "Pined" };
            }
            else {
                Array.Resize(ref MangaData.Tags, MangaData.Tags.Length + 1);
                MangaData.Tags[^1] = "Pined";
            }
            PinnedImage.Source = Windows.Assets.PinActive;
            MenuItemPins.Header = "UnPin";
        }
        else{
            MangaData.Tags = MangaData.Tags.Where(s => s != "Pined").ToArray();
            MenuItemPins.Header = "Pin";
            PinnedImage.Source = Windows.Assets.PinUnActive;
        }


        // write data on disk
        var _object = new ChapterContent();
        _object.MangaData = MangaData;
        _object.WriteDataToDisk();

    }
}