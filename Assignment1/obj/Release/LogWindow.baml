   M S B A M L   `   `   `  ����   Assignment1!    AssignmentsDAT510.LogWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $��LogMain��  � Title$>  8Log | Cryptography Assignments, Fall 2013 - Morten Salte��$	��500��$	��650��.���  � RowDefinitions �� $
��Auto ��� $��* �.+��� $��0��$��0,0,0,1q�
�������._�o� $��4q�.@�m� $��_File��.@�m� -    m�Name$ CloseMenuItem��$��_Close�� m� Icon �� $��Images/Logout.png��� $��1��$��10q� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$>��8These are all the previously encrypted/decrypted actions��$��5q��� -    ��Name$ LogListView��$��1��$��5q�����  �� ElementName$ LogMain�� �� Path$	 Log�����  �� 
TargetType" ��.���  �� Property � ��	 �� Value$	 25��-   
 �� View
 �� .P���  �� Width$
 Auto��$��Logged�� �� DisplayMemberBinding �� *LoggedFormatted+�� $
 Auto��$	��Key�� �� *Key+�� $	 200��$��Input�� �� *Input+�� $
 Auto��$��Action�� �� *Action+� $��2�� � ColumnDefinitions �� $��* ��� $
��Auto ��� $
��Auto �.+��� -    ��Name$ ClearButton��$��0��$
��Left=�$��	Clear Log��$��5q�$	��100��$��5q��� -   $ OkButton��$��1��$��OK��$��5q�$��5q�$	��100���� -   $ CloseButton��$��2��$��Cancel��$��5q�$��5q�$	��100��