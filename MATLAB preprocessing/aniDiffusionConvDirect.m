function I_kp1 = aniDiffusionConvDirect(img,iterMax)

[M,N]=size(img);
I_kp1 = zeros(M,N);
I_kp  = img;
lambda = 1;

%I_kp = imfilter(I_kp,fspecial('gaussian'));
  
Hx = fspecial('sobel');
Hy = Hx';

for k = 1 : iterMax

    fprintf('.');            
    Dx = imfilter(I_kp,Hx);
    Dy = imfilter(I_kp,Hy);
    C  = diffusivity(sqrt(Dx.^2+Dy.^2));
    D2x = imfilter(C.*Dx,Hx);
    D2y = imfilter(C.*Dy,Hy);
            
    I_kp1 = I_kp + 0.44520 .* ( (D2x+D2y) );
    I_kp = I_kp1;
end
    fprintf('\n');
end

function R = diffusivity(value)
    K = 0.025;
    R = 1 ./ (1+(abs(value)./K).^2);
%     R = exp(-(abs(value)./K).^2 );
end