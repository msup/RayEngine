using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace WpfOpenTK
{
    public class VolumetricData
    {
        byte[, ,] volumeData = null;
        byte[] wholeVolumeData = null;

        #region properties
        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public int Depth
        {
            get;
            set;
        }

        public byte[, ,] RawVolumeData
        {
            get
            {
                return volumeData;
            }
        }

        public byte[] VolumeDataArray
        {
            get
            {
                return wholeVolumeData;
            }
        }
        #endregion

        public VolumetricData( int width, int height, int depth )
        {
            Width = width;
            Height = height;
            Depth = depth;

            volumeData = new byte[Width, Height, Depth];
        }

        /// <summary>
        /// deprecated
        /// </summary>
        /// <param name="fileName"></param>
        public VolumetricData( string fileName )
        {
            #region Get the volume data file path

            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string volumeDataPath = (string) configurationAppSettings.GetValue( "VolumeDataPath", typeof( string ) );

            #endregion Get the volume data file path

            #region Read volume dimensions from accompanying xml volume data

            var volumeDataDescriptionFile = XDocument.Load( volumeDataPath + ".xml" );

            Width = Int32.Parse(
                     (
                     from c in volumeDataDescriptionFile.Descendants( "Dimensions" )
                     select c.Element( "Width" ).Value
                     ).Single()
            );

            Height = Int32.Parse(
                     (
                     from c in volumeDataDescriptionFile.Descendants( "Dimensions" )
                     select c.Element( "Height" ).Value
                     ).Single()
            );

            Depth = Int32.Parse(
                     (
                     from c in volumeDataDescriptionFile.Descendants( "Dimensions" )
                     select c.Element( "Depth" ).Value
                     ).Single()
            );

            #endregion Read volume dimensions from accompanying xml volume data

            #region Volume memory allocation

            volumeData = new byte[Width, Height, Depth];
            if(volumeData == null)
                throw new Exception( "Memory allocation for volume data failed." );

            #endregion Volume memory allocation

            // FIXME: dimension check
            // TODO: check the format - raw

            FileStream inStream = new FileStream( volumeDataPath, FileMode.Open );

            BinaryReader binReader = new BinaryReader( inStream );

            byte[] rawData = new byte[Width * Height];

            uint index = 0;

            while((binReader.Read( rawData, 0, Width * Height ) != 0) && index < Depth) {
                CopyToArray( rawData, ref volumeData, Width, Height, index );

                FileStream fstream = new FileStream( "file" + index.ToString(), FileMode.Create );
                fstream.Write( rawData, 0, rawData.Length );
                fstream.Close();

                index++;
            }
        }

        public VolumetricData()
        {
            #region Get the volume data file path

            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string volumeDataPath = (string) configurationAppSettings.GetValue( "VolumeDataPath", typeof( string ) );

            #endregion Get the volume data file path

            #region Read volume dimensions from accompanying xml volume data

            var volumeDataDescriptionFile = XDocument.Load( volumeDataPath + ".xml" );

            Width = Int32.Parse(
                     (
                     from c in volumeDataDescriptionFile.Descendants( "Dimensions" )
                     select c.Element( "Width" ).Value
                     ).Single()
            );

            Height = Int32.Parse(
                     (
                     from c in volumeDataDescriptionFile.Descendants( "Dimensions" )
                     select c.Element( "Height" ).Value
                     ).Single()
            );

            Depth = Int32.Parse(
                     (
                     from c in volumeDataDescriptionFile.Descendants( "Dimensions" )
                     select c.Element( "Depth" ).Value
                     ).Single()
            );

            #endregion Read volume dimensions from accompanying xml volume data

            #region Volume memory allocation

            wholeVolumeData = new byte[Width * Height * Depth];
            if(wholeVolumeData == null)
                throw new Exception( "Memory allocation for volume data failed." );

            #endregion Volume memory allocation

            #region Read Volume data

            FileStream inStream = new FileStream( volumeDataPath, FileMode.Open );

            BinaryReader binReader = new BinaryReader( inStream );

            binReader.Read( wholeVolumeData, 0, Width * Height * Depth );
           
            inStream.Close();
            binReader.Close();

            #endregion Read Volume data
        }

        void CopyToArray( byte[] source, ref byte[, ,] destination, int width, int height, uint depthIndex )
        {
            // changed for, to height, width reversed
            // changed destination j, i
            // changed source height
            /*for ( int i = 0; i < width; i++ )
                for ( int j = 0; j < height; j++ )
                {
                    // TODO: fixme 256*256
                    destination[i, j, depthIndex] = (uint)
                        ( ( source[i*height + j] << 16 )
                        //|
                        //  ( source[i*height + j] << 16 ) |
                        //  ( source[i*height + j] << 08 ) |
                        //  ( source[i*height + j] << 00 )
                        );
                }

            */

            for(int i = 0; i < width; i++)
                for(int j = 0; j < height; j++) {
                    // TODO: fixme 256*256
                    destination[i, j, depthIndex] = source[i + j * height];
                }
        }

        public void GenerateRandom()
        {
            Random rand = new Random();

            for(int i = 0; i < Width; i++)
                for(int j = 0; j < Height; j++)
                    for(int k = 0; k < Depth; k++)
                        volumeData[i, j, k] += (byte) (rand.Next());
        }
    }
}