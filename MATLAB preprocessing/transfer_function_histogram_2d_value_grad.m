 clear all;
close all;
clc;

% fid = fopen('Carp8bit.raw','r','b');
% 
% M = 256;
% N = 256;
% K = 154;

% data = fread(fid, M*N*K,'char');
% img = reshape(data,[M N K]);
% img = img(:,:,100);

% img = rgb2gray(imread('test_CC.bmp'));
img = (imread('foot_013.png'));

imgBW = im2bw(img,0.1);
% img = imnoise(img,'gaussian',0.05);
img = im2double(img);
img = img / max(img(:)) * 1;


img = imgpolarcoord(img);
% http://www.holoborodko.com/pavel/?page_id=1660
% robust gradient field computation & normalization
Hx = 1/32*[-1 -2 0 2 1; -2 -4 0 4 2; -1 -2 0 2 1];
Hy = Hx';

Hx = 1/512*[-1 -4 -5 0 5 4 1; -4 -16 -20 0 20 16 4;...
    -6 -24 -30 0 30 24 6; -4 -16 -20 0 20 16 4; -1 -4 -5 0 5 4 1];
Hy = Hx';

fx = imfilter(img,Hx, 'replicate' );
fx2 = imfilter(fx,Hx, 'replicate' );

fy = imfilter(img,Hy, 'replicate' );
fy2 = imfilter(fy,Hy, 'replicate' );


F1 = sqrt(fx.^2+fy.^2);
F2 = sqrt(fx2.^2+fy2.^2);

M1 = min(F1(:));
M2 = max(F1(:));
F1 = (F1-M1)/(M2-M1);

M1 = min(F2(:));
M2 = max(F2(:));
F2 = (F2-M1)/(M2-M1);

% imshow(F);

histogram = zeros(256,256);
histogram2 = zeros(256,256);
StatHistogram = zeros(256,256);
MeanDistanceHistogram = zeros(256,256);

[M,N]=size(img);
w = 4;
varAnalyze = [];
priemer = [];
C2 = [];

for i = 1 + w : M -w 
    for j = 1 +w  : N -w
     
      V  = ceil(( img(i,j))*255 ) + 1 ;
     G1  = ceil( F1(i,j)*255  ) + 1 ;
     G2  = ceil( F2(i,j)*255  ) + 1 ;
     
    temp = img( i-w:i+w,j-w:j+w);
    
  
    
    aver = mean(temp(:)) * 255;
    
%     vari = sqrt( var(temp(:)) ) /0.2379*100 ;
%     M1 = min(varAnalyze(:));
%     M2 = max(varAnalyze(:));
  
    vari =   moment(temp(:),2) * 255;
    
    varAnalyze = [varAnalyze; vari];
    priemer = [priemer;aver];
    
    fprintf('Mean   %3f Var: %3f \n',mean(temp(:)), var(temp(:)) );
    fprintf('Moment %3f \n',vari);

    
    StatHistogram( round(vari) + 1, round(aver) + 1) = StatHistogram(round(vari) +1,round(aver) +1 ) + 1;
    
    c1 = 1 / sqrt(M.^2+N.^2) * 255;
%     fi = M/2-i;
%     fj = N/2-j;
    c2 = c1 * sqrt(0*i*i+j*j) ;

    MeanDistanceHistogram( round(aver)+1, round(c2)+1) = MeanDistanceHistogram( round(aver)+1, round(c2)+1) + 1;
%     plot(aver,c2,'.'); hold on; 
    C2 = [C2,c2];

%      fprintf('Value %f Grad: %f \n',V,G);
     histogram(V,G1)  = histogram(V,G1) + 1;
     histogram2(V,G2) = histogram(V,G2) + 1;
    
    end
end

figure;
subplot(511);
imshow(img);
subplot(512);

H = 1/9*ones(3,3);
%%
Hf = imfilter(fliplr(histogram2),H)';
image(Hf*255);


subplot(513);
imshow(F2);



% imshow(histogram*255)

H = 1/9*ones(3,3);
%%
Hf = imfilter(fliplr(histogram),H)';
image(Hf*255);

subplot(514);
image(StatHistogram*255);
subplot(515);
imhist(img);

 whitebg([0 .0 .0]);
 