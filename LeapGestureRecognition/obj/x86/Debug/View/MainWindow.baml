   M S B A M L   `   `   `  ����R  NSharpGL.WPF, Version=2.3.0.1, Culture=neutral, PublicKeyToken=27fc851303210b27>.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPFSharpGL.WPF  5   	    LeapGestureRecognition'   !LeapGestureRecognition.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   BsharpGL.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPF     5   	     � Title$  SharpGL WPF Application��5   	   $	��350��6)   $	��525��66    � DataContext 5      �� *MainViewModel+.��5	      �  � RowDefinitions 5
      �� 5      $
��Auto �6   �� 5      .+�5      �� 
�������$��0��6&    �� Orientation$ 
Horizontal=�63    � 	Resources 5       �� 
TargetType �� Property �� Value��%X   '��       �� 5      " ��6   .�5      ��  � ��$ 10,5,0,5q�6    5      5      .+�5      �� 
Option 16   �� 5      
Option 26   �� 5      
Option 36   �� 5      
Option 46   5      � 5      $��1��6	    � ColumnDefinitions 5      �� 5       $��1* �6   �� 5!      $��8* �6    5#      �� 5$      $��6* �6   �� 5%      $��1* �6   .+�5)      �� 
�������$ Vertical=�6'   $��0��6>   $��2��6N   $��0��6_   $��Stretch=�6l   $��Stretch=�6�    5*      ��%X   '��       �� 5+      " ��6   .�5,      ��  � ��$ 10,5,0,5q�6!   5-      5.      .+�50      �� Item 16   �� 51      Item 26   �� 52      Item 36   �� 53      Item 46   �� 54      Item 56   �� 55      Item 66   56         SharpGL.WPF.OpenGLControl  5:      -   5        $��openGLControl��5;      	   DrawFPS	 . 
   RenderContextType$	
 FBO=�5=         	FrameRate$ 20Q�64   $��1��5>      $��0��6   �� 5A      
������$��1��6,   $��1��6<   42The output window content should be displayed here6}   5C      5E      5F      