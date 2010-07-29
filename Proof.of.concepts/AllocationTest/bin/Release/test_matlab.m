clear all, clc;

iptsetpref('UseIPPL', false)
  
tic
img = imread('test.bmp');
fprintf('Read file %f \n',toc);

img = im2int16(img);

Hx = 1/12*[+1 -8 0 8 -1];
Hx = Hx';

tic
for i = 1 : 1
    fprintf('%d  ',i);
    Gx = imfilter(img,Hx);
end
fprintf('\n');
fprintf('Convolve %f \n',toc);

Gx = double( Gx );
m1 = double( min(Gx(:)) );
m2 = double( max(Gx(:)) );
a =  uint8( 255 *( ( Gx - m1 ) ) / ( m2 - m1 ) );
% b = 32768*a;

imshow(uint8(a))
