NodeProgram Part10;
NodeVar
   NodeNumber     : INTEGER;
   a, b, c, x : INTEGER;
   y          : REAL;

BEGIN {Part10}
   BEGIN
      NodeNumber := 2;
      a := NodeNumber;
      b := 10 * a + 10 * NodeNumber DIV 4;
      c := a - - b
   END;
   x := 11;
   y := 20 / 7 + 3.14;
   { writeln('a = ', a); }
   { writeln('b = ', b); }
   { writeln('c = ', c); }
   { writeln('NodeNumber = ', NodeNumber); }
   { writeln('x = ', x); }
   { writeln('y = ', y); }
END.  {Part10}

