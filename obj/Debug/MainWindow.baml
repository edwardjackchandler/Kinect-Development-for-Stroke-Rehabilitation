   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   	    Kinect12-02-15!   SkeletalTracking.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   	   -   5          � Title$2  ,Kinect Development for Stroke Rehabilitation��5   	   $	��720��6?   $
��1280��6O   .��5      �� -   5        $��
MainCanvas��5      .+�5   
   �� $
��1027��6   $��
Image2.png��6   $
��Fill=�62   $	��353��6A    �� Left$	 971��6M   
 �� Top$
 -130��6_   ;   5Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer  5	   
   -   5          Name$ kinectColorViewer1��5	   H   $ 0��6   $	��640��6/   $	��480��6;   5   /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect 5
      �� *Kinect+ �� ElementName$ kinectSensorChooser1��6[   A� 5   
   -   5         A�Name$ leftEllipse��5   =   $ 0��6   $ 0��6"   $��20��61   $��20��6S   ���	#FF4D298D J� Opacity$ 1Q�6o   
�������6{   A� 5   
   -   5        $ rightEllipse��5   P   $	 100��6   $ 0��6$   ���	#FF2CACE3$��20��6D   $��20��6f   $ 1Q�6q   
�������6}   A� 5   
   -   5        $ rightKneeEllipse��5   J   $	 150��6   $ 0��6$   
���  ��$��20��6>   $��20��6d   $ 0Q�6o   
�������6{   A� 5   
   -   5        $ leftKneeEllipse��5   P   $	 200��6   $ 0��6$   ���	#FFE3902C$��20��6D   $��20��6i   $ 0Q�6t   
�������6�   A� 5   
   -   5        $ 	headImage��5   P   $	 250��6   $ 0��6$   ���	#FF346142$��40��6D   $��40��6c   $ 1Q�6n   
�������6z   A� 5   
   -	   5        $ 
hipEllipse��5   U   $ 65��6   $ 15��6#   
�������$��20��6I   $��20��6i   $ 1Q�6t   
�������6�   A� 5   
   -
   5        $ rightFootEllipse��5   P   $	 300��6   $ 0��6$   ���	#FFE3902C$��20��6D   $��20��6j   $ 0Q�6u   
�������6�   A� 5   
   -   5        $ leftFootEllipse��5   J   $	 350��6   $ 0��6$   
���  ��$��20��6>   $��20��6c   $ 0Q�6n   
�������6z   =   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  5   
   -   5          Name$ kinectSensorChooser1��5   D   $	 451��6!   $	 272��63   $	��328��6b   6n   �� 5   
   -   5        	 ��Name$	 tbTime��5      $��65(�6$   $	��640��62   
 �� TextAlignment$
 Center=�6>   
�������$	��210��6h   $	 480��6u   �� 6�    � Language$ en-gb�6�   $� ��6�   6�   �� 5   
   -   5        $	 
motivation��5      $��45(�6(   $	��366��66   $	 687��6B   $	��478��6T   $	 211��6a   �� 6t   $ en-gb�6x   $� ��6�   6�   �� 5   
   -   5        $	 percent��5      $��65(�6%   ���#7030A0$	��316��6H   $	 683��6T   $	��170��6f   $ 16��6s   �� 6�   $ en-gb�6�   $� ��6�   6�   �� 5   
   -   5        $��LEFT ARM��5      $��30(�6$   $��48��62   $
 1058��6>   $ 15��6Q   $	��204��6a   6m   �� 5   
   -   5        $��	RIGHT ARM��5      $��30(�6%   $��48��63   $
 1058��6?   $ 72��6R   $	��204��6b   6n   �� 5   
   -   5        $��LEFT LEG��5      $��30(�6$   $��48��62   $
 1058��6>   $	 130��6Q   $	��204��6b   6n   �� 5   
   -   5        $��	RIGHT LEG��5      $��30(�6%   $��48��63   $
 1058��6?   $	 188��6R   $	��204��6c   6o   �� 5   
   -   5        $��SEATED RIGHT
        ARM��5      $��Center=�6=   $��30(�6a   $��88��6o   $
 1058��6{   $	 245��6�   $	��204��6�   6�   �� 5   
   -   5        $��SEATED LEFT
      ARM��5      $��Center=�65   $��30(�6Y   $��88��6g   $
 1058��6s   $	 344��6�   $	��204��6�   6�   � 5   
   $	��363��6   $	��325��6   $	 755��6(   $	 272��6:   .+�5      r� -   5         r�Name$  Instructional_Video_Player��5      $	��578��6@   $	��325��6L    r� LoadedBehavior$ Manual=�6Z    r� Stretch$ UniformToFill=�6r   $ 1Q�6�    r� Source$l fC:\Users\Jack\Documents\Visual Studio 2012\Projects\Kinect15-04-15\Instructional Videos\leftArmMov.mp4?�5      $	 840��6�   $	 370��6�   $��Center=�6�   $��Center=�6�   5       5!      5"      