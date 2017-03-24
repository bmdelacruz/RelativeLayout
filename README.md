# WPF RelativeLayout Control
A custom WPF control that mimics the behaviour of Android's RelativeLayout. This reduces the usage of multiple nested panels and such.

## Sample
This is the output of the test application. In this sample, only the RelativeLayout is used here to contain the 9 Grids and position them relative to each other.

<img src="/../master/Screenshots/TestWindow.PNG?raw=true">

Here's a snippet of the XAML code from the test application.

```xml
...
<RelativeLayout>
    <Grid x:Name="aqua" 
          Background="Aqua"
          Width="150" Height="150"
          RelativeLayout.CenterInParent="True"/>

    <Grid x:Name="bisque"
          Width="20"
          Background="Bisque"
          RelativeLayout.AlignParentBottom="True"
          RelativeLayout.AlignTop="aqua"
          RelativeLayout.ToRightOf="aqua"/>

    <Grid x:Name="darkBlue"
          Height ="20"
          Background="DarkBlue"
          RelativeLayout.Above="aqua"
          RelativeLayout.AlignLeft="aqua"
          RelativeLayout.AlignParentRight="True"/>
...
```

## How to include in project
1. Download the DLL from [here](https://github.com/bmdelacruz/RelativeLayout/releases/download/0.1.1/Map.Controls.RelativeLayout.dll).
2. Add the downloaded DLL as a reference in your WPF Project.

## Reference
### RelativeLayout's child properties
- __ToLeftOf__ : string. The name of the target control  
  The control will be placed on the __left__ of the specified target control.
  
- __ToRightOf__ : string. The name of the target control  
  The control will be placed on the __right__ of the specified target control.
  
- __Above__ : string. The name of the target control  
  The control will be placed __above__ the specified target control.
  
- __Below__ : string. The name of the target control  
  The control will be placed __below__ the specified target control.
  
- __AlignLeft__ : string. The name of the target control  
  The control's left edge will be aligned to the __left__ edge of the specified target control.
  
- __AlignRight__ : string. The name of the target control  
  The control's right edge will be aligned to the __right__ edge of the specified target control.
  
- __AlignTop__ : string. The name of the target control  
  The control's top edge will be aligned to the __top__ edge of the specified target control.
  
- __AlignBottom__ : string. The name of the target control  
  The control's bottom edge will be aligned to the __bottom__ edge of the specified target control.
  
- __AlignParentLeft__ : boolean.  
  The control's left edge will be aligned to the __left__ edge of the RelativeLayout.
  
- __AlignParentRight__ : boolean.  
  The control's right edge will be aligned to the __right__ edge of the RelativeLayout.
  
- __AlignParentTop__ : boolean.  
  The control's top edge will be aligned to the __top__ edge of the RelativeLayout.
  
- __AlignParentBottom__ : boolean.  
  The control's bottom edge will be aligned to the __bottom__ edge of the RelativeLayout.
  
- __CenterInParent__ : boolean.  
  The control will be centered, horizontally and vertically, inside the RelativeLayout.
  
- __CenterHorizontal__ : boolean.  
  The control will be centered horizontally inside the RelativeLayout.
  
- __CenterVertical__ : boolean.  
  The control will be centered vertically inside the RelativeLayout.
