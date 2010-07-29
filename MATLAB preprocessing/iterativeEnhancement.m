% An Iterative Image Enhancement Algorithm and A
% New Evaluation Framework
%
% - note that image pixels must be in [0-1] range

function enhImg   = iterativeEnhancement(img,betaPar,T,delta)

I   = im2double(img);
Hx  = fspecial('sobel');
Hy  = Hx';

dIx = imfilter(I,Hx);
dIy = imfilter(I,Hy);
dI    = sqrt(dIx.^2+dIy.^2);

A = 2*I+dI;
B = 2*sqrt( betaPar.*( dIx.^2+dIy.^2+1));
 
r = A ./ B;

rt = r;

for t = 1 : T
    
    drx = imfilter(rt,Hx);
    dry = imfilter(rt,Hy);
    Gt    = sqrt(drx.^2+dry.^2);
    drt = Gt;

    a = Gt - min(min(Gt));
    b = max(max(Gt))-min(min(Gt));
    Gt2 = max(a./b,delta);
    
    A = 2.*rt + drt;
    B = 2.*sqrt(Gt2.*(drx.^2+dry.^2)+1);
    
    rt_1 = rt + A./B;
    rt = rt_1;
end

enhImg = rt;
