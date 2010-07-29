function y = lerp(x,x0,x1,y0,y1)

y = y0+(x-x0)*(y1-y0)/(x1-x0);