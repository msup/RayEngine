using System;
using Emgu.CV.Structure;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using System.Diagnostics;



namespace Filter_TestApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        Filter.Diffusion diffusion = new Filter.Diffusion();
        
        
        public Window1()
        {
            InitializeComponent();

            diffusion.Changed += new Filter.Diffusion.ChangedEventHandler( UpdateProgressBar );


           
        }

        void UpdateProgressBar( object sender, double e )
        {
           
            progressBar.Value = e;
        }

        private void button1_Click( object sender, RoutedEventArgs e )
        {
            
            
            long elapsedMilliSeconds = 0;
            Bitmap bmp = diffusion.run( out elapsedMilliSeconds );

            //bmp.Save( "vystup.bmp", System.Drawing.Imaging.ImageFormat.Bmp );

            // show bitmap in image control in wpf xaml
            //MemoryStream strm = new MemoryStream();
            //bmp.Save( strm, System.Drawing.Imaging.ImageFormat.Bmp );

            //BitmapImage bmpImage = new BitmapImage();

            //bmpImage.BeginInit();
            //strm.Seek( 0, SeekOrigin.Begin );
            //bmpImage.StreamSource = strm;
            //bmpImage.EndInit();

            //myImage.Source = bmpImage;
            label.Text = elapsedMilliSeconds.ToString(); 
        }

    }
}
