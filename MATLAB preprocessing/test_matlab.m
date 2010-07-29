tic
img = imread('test.bmp');
fprintf('Read file %f \n',toc);

img = im2double(img);

Hx = 1/12*[+1 -8 0 8 -1];

tic
Gx = imfilter(img,Hx);
fprintf('Convolve %f \n',toc);
