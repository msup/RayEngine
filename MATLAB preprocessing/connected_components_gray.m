%  Parallel Algorithm for Gray-scale Image Segmentation
%  Harvey A. Cohen

function F = connected_components_gray(img)
clc;
% img = rgb2gray(imread('test_CC.bmp'));
% img =  im2bw(img,0.1);
[M,N] = size(img);

E   = img;
F   = 0*ones(M,N);     % label matrix
LUT = [];
BKG = 0;              % background pixel
next_available = 10;

for i = 2 : M
    for j = 2 : N
        
        if (E(i,j) ~= BKG )

            test = 0;

            if (E(i,j-1) == E(i,j))
                test = max ( test, F(i,j-1) );
            end

            if (E(i-1,j) == E(i,j))
                test = max ( test, F(i-1,j) );
            end

          
%             LUT(test+1) = min_valid(E(i,j-1),E(i-1,j) );
            
            if (test == BKG)
                test = next_available;
                next_available = next_available + 1;
%                 LUT(test+1) = test;
             
%                 LUT(test+1) = min_valid(E(i,j-1),E(i-1,j) );
            end
            
            F(i,j) = test;
          

        end
    end
end

% image(img);
% image(LUT)

end


function R = min_valid(a,b)
   Temp    = sort([a,b]);
   [R ind] = min (Temp);
   
   if (R == 0) % FIXME 0 == BKG
     R = Temp(2);
   end
end
