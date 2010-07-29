using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace AllocationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bmp = new Bitmap("test.bmp");
            Console.WriteLine("Only Bitmap is loaded");
            //Console.ReadKey();

            DataSet orig = new DataSet(  bmp);
            Console.WriteLine("Allocated Memory {0}", bmp.Width * bmp.Height * sizeof(short) / 1000000);
            //Console.ReadKey();

            DataSet Gx = new DataSet( (short) bmp.Width, (short) bmp.Height, 1 );
            DataSet Gy = new DataSet( (short) bmp.Width, (short) bmp.Height, 1 );
            DataSet C = new DataSet( (short) bmp.Width, (short) bmp.Height, 1 );
            

            Console.WriteLine( "Allocated Memory {0}", bmp.Width * bmp.Height * sizeof( short ) / 1000000 );
            //Console.ReadKey();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
           // orig.Convolve1D_Vertikal( result );

            orig.Convolve1D_VH( Gx, Gy );

            C.Diffusivity( Gx, Gy );

            // Gx = Cx = Gx .* C 
            Gx.MultiplyBy( C );
            //Gy.MultiplyBy( C );

            // Gx = Gx.*C + Gy.C = Cx+Cy
            //Gx.Add( Gy );

            stopWatch.Stop();

            Console.WriteLine("{0}", stopWatch.ElapsedMilliseconds);

            //Console.ReadKey();
            C.SaveAsBitmap( "C.bmp" );
            Gx.SaveAsBitmap("Gx.bmp");
            Gy.SaveAsBitmap("Gy.bmp");

           
        }
    }
}
