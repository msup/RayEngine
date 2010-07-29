using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AllocationTest
{
    public class DataSet
    {
        short[ , , ] volume = null;

        int Width;
        int Height;
        int Depth;

        double scale = 65536;

        public DataSet( Bitmap bmp )
        {
            volume = new short[ bmp.Width, bmp.Height, 1 ];


            Width = bmp.Width;
            Height = bmp.Height;
            Depth = 1;

            // http://www.bobpowell.net/lockingbits.htm


            BitmapData bmpData = bmp.LockBits( new Rectangle( 0, 0, bmp.Width, bmp.Height ), ImageLockMode.ReadOnly, bmp.PixelFormat );
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            unsafe
            {
                int PixelSize = 1;

                for ( int y = 0; y < bmpData.Height; y++ )
                {
                    byte* row = (byte*) bmpData.Scan0 + ( y * bmpData.Stride );
                    for ( int x = 0; x < bmpData.Width; x++ )
                    {
                        byte a = row[ x * PixelSize ];

                        volume[ x, y, 0 ] = (short) a;

                    }
                }

            }

            bmp.UnlockBits( bmpData );

            //for ( int i = 0; i < Width; i++ )
            //    for ( int j = 0; j < Height; j++ )
            //        volume[ i, j, 0 ] = (double) bmp.GetPixel( i, j ).R;
        }


        public DataSet( int Width, int Height, int Depth )
        {
            this.Width = Width;
            this.Height = Height;
            this.Depth = Depth;

            volume = new short[ Width, Height, Depth ];
        }

        private short GetMaximum()
        {
            short max = 0;

            for ( int i = 0; i < Width; ++i )
                for ( int j = 0; j < Height; ++j )
                    for ( int k = 0; k < Depth; ++k )
                    {
                        if ( volume[ i, j, k ] > max )
                            max = volume[ i, j, k ];
                    }
            return max;
        }

        private short GetMinimum()
        {
            short min = 0;

            for ( int i = 0; i < Width; ++i )
                for ( int j = 0; j < Height; ++j )
                    for ( int k = 0; k < Depth; ++k )
                    {
                        if ( volume[ i, j, k ] < min )
                            min = volume[ i, j, k ];
                    }
            return min;
        }

        public void Scale255()
        {
            short max = GetMaximum();
            short min = GetMinimum();
            short c = (short) ( ( 32768 / ( max - min ) ) * 255 );

            for ( int i = 0; i < Width; i++ )
                for ( int j = 0; j < Height; j++ )

                    this.volume[ i, j, 0 ] = (short) ( ( ( this.volume[ i, j, 0 ] - min ) * c ) >> 15 );
            //this.volume[ i, j, 0 ] = (short) ( ( ( this.volume[ i, j, 0 ] - min ) * c ) / 32768 );
        }

        //public void ScaleMax()
        //{
        //    short max = GetMaximum();
        //    short min = GetMinimum();

        //    for ( int i = 0; i < Width; i++ )
        //        for ( int j = 0; j < Height; j++ )
        //            this.volume[ i, j, 0 ] /= max;
        //}


        public void Convolve1D_VH( DataSet Gx, DataSet Gy )
        {
            int kernelSize = 5;

            short[] kernel1D = { +1, -8, 0, 8, -1 };

            short[] doubleIndexes = { -2, -1, 0, +1, +2 };


            for ( int k = 0; k < Depth; k++ )
                for ( int i = 0; i < Width; i++ )
                    for ( int j = 0; j < Height; j++ )
                    {
                        short sumY = 0;
                        short sumX = 0;

                        for ( int m = 0; m < kernelSize; m++ )
                        {
                            // Vertikal 
                            int indexesV = j + doubleIndexes[ m ];
                            if ( indexesV <= 0 )
                                indexesV = Math.Abs( indexesV );

                            if ( indexesV >= Height )
                                indexesV = Height - 1;

                            short a = volume[ i, indexesV, k ];

                            short b = kernel1D[ m ];

                            sumY += (short) ( a * b );

                            // Horizontal
                            int indexesH = i + doubleIndexes[ m ];
                            if ( indexesH <= 0 )
                                indexesH = Math.Abs( indexesH );

                            if ( indexesH >= Width )
                                indexesH = Width - 1; //-( indexes - data.Width );

                            a = volume[ indexesH, j, k ];

                            //b = kernel1D[ m ];

                            sumX += (short) ( a * b );


                        }

                        Gy.volume[ i, j, k ] = (short) sumY;
                        Gx.volume[ i, j, k ] = (short) sumX;

                    }
        }


        public void Convolve1D_Horizontal( DataSet result )
        {
            int kernelSize = 5;
            //double[] kernel1D = { 0.0833333333333333, -0.666666666666667, 0, 0.666666666666667, -0.0833333333333333 };
            short[] kernel1D = { +1, -8, 0, 8, -1 };

            short[] doubleIndexes = { -2, -1, 0, +1, +2 };

            // check the odd size of kernel1D

            //for ( int i = kernelSize / 2 + 1; i < ( data.Width - ( kernelSize / 2 + 1 ) ) - 2; i++ )
            for ( int k = 0; k < Depth; k++ )
                for ( int i = 0; i < Width; i++ )
                    for ( int j = 0; j < Height; j++ )
                    {
                        short sum = 0;
                        for ( int m = 0; m < kernelSize; m++ )
                        {
                            // try
                            //  {
                            int indexes = i + doubleIndexes[ m ];
                            if ( indexes <= 0 )
                                indexes = Math.Abs( indexes );

                            if ( indexes >= Width )
                                indexes = Width - 1; //-( indexes - data.Width );

                            short a = volume[ indexes, j, k ];

                            short b = kernel1D[ m ];

                            sum += (short) ( a * b );

                        }
                        // testing direct access
                        result.volume[ i, j, k ] = (short) ( sum / 12 );
                    }
        }

        public void Convolve1D_Vertikal( DataSet result )
        {
            int kernelSize = 5;

            short[] kernel1D = { +1, -8, 0, 8, -1 };

            short[] doubleIndexes = { -2, -1, 0, +1, +2 };


            for ( int k = 0; k < Depth; k++ )
                for ( int i = 0; i < Width; i++ )
                    for ( int j = 0; j < Height; j++ )
                    {
                        short sum = 0;
                        for ( int m = 0; m < kernelSize; m++ )
                        {
                            // try
                            //  {
                            int indexes = j + doubleIndexes[ m ];
                            if ( indexes <= 0 )
                                indexes = Math.Abs( indexes );

                            if ( indexes >= Height )
                                indexes = Height - 1; //-( indexes - data.Width );

                            short a = volume[ i, indexes, k ];

                            short b = kernel1D[ m ];

                            sum += (short) ( a * b );

                        }
                        // testing direct access
                        result.volume[ i, j, k ] = (short) ( sum / 12 );
                    }
        }

        public void MultiplyBy( DataSet data )
        {
             for ( int k = 0; k < Depth; k++ )
                for ( int i = 0; i < Width; i++ )
                    for ( int j = 0; j < Height; j++ )
                    {
                        double temp =  this.volume[ i, j, k ];
                        double temp2 = data.volume[ i, j, k ];
                        temp = temp * temp2;
                                                                      
                        this.volume[ i, j, k ] = (short) ( temp);

                

                    }
        }


        public void MultiplyBy( double value )
        {
             for ( int k = 0; k < Depth; k++ )
                for ( int i = 0; i < Width; i++ )
                    for ( int j = 0; j < Height; j++ )
                    {
                        this.volume[ i, j, k ] = (short) ( ( (double) value * this.volume[ i, j, k ] / scale ) * scale );
                    }
        }

        public void Add( DataSet data )
        {
             for ( int k = 0; k < Depth; k++ )
                for ( int i = 0; i < Width; i++ )
                    for ( int j = 0; j < Height; j++ )
                    {
                        this.volume[ i, j, k ] += data.volume[ i, j, k ];
                    }
        }


        public void Diffusivity( DataSet Gx, DataSet Gy )
        {

            //   C = diffusivity(sqrt(Gx.^2 + Gy.^2));

            //   function R = diffusivity(value)
            //   K = 0.05;
            //%  R = 1 ./ (1+(abs(value)./K).^2);
            //   R = exp(-(abs(value)./K).^2 );
            //   end

            double K = 0.05;

           

            for ( int k = 0; k < Depth; k++ )
                for ( int i = 0; i < Width; i++ )
                    for ( int j = 0; j < Height; j++ )
                    {
                        double gx = (double) Gx.volume[ i, j, k ] / scale;
                        double gy = (double) Gy.volume[ i, j, k ] / scale;

                        double c = Math.Sqrt( gx * gx + gy * gy );
                        c = Math.Abs( c );

                        this.volume[ i, j, k ] = (short) ( scale * Math.Exp( -c * c / ( K * K ) ) );
                        //this.volume[ i, j, k ] = (short) ( c * scale );

                    }
        }

        public void SaveAsBitmap( string path )
        {
            this.Scale255();

            Bitmap outBitmap = new Bitmap( Width, Height );
            byte data = 0;

            for ( int i = 0; i < Width; i++ )
                for ( int j = 0; j < Height; j++ )
                {
                    data = (byte) this.volume[ i, j, 0 ];
                    outBitmap.SetPixel( i, j, Color.FromArgb( data, data, data ) );
                }

            outBitmap.Save( path, System.Drawing.Imaging.ImageFormat.Bmp );
        }
    }
}
