warning off
clc

%
display = false;

% gui specific
h = waitbar(0,'Calculation in progress');

% raw data description
M = 256;
N = 256;
K = 256;

delta_t = 0.05; 
iterMax = 5;


% init
fid = fopen('skull.raw','r','b');
% fed = fopen('nonlinearDiffused_output_denoised_30_30iter.raw','w','b');
outputPath = sprintf('nonlinDiff.skull.deltaT %.2f.iter %d.raw',delta_t,iterMax);
fed = fopen(outputPath,'w','b');


enhAF = zeros(M,N,K);

for k = 1 : K-1
    
% read data
fseek(fid,k*M*N,'bof');
img = fread(fid, M*N,'char');
img = reshape(img,[M N]);
img = (img)/255;


% preprocess
a = double(img);
I = a;
V = a;
  
g = ac_gradient_map(I,2);

dim = 1;

for i = 1:iterMax
    I = ac_div_AOS(I, g, delta_t);
    slice = I(:,:); 
    
    Minimum = min(min(slice));
    Maximum = max(max(slice));
    result = (slice-Minimum)/(Maximum-Minimum)*255;
  
end
      
% subplot(121);
% imshow(a);
% subplot(122);
% imshow(slice);
% colormap(gray);
% pause;

enhAF(:,:,k+1) = im2uint8(slice) ;

if display == true
    subplot(121);
    image(im2uint8(a));
    subplot(122);
    image(enhAF(:,:,k+1));
end

waitbar(k/K,h);
end

% save
fwrite(fed,enhAF(:),'uint8');

 
fclose(fid);
fclose(fed);

