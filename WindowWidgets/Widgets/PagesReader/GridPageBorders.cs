using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using Avalonia;
using Avalonia.Media;

class GridPageBorders: Border
{
    public SolidColorBrush Backgruond = new SolidColorBrush(Color.FromUInt32(0x00000000));

    public Canvas Parent;
    public Image PageImage;

    public int PageWidth;
    public int PageHeight;

    public double ImageScaler;

    public double XPos; // those are for the parent to access and check the postion of the oobject
    public double YPos; // Note that you have to assigne these your self

    public GridPageBorders(Canvas parent, string Directory, double Scaler) {


        Parent = parent;
        ImageScaler = Scaler;
        Background = Backgruond;

        Parent.Children.Add(this);

        IsHitTestVisible = false;
        IsVisible = false;


        using (var stream = File.OpenRead(Directory)) // we use the "using" so memory leaks does not occure
        {
            Bitmap bitmap = new Bitmap(stream);

            PageWidth = bitmap.PixelSize.Width;
            PageHeight = bitmap.PixelSize.Height;



            Width = Parent.Width;

            if (Parent.Width < PageWidth * ImageScaler){
                Height = (Parent.Width / PageWidth) * PageHeight;
            }
            else{
                Height = PageHeight * ImageScaler;
            }

            PageImage = new Image
            {
                Source = bitmap,
                Stretch = Avalonia.Media.Stretch.Uniform,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                
            };
        };

        Child = PageImage;

        PropertyChanged += OnPropertyChanged;
    }


    private void OnPropertyChanged(object sender, object e)
    {
        UpdateWidget();
    }

    private void OnDisplay(object sender, VisualTreeAttachmentEventArgs e)
    {
        UpdateWidget();
    }


    public void UpdateWidget()
    {

        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
        Width = Parent.Width;

        if (Parent.Width < PageWidth * ImageScaler){
            Height = (Parent.Width / PageWidth) * PageHeight;
        }
        else {
            Height = PageHeight * ImageScaler;
        }
    }
}
