clear all;
% fid = fopen('nonlinearDiffused_output.raw','r','b');
fid = fopen('skull.raw','r','b');
fed = fopen('skull_noisy_30.raw','w','b');

M = 256;
N = 256;
k = 256;

% fseek(fid,k*M*N,'cof');
img = fread(fid, k*M*N,'char');

R = rand(256*256*256,1);
R = 2*(R-0.5);
R = 30*R;
noisy = img + R;

% a = reshape(a,[M N]);
% a = uint8(a);
% image(a);
% surf(a);
% imshow(a,[])
% colormap(jet())

img = reshape(img,[256 256 256]);
noisy = reshape(noisy,[256 256 256]);

subplot(122);
imshow(uint8(noisy(:,:,60)))
subplot(121);
imshow(uint8(img(:,:,60)))

fwrite(fed,noisy(:),'uint8');

fclose(fid);
fclose(fed);

% subplot(121); imshow(img); subplot(122); imshow(slice)