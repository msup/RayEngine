
function LH_histogram_2Dslices

display = true;
clc

LHhisto = zeros(256,256);

img = rgb2gray(imread('gradientImageTest.bmp'));
% img = imread('foot_013.png');
img = im2double(img);
% img = imnoise(img,  'gaussian' ,0.02);
% img = img + 0.1*rand(size(img));
% img = im2double(img);
img = img / max(img(:)) * 1;


% http://www.holoborodko.com/pavel/?page_id=1660
% robust gradient field computation & normalization
Hx = 1/32*[-1 -2 0 2 1; -2 -4 0 4 2; -1 -2 0 2 1];
Hy = Hx';

Hx = 1/512*[-1 -4 -5 0 5 4 1; -4 -16 -20 0 20 16 4;...
    -6 -24 -30 0 30 24 6; -4 -16 -20 0 20 16 4; -1 -4 -5 0 5 4 1];
Hy = Hx';

fx = imfilter(img,Hx, 'replicate' );
fy = imfilter(img,Hy, 'replicate' );

F = sqrt(fx.^2+fy.^2);
fx = fx./(F+0.1);
fy = fy./(F+0.1);


if (display)
subplot(211);
    
imshow(im2uint8(img))
hold on;

% imshow(abs(fx));
% hold on;
% imshow(abs(fy));
% hold on;
end

E = edge(img,'canny',0.10);
[ex,ey]=find(E==1);
hold on;

if (display)

plot(ey,ex,'y.','LineWidth',3)
hold on; quiver(fx,fy,0)
end


[M,N]=size(img);
for i = 1 : 1 : length(ex)
% for k = 20 : M - 20
%     for l = 20 : N - 20

fprintf('Iteration : %d / %d \n', i, length(ex));
x0 = ey(i);
y0 = ex(i);
% x0 = k;
% y0 = l;

if (display)
hold on; 
plot(x0,y0,'mx','LineWidth',10);
end

h = 1.00;
% d = -1;



   x0v = x0; % vektor rieseni
   y0v = y0; % vektor rieseni

    
   % backward integrate
   [x0v,y0v] = integrate(x0v,y0v,-1, fx, fy,h, M, N);
   Low = img( round(y0v), round(x0v)  );
   hold on;
   
   if (display)
    plot(x0v,y0v,'r+','LineWidth',5);    
   end
   
   x0v = x0; % vektor rieseni
   y0v = y0; % vektor rieseni
   % forward integrate
   [x0v,y0v] = integrate(x0v,y0v,+1, fx, fy,h, M, N);
   High = img( round(y0v), round(x0v)  );
   hold on;
   
   if (display)
   plot(x0v,y0v,'g+','LineWidth',5);    
   end
%    pause;
   [High Low];
    
   Low =  round(255*Low)+1;
   High = round(255*High)+1;
   
   increment = img(y0,x0);
   
   LHhisto(High,Low) = LHhisto(High,Low) + increment;
   LHhisto(High,High) = LHhisto(High,High) + increment;
   LHhisto(Low,Low) = LHhisto(Low,Low) + increment;
   
  
   [x0v; y0v]';

end
% end
%     pause;
%     imshow(im2uint8(LHhisto))
%     
    subplot(212);
%    LHhisto = LHhisto / max(LHhisto(:)) ;
   
   imshow(LHhisto)
    
% figure;
%     surf(LHhisto);
end

function [x,y] = integrate(x0v,y0v,d, fx, fy,h, M, N)

for i = 2 : 10
   % x zlozka
   ax0 = floor( x0v(i-1)  );
   ax1 = ax0  + 1 ;
   
   ay0 = fx( floor(y0v(i-1)-0), ax0 );
   ay1 = fx( floor(y0v(i-1)+0), ax1  );
   X = x0v( i-1 );
   Rx = lerp( X ,ax0,ax1,ay0,ay1 ); 
   
   x0v( i ) = x0v( i-1 ) + d* h * Rx;
    
   % --- boundary check ----
   if (x0v(i) <= 1)
       x0v(i) = 1;
   else if (x0v(i) >= N)
      x0v(i) = N-10;
       end
   end
         
   % -----------------------
   
   %    x = x0v( i-1 ) + d* h * Rx;
   
   
   % y zlozka
   bx0 = floor( y0v(i-1)  );
   bx1 = bx0  + 1 ;
   by0 = fy(bx0, floor(x0v(i-1)-0)  );
   by1 = fy(bx1, floor(x0v(i-1)+0));
   Y = y0v( i-1 );
   Ry = lerp( Y ,bx0,bx1,by0,by1 ); 
   y0v( i ) = y0v( i-1 ) + d * h * Ry;
  
       % --- boundary check ----
   if (y0v(i) < 1)
       y0v(i) = 1;
   else if (y0v(i) > M)
      y0v(i) = M-2;
       end
   end
         
   % -----------------------
   
%    y = y0v( i-1 ) + d * h * Ry;

%     if (i >= 2)
%        if ( (abs(x0v(i)-x0v(i-1)) < 0.01) || (abs(y0v(i)-y0v(i-1)) < 0.01) )
%            break
%        end
%     end  
%     hold on; 
x = x0v(i);
    y = y0v(i);
    
%     if d==1
%         plot(x,y,'g.')
%     else
%         plot(x,y,'r.')
%       end
%     
end
 

%     if d==1
%         plot(x,y,'g.')
%     else
%         plot(x,y,'r.')
%     end

    
end 

