U S E   [ A I M S _ M a i n ]  
 G O  
 / * * * * * *   O b j e c t :     S t o r e d P r o c e d u r e   [ d b o ] . [ A I M S _ C a l c _ 1 3 0 ]         S c r i p t   D a t e :   0 3 / 0 1 / 2 0 1 3   0 8 : 1 7 : 1 7   * * * * * * /  
 S E T   A N S I _ N U L L S   O N  
 G O  
 S E T   Q U O T E D _ I D E N T I F I E R   O F F  
 G O  
 - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  
 - -   P u r p o s e : 	 T h i s   p r o c e d u r e   c a l c u l a t e s   t h e   v a l u e   f o r   D A T A _ I D : 1 3 0   E B I T D A  
 - -  
 - - 	 	 	 S O P I   +   S D P R  
 - -  
 - -   A u t h o r : 	 D a v i d   M u e n c h  
 - -   D a t e : 	 J u l y   2 ,   2 0 1 2  
 - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  
 A L T E R   p r o c e d u r e   [ d b o ] . [ A I M S _ C a l c _ 1 3 0 ] (  
 	 @ I S S U E R _ I D 	 	 	 v a r c h a r ( 2 0 )   =   N U L L 	 	 	 - -   T h e   c o m p a n y   i d e n t i f i e r 	 	  
 , 	 @ C A L C _ L O G 	 	 	 c h a r 	 	 =   ' Y ' 	 	 	 - -   W r i t e   e r r o r s   t o   t h e   l o g   t a b l e  
 )  
 a s  
  
 	 - -   G e t   t h e   d a t a  
 	 s e l e c t   p f . *    
 	     i n t o   # A  
 	     f r o m   d b o . P E R I O D _ F I N A N C I A L S   p f   w i t h   ( n o l o c k )  
 	   w h e r e   D A T A _ I D   =   2 9 	 	 	 	 	 - -   S O P I  
 	       a n d   p f . I S S U E R _ I D   =   @ I S S U E R _ I D  
  
 	 s e l e c t   p f . *    
 	     i n t o   # B  
 	     f r o m   d b o . P E R I O D _ F I N A N C I A L S   p f   w i t h   ( n o l o c k )  
 	   w h e r e   p f . I S S U E R _ I D   =   @ I S S U E R _ I D  
 	       a n d   (     ( p f . P E R I O D _ T Y P E   l i k e   ' Q % '   a n d   p f . D A T A _ I D   =   2 4 ) 	 	 - -   S D P R  
 	               o r   ( p f . P E R I O D _ T Y P E   =   ' A '   a n d   p f . D A T A _ I D   =   2 9 1 )   ) 	 	 	 - -   D e p r e c i a t i o n  
  
 	 - -   A d d   t h e   d a t a   t o   t h e   t a b l e  
 	 B E G I N   T R A N   T 1  
 	 i n s e r t   i n t o   P E R I O D _ F I N A N C I A L S ( I S S U E R _ I D ,   S E C U R I T Y _ I D ,   C O A _ T Y P E ,   D A T A _ S O U R C E ,   R O O T _ S O U R C E  
 	 	     ,   R O O T _ S O U R C E _ D A T E ,   P E R I O D _ T Y P E ,   P E R I O D _ Y E A R ,   P E R I O D _ E N D _ D A T E ,   F I S C A L _ T Y P E ,   C U R R E N C Y  
 	 	     ,   D A T A _ I D ,   A M O U N T ,   C A L C U L A T I O N _ D I A G R A M ,   S O U R C E _ C U R R E N C Y ,   A M O U N T _ T Y P E )  
 	 s e l e c t   a . I S S U E R _ I D ,   a . S E C U R I T Y _ I D ,   a . C O A _ T Y P E ,   a . D A T A _ S O U R C E ,   a . R O O T _ S O U R C E  
 	 	 ,     a . R O O T _ S O U R C E _ D A T E ,   a . P E R I O D _ T Y P E ,   a . P E R I O D _ Y E A R ,   a . P E R I O D _ E N D _ D A T E  
 	 	 ,     a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 	 ,     1 3 0   a s   D A T A _ I D 	 	 	 	 	 	 	 - -   D A T A _ I D : 1 3 0   E B I T D A  
 	 	 ,     a . A M O U N T   +   i s n u l l ( b . A M O U N T ,   0 . 0 )   a s   A M O U N T 	 	 	 - -   S O P I   +   S D P R  
 	 	 ,     ' S O P I ( '   +   C A S T ( a . A M O U N T   a s   v a r c h a r ( 3 2 ) )   +   ' )   +   S D P R ( '     +   C A S T ( i s n u l l ( b . A M O U N T ,   0 . 0 )   a s   v a r c h a r ( 3 2 ) )   +   ' ) '   a s   C A L C U L A T I O N _ D I A G R A M  
 	 	 ,     a . S O U R C E _ C U R R E N C Y  
 	 	 ,     a . A M O U N T _ T Y P E  
 	     f r o m   # A   a  
 	     l e f t   j o i n 	 # B   b   o n   b . I S S U E R _ I D   =   a . I S S U E R _ I D    
 	 	 	 	 	 a n d   b . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E   a n d   b . P E R I O D _ T Y P E   =   a . P E R I O D _ T Y P E  
 	 	 	 	 	 a n d   b . P E R I O D _ Y E A R   =   a . P E R I O D _ Y E A R   a n d   b . F I S C A L _ T Y P E   =   a . F I S C A L _ T Y P E  
 	 	 	 	 	 a n d   b . C U R R E N C Y   =   a . C U R R E N C Y  
 	   w h e r e   1 = 1    
 - - 	   o r d e r   b y   a . I S S U E R _ I D ,   a . C O A _ T Y P E ,   a . D A T A _ S O U R C E ,   a . P E R I O D _ T Y P E ,   a . P E R I O D _ Y E A R ,     a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 C O M M I T   T R A N   T 1  
 	 i f   @ C A L C _ L O G   =   ' Y '  
 	 	 B E G I N  
 	 	 	 - -   E r r o r   c o n d i t i o n s   -   m i s s i n g   d a t a    
 	 	 	 i n s e r t   i n t o   C A L C _ L O G (   L O G _ D A T E ,   D A T A _ I D ,   I S S U E R _ I D ,   P E R I O D _ T Y P E ,   P E R I O D _ Y E A R  
 	 	 	 	 	 	 	 	 ,   P E R I O D _ E N D _ D A T E ,   F I S C A L _ T Y P E ,   C U R R E N C Y ,   T X T )  
 	 	 	 (  
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   1 3 0   a s   D A T A _ I D ,   a . I S S U E R _ I D ,   a . P E R I O D _ T Y P E  
 	 	 	 	 ,     a . P E R I O D _ Y E A R ,   a . P E R I O D _ E N D _ D A T E ,   a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 	 	 	 ,   ' W A R N I N G   c a l c u l a t i n g   1 3 0 : E B I T D A .     D A T A _ I D : 2 4   S D P R   o r   2 9 1   D e p r e c i a t i o n   i s   m i s s i n g '  
 	 	 	     f r o m   # A   a  
 	 	 	     l e f t   j o i n 	 # B   b   o n   b . I S S U E R _ I D   =   a . I S S U E R _ I D    
 	 	 	 	 	 	 	 a n d   b . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E   a n d   b . P E R I O D _ T Y P E   =   a . P E R I O D _ T Y P E  
 	 	 	 	 	 	 	 a n d   b . P E R I O D _ Y E A R   =   a . P E R I O D _ Y E A R   a n d   b . F I S C A L _ T Y P E   =   a . F I S C A L _ T Y P E  
 	 	 	 	 	 	 	 a n d   b . C U R R E N C Y   =   a . C U R R E N C Y  
 	 	 	   w h e r e   1 = 1   a n d   b . I S S U E R _ I D   i s   N U L L  
  
 	 	 	 )   u n i o n 	 (  
  
 	 	 	 - -   E r r o r   c o n d i t i o n s   -   m i s s i n g   d a t a    
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   1 3 0   a s   D A T A _ I D ,   a . I S S U E R _ I D ,   a . P E R I O D _ T Y P E  
 	 	 	 	 ,     a . P E R I O D _ Y E A R ,     a . P E R I O D _ E N D _ D A T E ,     a . F I S C A L _ T Y P E ,     a . C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   1 3 0 : E B I T D A .     D A T A _ I D : 2 9   S O P I   i s   m i s s i n g '   a s   T X T  
 	 	 	     f r o m   # B   a  
 	 	 	     l e f t   j o i n 	 # A   b   o n   b . I S S U E R _ I D   =   a . I S S U E R _ I D    
 	 	 	 	 	 	 	 a n d   b . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E   a n d   b . P E R I O D _ T Y P E   =   a . P E R I O D _ T Y P E  
 	 	 	 	 	 	 	 a n d   b . P E R I O D _ Y E A R   =   a . P E R I O D _ Y E A R   a n d   b . F I S C A L _ T Y P E   =   a . F I S C A L _ T Y P E  
 	 	 	 	 	 	 	 a n d   b . C U R R E N C Y   =   a . C U R R E N C Y  
 	 	 	   w h e r e   1 = 1   a n d   b . I S S U E R _ I D   i s   N U L L  
 	 	 	 )   u n i o n 	 (  
 	 	 	 - -   E R R O R   -   N o   d a t a   a t   a l l   a v a i l a b l e  
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   1 3 0   a s   D A T A _ I D ,   i s n u l l ( @ I S S U E R _ I D ,   '   ' )   a s   I S S U E R _ I D ,   '   '   a s   P E R I O D _ T Y P E  
 	 	 	 	 ,     0   a s   P E R I O D _ Y E A R ,     ' 1 / 1 / 1 9 0 0 '   a s   P E R I O D _ E N D _ D A T E ,     '   '   a s   F I S C A L _ T Y P E ,     '   '   a s   C U R R E N C Y  
 	 	 	 	 ,   ' W A R N I N G   c a l c u l a t i n g   1 3 0 : E B I T D A .     D A T A _ I D : 2 4   S D P R   o r   2 9 1   D e p r e c i a t i o n   n o   d a t a '   a s   T X T  
 	 	 	     f r o m   ( s e l e c t   C O U N T ( * )   C N T   f r o m   # B   h a v i n g   C O U N T ( * )   =   0 )   z  
 	 	 	 )   u n i o n 	 (  
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   1 3 0   a s   D A T A _ I D ,   i s n u l l ( @ I S S U E R _ I D ,   '   ' )   a s   I S S U E R _ I D ,   '   '   a s   P E R I O D _ T Y P E  
 	 	 	 	 ,     0   a s   P E R I O D _ Y E A R ,     ' 1 / 1 / 1 9 0 0 '   a s   P E R I O D _ E N D _ D A T E ,     '   '   a s   F I S C A L _ T Y P E ,     '   '   a s   C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   1 3 0 : E B I T D A .     D A T A _ I D : 2 9   S O P I   n o   d a t a '   a s   T X T  
 	 	 	     f r o m   ( s e l e c t   C O U N T ( * )   C N T   f r o m   # A   h a v i n g   C O U N T ( * )   =   0 )   z  
 	 	 	 )  
 	 	 E N D  
 	 	  
 	 - - -   C l e a n   u p  
 	 d r o p   t a b l e   # A  
 	 d r o p   t a b l e   # B  
 