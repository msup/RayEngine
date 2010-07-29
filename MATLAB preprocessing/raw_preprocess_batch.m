warning off
clc

% raw data description
M = 256;
N = 256;
K = 256;

% init
fid = fopen('skull.raw','r','b');
fed = fopen('enhan3ced.raw','w','b');

enhAF = zeros(M,N,K);

for k = 1 : K-1
    
% read data
fseek(fid,k*M*N,'bof');
img = fread(fid, M*N,'char');
img = reshape(img,[M N]);
img = (img)/255;

% preprocess
betaPar = 10.2;
T = 0;
delta = 8*10^8;
enh = iterativeEnhancement(img,betaPar,T,delta);
    fprintf('%d - ',k');
enhAF(:,:,k) = im2uint8( aniDiffusionConvDirect(enh,5)) ;

% subplot(131);
% imshow(img);
% subplot(132);
% imshow(enh);
% subplot(133);
% imshow(enhAF);
% colormap(jet());
% pause;

end

% save

fwrite(fed,enhAF(:),'bit8');

fclose(fid);
fclose(fed);

