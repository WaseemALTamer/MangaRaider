using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;



class MangaCoverBorder : Border
{

    public UInt32 Backgruond = 0xff000000;
    public MangaContent MangasData;

    public Canvas Parent;

    public int BWidth = 800/3;
    public int BHeight = 800/2;

    private Canvas ImageBorders;
    private Panel CoverImageHolder;
    private TextBox MangaNameBlock;


    private UniformTransation ImageTransation;
    private UniformTransation TextTransation;


    public MangaCoverBorder(MangaContent Data)
    {
        MangasData = Data;
        //You shuold Manually add it to to the parent and also it is recomended to add it to the Parent varable "Parent"

        ImageBorders = new Canvas();

        CoverImageHolder = new Panel();
        CoverImageHolder.Children.Add(MangasData.CoverImage); // Add Image
        ImageBorders.Children.Add(CoverImageHolder);


        MangaNameBlock = new TextBox
        {
            Text = MangasData.MangaName,
            Background = new SolidColorBrush(Color.FromUInt32(0xcc000000)),
            IsReadOnly = true,
            IsTabStop = false,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap, // Enables text wrapping
            MinHeight = 0,
            AcceptsReturn = true, // Allows multiline text
            FocusAdorner = null,
            Focusable = false,
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



        AttachedToVisualTree += OnDisplay;

        Child = ImageBorders;


        ImageBorders.ClipToBounds = true;
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

    private void TextTransationTrigger(double Value)
    {
        Canvas.SetTop(MangaNameBlock, ImageBorders.Height - (MangaNameBlock.Height*Value));
    }
}
