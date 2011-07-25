using System.Drawing;
namespace Data
{
    public class LookupTable
    {
        #region private fields

        uint[] LUTdata1D = null;
        uint[,] LUTdata2D = null;

        #endregion private fields

        public uint[] LUTData1D
        {
            get
            {
                return LUTdata1D;
            }
        }

        public bool Load( string fileName )
        {
            bool retValue = false;
            Bitmap image = new Bitmap( fileName );

            LUTdata1D = new uint[256];
            for(int x = 0; x < image.Width; x++) {
                Color pixelColor = image.GetPixel( x, 0 );
                LUTdata1D[x] = (uint) ((pixelColor.R << 24) |
                                        (pixelColor.G << 16) | 
                                        (pixelColor.B << 8)  |
                                        (pixelColor.A << 0));
            //    LUTdata1D[x] = (uint) (pixelColor.A << 0);
            
            }

            return retValue;
        }

        public bool Load()
        {
            int width = 256;

            LUTdata1D = new uint[256];

            //for ( int i = 150; i < 160; ++i )
            //    LUTdata1D[i] = (uint) ( ( ( i-50 ) << 24 ) | ( ( i-0 ) << 16 ) );

            /*
                                Vh = k*H+q
                                Vl = k*L+q
                                -----------------
                                Vh-k*H = q
                                Vl = k*L+Vh-kH    =>     (Vl-Vh)=k*(L-H)   ==>   (Vl-Vh)/(L-H) = k
                                Vh-(Vl-Vh)/(L-H)*H = q

                    */

            for(int i = 188; i < width - 10; ++i)
                LUTdata1D[i] = (uint) (((i - 150) << 24) | ((i - 50) << 08) | ((i - 50) << 16) | ((i - 60) << 0));

            int L = 0;
            int H = 255;
            float Vl = 1;
            float Vh = 200;
            float k = (Vl - Vh) / (L - H);
            float q = Vh - (Vl - Vh) / (L - H) * H;

            for(int i = L; i < H; ++i)
                LUTdata1D[i] = (uint) ((((uint) (System.Math.Round( i * k + q ))) << 24) | ((uint) (System.Math.Round( i + 70f )) << 0));

            L = 5;
            H = 255;
            Vl = 5;
            Vh = 100;
            k = (Vl - Vh) / (L - H);
            q = Vh - (Vl - Vh) / (L - H) * H;

            //for ( int i = L; i < H; ++i )
            //    LUTdata1D[i] = (uint) ( ( ( (uint) ( System.Math.Round( i*k+q ) ) ) << 24 ) | ( (uint) ( System.Math.Round( i+70f ) ) << 8 ) );

            return true;
        }
    }
}