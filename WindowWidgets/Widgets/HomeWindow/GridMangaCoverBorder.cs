using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using System;
using Avalonia.LogicalTree;
using Avalonia.Interactivity;




class GridMangaCoverBorder : Border
{

    public UInt32 Backgruond = 0xff000000;
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

    private ContextMenu contextMenu;


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


        contextMenu = new ContextMenu();
        var menuItemDetails = new MenuItem
        {
            Header = "Copy Name",
        };
        menuItemDetails.Click += OnClickCopyName;


        contextMenu.ItemsSource = new[]
        {
            menuItemDetails
        };

        // Assign the context menu to the control
        ContextMenu = contextMenu;



        ContextMenu = contextMenu;



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

        Background = new SolidColorBrush(Color.FromUInt32(Backgruond));
    }

    private void ImageTransationTrigger(double Value) {
        CoverImageHolder.RenderTransform = new ScaleTransform(Value, Value);
    }

    private void TextTransationTrigger(double Value){
        Canvas.SetTop(MangaNameBlock, ImageBorders.Height - (MangaNameBlock.Height*Value));
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
}