using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;



class HolderMangaCoverBorder : Border
{

    public SolidColorBrush Backgruond = new SolidColorBrush(Color.FromUInt32(0xff1f1f1f));
    public MangaContent MangaData;

    public Canvas Parent;

    public Image CoverImageCopy;

    public double BWidth = 800 / 3;
    public double BHeight = 800 / 2;

    private Canvas ImageBorders;
    private Panel CoverImageHolder;
    private TextBox MangaNameBlock;
    





    public HolderMangaCoverBorder(Canvas ParentWindow)
    {
        Parent = ParentWindow;
        Parent.Children.Add(this);

        //You shuold Manually add it to to the parent and also it is recomended to add it to the Parent varable "Parent"

        ImageBorders = new Canvas();
        CoverImageHolder = new Panel();

        CoverImageCopy = new Image
        {
            Stretch = Stretch.Fill
        };

        CoverImageHolder.Children.Add(CoverImageCopy); // Add Image

        ImageBorders.Children.Add(CoverImageHolder);


        Parent.SizeChanged += OnPropertyChanged;
        AttachedToVisualTree += OnDisplay;
        Child = ImageBorders;
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
        BHeight = 1.5 * BWidth;


        Width = BWidth;
        Height = BHeight;


        ImageBorders.Width = BWidth - 10;
        ImageBorders.Height = BHeight - 10;
        CoverImageHolder.Width = ImageBorders.Width;
        CoverImageHolder.Height = ImageBorders.Height;
        MangaNameBlock.Width = BWidth - 10;
        Canvas.SetTop(MangaNameBlock, Height);

        Background = Backgruond;
    }

    public void Populate(MangaContent mangaData)
    {
        MangaData = mangaData;

        if (CoverImageCopy.Source != MangaData.CoverImage.Source) {
            CoverImageCopy.Source = MangaData.CoverImage.Source; //copy the image
        }


        //For the Name we recreate it because then it wont resize the height to fit the text so this is a simple way to fix it
        if (MangaNameBlock != null) ImageBorders.Children.Remove(MangaNameBlock); ;

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
        };
        MangaNameBlock.SizeChanged += (sender, e) =>
        {
            MangaNameBlock.Height = e.NewSize.Height;
        };
        ImageBorders.Children.Add(MangaNameBlock); // add it to the canvase
        UpdateWidget();

    }

}