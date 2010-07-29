
function [R,Equivalences] =  labeling2D(inImage)

warning off
img =  double(inImage);
[M,N] = size(img);

L = zeros(M,N);
m = 5;

Threshold = 0;

E = im2double(edge(img,'canny'));
R = zeros(M,N);

subplot(211);
imshow(E);
% subplot(312);
% imshow(R);
subplot(212);

Equivalences = [];


for i =  2 : M - 1
for j =  2 : N - 1
       
        if (img(i,j) ~= 0 && E(i,j) ~= 1)
 
            
%           image(im2uint8(img/255 + E ) ); colormap gray
%           hold on;
%           plot(j,i,'o');
%           plot(j-1,i-1,'r.');
%           plot(j,i-1,'r.');
%           plot(j+1,i-1,'r.');
%           plot(j-1,i,'r.');
%           
            
          a = img(i-1,j-1);
          b = img(i-1,j);
          c = img(i-1,j+1);
          d = img(i,j-1);
          
          Ea = E(i-1,j-1);
          Eb = E(i-1,j);
          Ec = E(i-1,j+1);
          Ed = E(i,j-1);
          
          Ra = R(i-1,j-1);
          Rb = R(i-1,j);
          Rc = R(i-1,j+1);
          Rd = R(i,j-1);
          
%           fprintf('Ea - [ %d %d %d; %d ]\n',Ea,Eb,Ec,Ed);
%           fprintf('Ra - [ %d %d %d; %d ]\n',Ra,Rb,Rc,Rd);
%           pause;
          
         

          if ( Eb==1 && Ec ==1 && Ed == 1)
             R(i,j) = m;
             m = m + 1;
    
                   
          elseif (Ea == 0 && Eb == 0 && Ec == 0 && Ed == 0 && Ra == Rd && Rb == Rc)
             R(i,j) = Rc;
             s = struct('int',[Rc Rd]);
             Equivalences = [Equivalences,s];
             
%           LxHLy;Lx
          elseif ( Eb ==1 && Ra == Rd && Rc ~=0 && Rc ~= Ra)
              R(i,j) = Rd;            
 


          % HHLL
          elseif ( Ea == 1 && Ed == 1 && Rb == Rc)
            R(i,j) = Rb ;
              
          % 2
          elseif ( Ea == 1 && b == c && d ~= b)
              R(i,j) = R(i,j-1);

          % 3
          elseif ( Ra == Rb && Rb == Rc && Ed == 1 && Ed == 1)
              R(i,j) = Ra;
          
          % 4
          elseif ( Ra == Rb && Rb == Rc && Rd == Rc )
              R(i,j) = R(i-1,j-1);
          
   
          % 5
          elseif ( Ra == Rb && Rb == Rd)
              R(i,j) = R(i-1,j-1);
          
          % 6
          elseif ( Ea == 1 && Eb == 1 && Rc ~= Rd)
              R(i,j) = Rd;
          
          % 7
          elseif ( Ra ~= Rc && Eb == 1 && Ed == 1)
              R(i,j) = R(i-1,j+1);
          
          % 8
          elseif ( Ea == 1 && Eb == 1 && Ec == 1 && d ~= 0)
              R(i,j) = R(i,j-1);
          
          % 9 HLL,H
          elseif ( Ea == 1 && Rb == Rc && Ed == 1 && Rb ~= 0)
              R(i,j) = R(i-1,j);
              
          % 11
          elseif ( Ea == 1 && Rb == Rc && Rc == Rd)
              R(i,j) = Rb;
          
          % LHH,Lo 
          elseif ( Ra == Rd && Eb == 1 && Ec == 1)
            R(i,j) = Rd;
                
          % HLL,Ly
          elseif (Ea ==1 && Rb == Rc && Rd ~= 0 && Rb ~= 0 && Rd ~= Rc)
              R(i,j) = Rc;
            
      
            
          end
          
%           R
          
        end 
        
    end
end

for i = M -1 : -1 : 2
for j = N -1 : -1 : 2
         
 

         
    end
end


 

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
