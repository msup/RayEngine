% clc

img = rgb2gray(imread('gradientImageTest.bmp'));
% img = imread('foot_013.png');
img = double(img);

% Hx = fspecial( 'sobel' );
% Hy = Hx';

% http://www.holoborodko.com/pavel/?page_id=1660
Hx = 1/32*[-1 -2 0 2 1; -2 -4 0 4 2; -1 -2 0 2 1];
Hy = Hx';

Hx = 1/512*[-1 -4 -5 0 5 4 1; -4 -16 -20 0 20 16 4;...
    -6 -24 -30 0 30 24 6; -4 -16 -20 0 20 16 4; -1 -4 -5 0 5 4 1];
Hy = Hx';

fx = imfilter(img,Hx);
fy = imfilter(img,Hy);

F = sqrt(fx.^2+fy.^2);
fx = fx./(F+0.1);
fy = fy./(F+0.1);

imshow(abs(fx));
hold on;
imshow(abs(fy));
hold on; quiver(fx,fy)


x0 = 76;
y0 = 56;
h = 1.0;
d = +1;
x0v = [x0]; % vektor rieseni
y0v = [y0]; % vektor rieseni

hold on; plot(x0,y0,'ro');

for i = 2 : 250
    
   ax0 = floor( x0v(i-1)  );
   ax1 = ax0  + 1 ;
   ay0 = fx( floor(y0v(i-1)-0), ax0 );
   ay1 = fx( floor(y0v(i-1)+0),ax1  );
   X = x0v( i-1 );
   Rx = lerp( X ,ax0,ax1,ay0,ay1 ); 
   x0v( i ) = x0v( i-1 ) + d* h * Rx;
   
   % y zlozka
   bx0 = floor( y0v(i-1)  );
   bx1 = bx0  + 1 ;
   by0 = fy(bx0, floor(x0v(i-1)-0)  );
   by1 = fy(bx1, floor(x0v(i-1)+0));
   Y = y0v( i-1 );
   Ry = lerp( Y ,bx0,bx1,by0,by1 ); 
   y0v( i ) = y0v( i-1 ) + d * h * Ry;
   
end

[x0v; y0v]'
hold on; 
plot(x0v,y0v,'r.')






% img = rgb2gray(imread('gradientImageTest.bmp'));
% img = double(img);
% 
% % Hx = fspecial( 'sobel' );
% % Hy = Hx';
% 
% % http://www.holoborodko.com/pavel/?page_id=1660
% Hx = 1/32*[-1 -2 0 2 1; -2 -4 0 4 2; -1 -2 0 2 1];
% Hy = Hx';
% 
% Hx = 1/512*[-1 -4 -5 0 5 4 1; -4 -16 -20 0 20 16 4;...
%     -6 -24 -30 0 30 24 6; -4 -16 -20 0 20 16 4; -1 -4 -5 0 5 4 1];
% Hy = Hx';
% 
% fx = imfilter(img,Hx);
% fy = imfilter(img,Hy);
% 
% F = sqrt(fx.^2+fy.^2);
% fx = fx./(F+0.1);
% fy = fy./(F+0.1);
% 
% imshow(abs(fx));
% hold on;
% imshow(abs(fy));
% hold on; quiver(fx,fy)
% 
% 
% x0 = 60.0;
% y0 = 25.0;
% h = 0.25;
% x0v = [x0]; % vektor rieseni
% y0v = [y0]; % vektor rieseni
% 
% hold on; plot(x0,y0,'ro');
% 
% for i = 2 : 50
%     
%    ax0 = floor( x0v(i-1) );
%    ax1 = ax0  + 1 ;
%    ay0 = fx(ax0,floor(y0v(i-1))  );
%    ay1 = fx(ax1,floor(y0v(i-1)) );
%    X = x0v( i-1 );
%    Rx = lerp( X ,ax0,ax1,ay0,ay1 ); 
%    x0v( i ) = x0v( i-1 ) - h * Rx;
%    
%    % y zlozka
%    bx0 = floor( y0v(i-1) );
%    bx1 = bx0  + 1 ;
%    by0 = fy(floor(x0v(i-1)),bx0);
%    by1 = fy(floor(x0v(i-1)),bx1);
%    Y = y0v( i-1 );
%    Ry = lerp( Y ,bx0,bx1,by0,by1 ); 
%    y0v( i ) = y0v( i-1 ) - h * Ry;
%    
% end
% 
% hold on; 
% plot(x0v,y0v,'r.')
