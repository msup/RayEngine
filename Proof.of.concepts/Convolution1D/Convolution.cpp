#include "Convolution.h"

Convolution::Convolution(void)
{
}

Convolution::~Convolution(void)
{
}

void Convolution::run()
{

  clock_t clock1,clock2;

  clock1 = clock(); 

    const short Depth = 1, Width = 2000, Height = 1000;

  // http://www.cplusplus.com/forum/articles/7459/

 double ***data;

  // Allocate memory
  data = new double**[Width];
  for (int i = 0; i < Width; ++i) {
    data[i] = new double*[Height];

    for (int j = 0; j < Height; ++j)
      data[i][j] = new double[Depth];
  }

double ***temp;

  // Allocate memory
  temp = new double**[Width];
  for (int i = 0; i < Width; ++i) {
    temp[i] = new double*[Height];

    for (int j = 0; j < Height; ++j)
      temp[i][j] = new double[Depth];
  }


clock2 = clock();
	
   double t=clock2-clock1;
   double sec=(t)/CLOCKS_PER_SEC;

   cout << "Allocation .. 1: " << sec << endl;
   


  /*double ***data = new double**[Width];
  
  for (int i=0; i<Depth; ++i) 
  {
    data[i]=new double* [Height];
  
    for (int j=0; j<Width; ++j) 
    {
      data[i][j]=new double [Depth];
    
      for (int k=0; k<Height; ++k)
        data[i][j][k]=0;
    
    }
  }

  double ***temp = new double**[Width];
  
  for (int i=0; i<Depth; ++i) 
  {
    temp[i]=new double* [Height];
  
    for (int j=0; j<Width; ++j) 
    {
      temp[i][j]=new double [Depth];
    
      for (int k=0; k<Height; ++k)
        temp[i][j][k]=0;
    }
  }*/
clock1 = clock();
      int kernelSize = 5;
      double kernel1D[] = { 0.0833333333333333, -0.666666666666667, 0, 0.666666666666667, -0.0833333333333333 };
      short doubleIndexes[] = { -2, -1, 0, +1, +2 };



      for ( int k = 0; k < Depth; k++ )
        for ( int i = 0; i < Width; i++ )
          for ( int j = 0; j < Height; j++ )
          {
            double sum = 0.0;
            for ( int m = 0; m < kernelSize; m++ )
            {

              int indexes = i + doubleIndexes[ m ];
              if ( indexes <= 0 )
                indexes = 0;

              if ( indexes >= Width )
                indexes = Width - 1; //-( indexes - data.Width );


              double a = data[ indexes][j][k ];

              double b = kernel1D[ m ];

              sum += a * b;


            }
            temp[ i][j][k] = sum;
          }

   clock2 = clock();
   t=clock2-clock1;
   sec=(t)/CLOCKS_PER_SEC;

   cout << "1D Convolution .. 2: " << sec << endl;
   
    }

