   M S B A M L   `   `   `  ����   Assignment2    Assignment2.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $
��Main��  � Title$?  9Main | Cryptography Assignments, Fall 2013 - Morten Salte��$	��600��$	��800��.��� .+��  � ColumnDefinitions �� $��* ��� $��* � � RowDefinitions �� $
��Auto ��� $��* �.+�� $��0��$��2��$��10q�.+�� .+���  �� Padding$ 15q�
�����w�$��1q� �� CornerRadius$ 7������  �� ElementName$
 Main�� �� Path$ MessageBackgroundColor�._��  �� $
��Auto ��� $��* �.+��� $��Images/ChatBubble.png��$��0��$��20��$��20���� $��1��$��5,0,0,0q����� $
 Main��$ Message�� $��0��$��10q�$��1��.+��  �� $
��Auto ��� $��* ��� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�� $��0�� �� $��* ��� $
��Auto �.+��� $��Input��$��0��$��5q�$��16(���  �� Orientation$ 
Horizontal=�$��1��$��Bottom=�$��5q�.+�	� -   $��InputTypeAutoRadioButton��$��5,0,0,0q� 	� 	GroupName$ 	InputType��$
��Auto��	 d� 	IsChecked	 �� $
 Main��$ IsInputTypeAutoChecked�
 �� Mode$
 TwoWay=�	� -   $ ��InputTypeBinaryRadioButton��$��5,0,0,0q�$ 	InputType��$��Binary��	 �� $
 Main��$ IsInputTypeBinaryChecked�$
 TwoWay=�	� -   $��InputTypeTextRadioButton��$��5,0,0,0q�$ 	InputType��$
��Text��	 �� $
 Main��$ IsInputTypeTextChecked�$
 TwoWay=��� -   $��InpuTextBox��$��1��$��5q�$��Visible=� �� TextWrapping$
 Wrap=����� $
 Main��$ Input�$
 TwoWay=�� $��2�� �� $��* ��� $
��Auto ��� $
��Auto �.+��� $��Output��$��0��$��5q�$��16(�$��Bottom=��� -   $��MoveUpButton��$��1��$��5q�$��30��$��25��$��5q�.���� $��Images/Up.png���� -   $��MoveDownButton��$��2��$��5q�$��30��$��25��$��5q�.���� $��Images/Down.png���� -   $��OutputTextBox��$��3��$��5q� �� AcceptsReturn .  �� 
IsReadOnly . $��Visible=�$
 Wrap=����� $
 Main��$ Output�$
 TwoWay=�� $��4��$��15��$��5q� � IsIndeterminate �� $
 Main��$ IsCryptographyInProgress�� $��5�� �� $��* ��� $��* �.+��� -	   $��ActionComboBox��$��5q�$��0�� �� SelectedIndex$ 0������ $
 Main��$ 
ActionList��� -
   $��OkButton��$��OK��$��5q�$��1��$��25��{��� $
 Main��$ 
CanClickOK�� $��1��$��10q�$��1��.+��  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* �.+��� $��0��$��Right=�$��RSA parameters��$��5q�$��16(�� $��1�� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��Prime Length:��$��5q�$��Center=��� -   $��PrimeLengthTextBox��$��1��$��5q�$��Center=����� $
 Main��$ PrimeLength�$
 TwoWay=��� -   $��GenerateButton��$��2��$��Generate��$��5q�$��25��$	��100��{��� $
 Main��$ CanClickGenerate�� $��2��$��15��$��5q� �� $
 Main��$ IsGenerationInProgress�� $��3�� �� $
��Auto ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��2��$��0��$��5q�$��Right=����� $
 Main��$" GenerationTimeElapsedMessage��� $��p��$��0��$��1��$��5q�$��50���� $��25��$��Center=�$��1�� . $��1��$��5q�
����������� $
 Main��$ RSAParameters.P.AsString�$
 OneWay=��� $��q��$��0��$��2��$��5q�$��50���� $��25��$��Center=�$��1��$��2�� . $��5q�
����������� $
 Main��$ RSAParameters.Q.AsString�$
 OneWay=��� $��n��$��0��$��3��$��5q�$��50���� -   $��RsaNTextBox��$��25��$��Center=�$��1��$��3��$��5q����� $
 Main��$ RSAParametersColor� �� $
 Main��$  IsRSAParametersLockChecked����� $
 Main��$ RSAParameters.N.AsString�$
 OneWay=��� $��d��$��0��$��4��$��5q�$��50���� -   $��RsaDTextBox��$��25��$��Center=�$��1��$��4��$��5q����� $
 Main��$ RSAParametersColor� �� $
 Main��$  IsRSAParametersLockChecked����� $
 Main��$ RSAParameters.D.AsString�$
 OneWay=��� $��e��$��0��$��5��$��5q�$��50���� -   $��RsaETextBox��$��25��$��Center=�$��1��$��5��$��5q����� $
 Main��$ RSAParametersColor� �� $
 Main��$  IsRSAParametersLockChecked����� $
 Main��$ RSAParameters.E.AsString�$
 OneWay=��� -   $��LockParametersCheckBox��$��6��$��2��$��0��$��5q�$��Locked��$��Right=�	 �� $
 Main��$  IsRSAParametersLockChecked�$
 TwoWay=��� $��7��$��5q�$��0��$��2��$��
PR = {d,n}���� $��8��$��5q�$��0��$��2��$��
PU = {e,n}��