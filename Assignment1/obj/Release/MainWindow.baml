   M S B A M L   `   `   `  ����   Assignment1    Assignment1.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $
��Main��  � Title$?  9Main | Cryptography Assignments, Fall 2013 - Morten Salte��$	��600��$	��800�� � 
ResizeMode$ 	CanResize=�.���� .���  � RowDefinitions �� $
��Auto ��� $��* �.+��� $��0��$��0,0,0,1q�
�������._�o� $��4q�.@�m� $��_File��.@�m� -    m�Name$ ImportFileMenuItem��$��_Import File...�� m� Icon �� $��Images/Import.png��m� -   $ OpenLogMenuItem��$��	_Open Log�� �� $��Images/Log.png���� m� -   $ ExitMenuItem��$��_Exit�� �� $��Images/Logout.png��m� $��_Options��.@�m� $��_Enable Logging�� m� IsCheckable .  m� 	IsChecked ��  �� ElementName$
 Main�� �� Path$ IsLoggingEnabled�	 �� Mode$	 TwoWay=�m� $��_Help��.@�m� -   $ OpenUseGuideMenuItem��$��
_Use Guide�� �� $��Images/Help.png��� $��10q�$��1�� �� $
��Auto ��� $
��Auto �.+��� $��0��
�������$��1q�
 �� Padding$
 10q�$��10q�
�������._�� $	��Top=� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $��50 ��� $
��Auto ��� $��* ��� $
��Auto �.+��� $��Enable logging��$��0��$��1��$��5q� d� 	IsChecked �� $
 Main��$ IsLoggingEnabled�$	 TwoWay=��� -    ��Name$ OpenLogButton��$��0��$��3��$��5q�$��5q�$��Right=�.����  �� Orientation$ 
Horizontal=�.+��� $��Images/Log.png��$��20��$��20���� $��Open log��$��5,0,0,0q�$��Center=��� $
��Bold"�$	��Key��$��1��$��0��$��5q��� -    ��Name$ 
KeyTextBox��$��1��$��1��$��5q�$��25��$��Center=�$��3������ $
 Main��$ InputState.Key� �� 	MaxLength �� $
 Main��$ KeyInputMaxLength��� $
��Bold"�$��Input��$��2��$��0�� �� Padding$ 5q��� -   $ InputTextBox��$��2��$��1��$��3��$��5q�$	��Top=� �� AcceptsReturn . $��Visible=�$	��100�� �� TextWrapping$
 Wrap=����� $
 Main��$ InputState.Input��� $
��Bold"�$��Action��$��3��$��0��$ 5q��� -	    ��Name$ ActionComboBox��$��3��$��1��$��5q� �� SelectedIndex$ 0������ $
 Main��$ 
ActionList� �� SelectedValue �� $
 Main��$ InputState.Action�$	 OneWay=�� -
    �Name$ ProgressBar��$��3��$��2��$��2��$��20��$��5q� � Minimum$ 0Q� � Maximum �� $
 Main��$ ProgressBarState.Max� � Value �� $
 Main��$ ProgressBarState.Current��� $��4��$��1��$��2��$��Center=�$��5q����� $
 Main��$ InputValidationMessage����� $
 Main��$! InputValidationMessageColor��� $ 
Horizontal=�$��4��$��3��$��5q�.+��� -   $��ClearInputButton��$��5q�$��0,0,5,0q�.���� $ 
Horizontal=�.+��� $��Images/Reset.png��$��20��$��20���� $��Clear input��$��Center=�$��5,0,0,0q��� -   $��SubmitButton��$��5q�.���� $ 
Horizontal=�.+��� $��Submit��$��0,0,5,0q�$��Center=��� $��Images/Submit.png��$��20��$��20���� $��1��
�������$��1q�$
 10q�$��10q�
�������._�� $	��Top=� �� $
��Auto ��� $
��Auto � �� $��50 ��� $��* ��� $
��Auto �.+��� $��Output��$
��Bold"�$��0��$��0��$��5q��� $��0��$��1��$	��100��$��5q�$	��Top=�$
 Wrap=�$��2�� . $��Visible=����� $
 Main��$ Output�$	 TwoWay=��� -   $��
CopyButton��$��1��$��5q�$��5q�$��2��.���� $ 
Horizontal=�.+��� $��Copy to input��$��0,0,5,0q�$��Center=��� $��Images/Copy.png��$��20��$��20��