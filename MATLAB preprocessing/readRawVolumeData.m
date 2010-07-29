clear all;
fid = fopen('Neghip.raw','r','b');
% fid = fopen('skull.raw','r','b');

M = 64;
N = 64;
k = 64;

% fseek(fid,k*M*N,'cof');
a = fread(fid, M*N*k,'char');
a = reshape(a,[M N k]);
% a = uint8(a);
% image(a);
% surf(a);
% imshow(a,[])
% colormap(jet())

fclose(fid);