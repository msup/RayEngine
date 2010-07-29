
function [R] =  labeling(image)
% clc
% img = [ 0 0 0 0 0 0 0 0;  0 0 0 1 0 0 0 0; 0 0 1 1 0 1 1 1; 0 1 0 1 0 1 1 0; 0 1 1 1 0 1 1 0];
warning off
img =  double(image);
[M,N] = size(img);

L = zeros(M,N);
m = 5;

Threshold = 0;


for i =  2 : M - 1
for j =  2 : N - 1
       
        if (img(i,j) ~= 0 )
 
            a = img(i-1,j-1);
            b = img(i-1,j);
            c = img(i  ,j-1);
            d = img(i-1,j+1);
% 
%             imshow(img) ; colormap jet;
%             hold on
%             
%            
%             [a b c d]
%             plot(j-1, i-1,'r.');
%             plot(j  , i-1,'r.');
%             plot(j-1, i  ,'r.');
%             plot(j+1, i-1,'r.');
%             
%             plot(j,i,'go');
%             fprintf('[%d %d] Value %d \n', i, j, img(i,j) );
%             
            if (a == 0 && b == 0 && c == 0 && d == 0)
                img(i,j) = m;
                m = m+1;
            else
                img(i,j) = save_min(a,b,c,d, 0);
        end
        
          
%             pause
        end        
    end
end

for i = M -1 : -1 : 2
for j = N -1 : -1 : 2
         
        if (img(i,j) ~= 0 )
                   
%             fprintf('[%d %d] \n',i,j);
        
            a = img(i+1,j+1);
            b = img(i+1,j);
            c = img(i+1,j-1);
            d = img(i,j+1);
            
%            imshow(img) ; colormap jet;
%            hold on
% 
%             plot(j+1,i+1,'r.');
%             plot(j,i+1,'r.');
%             plot(j-1,i+1,'r.');
%             plot(j+1,i,'r.');
%             plot(j,i,'go');
%             fprintf('[%d %d] Value %d \n', i, j, img(i,j) );        
%             [img(i,j) a b c d]
%             pause

            if (a == 0 && b == 0 && c == 0 && d == 0)
                asdfasdffds = 0;
            else
                img(i,j) = save_min(a,b,c,d,0);        
            end
            
        end   
    end
end


 
R = img;
end

function RR = save_min(a,b,c,d, value)
    
    RR = value;
    Temp = sort([a b c d]);
    for k = 1 : length(Temp);
        if (Temp(k) ~= 0)
            RR = Temp(k);
        end
    end
    
end


% %%
% %function [R] =  labeling(image)
% % clc
% % img = [ 0 0 0 0 0 0 0 0;  0 0 0 1 0 0 0 0; 0 0 1 1 0 1 1 1; 0 1 0 1 0 1 1 0; 0 1 1 1 0 1 1 0];
% 
% img =  (image);
% [M,N] = size(img);
% 
% L = zeros(M,N);
% m = 5;
% 
% Threshold = 0;
% 
% 
% for j =  2 : N - 1
%     for i = 2 : M 
%       
%         if (img(i,j) ~= 0 )
%         
%             a = img(i-1,j-1);
%             b = img(i-1,j);
%             c = img(i-1,j+1);
%             d = img(i,j-1);
%             
%             if (a == 0 && b == 0 && c == 0 && d == 0)
%                 img(i,j) = m;
%                 m = m+1;
%             else
%                 img(i,j) = save_min(a,b,c,d, img(i,j));
%             end
%         
%         end        
%     end
% end
%  
% for j = N-1  : -1 : 2
% for     i = M -1 : -1 : 2
%          
%         if (img(i,j) ~= 0 )
%                    
% %             fprintf('[%d %d] \n',i,j);
%         
%             a = img(i+1,j+1);
%             b = img(i+1,j);
%             c = img(i+1,j-1);
%             d = img(i,j+1);
%             
%             
%             img(i,j) = save_min(a,b,c,d, img(i,j) );        
%         end   
%     end
% end
% 
% 
%  
% R = img;
% end
% 
% function RR = save_min(a,b,c,d, value)
%     
%     RR = value;
%     Temp = sort([a b c d]);
%     for k = 1 : length(Temp);
%         if (Temp(k) ~= 0)
%             RR = Temp(k);
%         end
%     end
%     
% end