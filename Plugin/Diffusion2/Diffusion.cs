using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Data;
using WpfOpenTK;
using System.Drawing;
using System.Diagnostics;

namespace Filter
{
    public class Diffusion : IFilter
    {
        private VolumeData Data = null;

        public delegate void ChangedEventHandler(object sender, double e);

        public event ChangedEventHandler Changed;

        protected virtual void OnChanged(double e)
        {
            if (Changed != null)
                Changed(this, e);
        }


        public Bitmap run(out long elapsedMilliSeconds)
        {
            Data = new VolumeData();
            var d = Directory.GetCurrentDirectory();

            try
            {
                var path = "test.bmp";
                //Data.Load( path );
                Data.Load2(path);

                Data.ScaleMax();
            }
            catch (IOException e)
            {
            }

            Bitmap diffused = this.diffuse(Data, out elapsedMilliSeconds);

            Data = null;

            return diffused;
        }

        private Bitmap diffuse(VolumeData data, out long elapsedMilliSeconds)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            VolumeData convolved = VolumeData.Convolve1D(Data, Direction.Horizontal);

            sw.Stop();
            elapsedMilliSeconds = sw.ElapsedMilliseconds;


            convolved.Scale255();

            //OnChanged(new EventArgs());


            OnChanged(66);

            #region conversion to bitmap
             /*
            Bitmap bmp = new Bitmap(convolved.Width, convolved.Height,
                                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int i = 0; i < convolved.Width; i++)
                for (int j = 0; j < convolved.Height; j++)

                    try
                    {
                        int value = (int) convolved.Volume[i, j, 0];

                        bmp.SetPixel(i, j, Color.FromArgb(value, value, value));
                    }
                    catch (Exception e)
                    {
                        int b;
                    }

           */
            #endregion
           
            Bitmap bmp = new Bitmap(1, 1);
            return bmp;
        }
    }
}