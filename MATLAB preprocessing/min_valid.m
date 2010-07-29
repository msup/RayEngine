function R = min_valid(a,b)
   Temp    = sort([a,b]);
   [R ind] = min (Temp);
   
   if (R == 0) % FIXME 0 == BKG
     R = Temp(2);
   end
end