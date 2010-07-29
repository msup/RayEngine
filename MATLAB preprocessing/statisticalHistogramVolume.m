clear all;
clc;
fid = fopen('Carp8bit.raw','r','b');

M = 256;
N = 26;
K = 154;

data = fread(fid, M*N*K,'char');
data = reshape(data,[M N K]);
% data(data > 255) = 255;

StatHistogram = zeros(256,200);

w = 10;

dimension = (2*w+1)^3;

for i = w + 1 : M - w
for j = w + 1 : N - w
for k = w + 1 : K - w

    temp = data( i-w:i+w,j-w:j+w,k-w:k+w );
    
    fprintf('Mean %3f Var: %3f \n',mean(temp(:)), var(temp(:)) );
    
    aver = mean(temp(:));
    
    vari = sqrt( var(temp(:)) ) ;
    
    StatHistogram( round(vari) + 1, round(aver) + 1) = StatHistogram(round(vari) +1,round(aver) +1 ) + 1;
    
end
end
end

image(StatHistogram(:,1:140))
    